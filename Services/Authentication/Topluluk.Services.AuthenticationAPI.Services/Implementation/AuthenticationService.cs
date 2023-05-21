using System.Collections;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RestSharp;
using Topluluk.Services.AuthenticationAPI.Data.Interface;
using Topluluk.Services.AuthenticationAPI.Model.Dto;
using Topluluk.Services.AuthenticationAPI.Model.Dto.Http;
using Topluluk.Services.AuthenticationAPI.Model.Entity;
using Topluluk.Services.AuthenticationAPI.Services.Interface;
using Topluluk.Shared.Constants;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Helper;
using Topluluk.Shared.Messages;
using Topluluk.Shared.Messages.Authentication;
using _MassTransit = MassTransit;
using ResponseStatus = Topluluk.Shared.Enums.ResponseStatus;

namespace Topluluk.Services.AuthenticationAPI.Services.Implementation
{
	public class AuthenticationService : IAuthenticationService
	{
        private readonly IAuthenticationRepository _repository;
        private readonly ILoginLogRepository _loginLogRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly RestClient _client;
        private readonly _MassTransit.ISendEndpointProvider _endpointProvider;
        public AuthenticationService(IAuthenticationRepository repository, _MassTransit.ISendEndpointProvider endpointProvider, IMapper mapper, IConfiguration configuration, ILoginLogRepository loginLogRepository)
		{
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
            _loginLogRepository = loginLogRepository;
            _endpointProvider = endpointProvider;
            _client = new RestClient();
		}

        public async Task<Response<TokenDto>> SignIn(SignInUserDto userDto, string? ipAdress, string? deviceId)
        {
            TokenHelper _tokenHelper = new TokenHelper(_configuration);

            UserCredential? user = new();
        
            if (!userDto.UserName.IsNullOrEmpty())
            {
                user = await _repository.GetFirstAsync(u => u.UserName == userDto.UserName && u.Provider == userDto.Provider);
            }
            else if (!userDto.Email.IsNullOrEmpty())
            {
                user = await _repository.GetFirstAsync(u => u.Email == userDto.Email && u.Provider == userDto.Provider);
            }

            if(user != null)
            {
                var verifiedPassword = VerifyPassword(userDto.Password, user.HashedPassword);
                if(verifiedPassword)
                {
                    // Dead code fix later.b
                    if (DateTime.Now < user.LockoutEnd)
                    {
                        return await Task.FromResult(Response<TokenDto>.Fail($"User locked until {user.LockoutEnd}", ResponseStatus.AccountLocked));
                    }
                    TokenDto token = _tokenHelper.CreateAccessToken(user.Id, user.UserName, user.Role, 2);
                    user.AccessFailedCount = 0;
                    user.LockoutEnd = DateTime.MinValue;
                    user.Locked = false;
                    
                    UpdateRefreshToken(user,token,2);

                    LoginLog loginLog = new() { UserId = user.Id, IpAdress = ipAdress, DeviceId = deviceId };
                    await _loginLogRepository.InsertAsync(loginLog);

                    return await Task.FromResult(Response<TokenDto>.Success(token, ResponseStatus.Success));
                }
                // User found but password wrong.

                if(user.AccessFailedCount < 15)
                {
                    user.AccessFailedCount += 1;
                }
                else
                {
                    user.Locked = true;
                    user.LockoutEnd = DateTime.Now.AddMinutes(20);

                    // todo We have to send a mail to user about someone wants to login without permission him/her account.
                }
                _repository.Update(user);
            }
            return await Task.FromResult(Response<TokenDto>.Fail("Username or password wrong!", ResponseStatus.NotAuthenticated));
        }

        public async Task<Response<TokenDto>> SignUp(CreateUserDto userDto)
        {
            var checkUniqueResult = await CheckUserNameAndEmailUnique(userDto.UserName, userDto.Email);

            if (!checkUniqueResult.IsSuccess)
            {
                return Response<TokenDto>.Fail(checkUniqueResult.Errors, ResponseStatus.InitialError);
            }

            try
            {
                var response = await _repository.InsertAsync(new UserCredential
                {
                    UserName = userDto.UserName,
                    Email = userDto.Email,
                    Provider = userDto.Provider,
                    HashedPassword = HashPassword(userDto.Password),
                });

                var content = new UserInsertDto
                {
                    Id = response.Data,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    UserName = userDto.UserName,
                    Email = userDto.Email,
                    BirthdayDate = DateTime.Now,
                    Gender = userDto.Gender
                };

                var userInsertRequest = new RestRequest("https://localhost:7202/user/insertuser").AddBody(content);
                var userInsertResponse = await _client.ExecutePostAsync(userInsertRequest);

                if (!userInsertResponse.IsSuccessful)
                {
                    _repository.DeleteCompletely(response.Data);
                    return Response<TokenDto>.Fail("Error occurred while user inserting!", ResponseStatus.InitialError);
                }

                var role = new List<string>() { UserRoles.USER };
                var token = new TokenHelper(_configuration).CreateAccessToken(response.Data, userDto.UserName, role ,2);
                var user = _repository.GetFirst(u => u.UserName == userDto.UserName);
                UpdateRefreshToken(user, token, 2);
                
                var sendEndpoint = await _endpointProvider.GetSendEndpoint(new Uri(QueueConstants.SUCCESSFULLY_REGISTERED_MAIL));
                var registerMessage = new SuccessfullyRegisteredCommand
                {
                    To = userDto.Email,
                    FullName = $"{userDto.FirstName} {userDto.LastName}"
                };
                sendEndpoint.Send<SuccessfullyRegisteredCommand>(registerMessage);

                return Response<TokenDto>.Success(token, ResponseStatus.Success);
            }
            catch (Exception e)
            {
                return Response<TokenDto>.Fail($"Some error occurred {e}", ResponseStatus.InitialError);
            }
        }


        public async Task<Response<string>> SignOut(string userId,SignOutUserDto userDto)
        {
            try
            {
                if (userId.IsNullOrEmpty() || userDto.RefreshToken.IsNullOrEmpty())
                    throw new Exception("User Not Found");

                UserCredential? user = await _repository.GetFirstAsync(u => u.Id == userId);

                if (user != null)
                {
                    user.RefreshToken = null;
                    _repository.Update(user);
                    return await Task.FromResult(Response<string>.Success("Signout successfully!",
                        ResponseStatus.Success));
                }

                return await Task.FromResult(Response<string>.Fail("User not found!", ResponseStatus.NotFound));


            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Some error occurred: {e}",
                    ResponseStatus.InitialError));
            }
        }
        public async Task<Response<string>> ResetPasswordRequest(string email)
        {
            UserCredential? user = await _repository.GetFirstAsync(u => u.Email == email);
            if (user != null)
            {
                Random random = new Random();
                int randomNumber = random.Next(100000, 999999);
                user.ResetPasswordCode = randomNumber.ToString();
                _repository.Update(user);
                var sendEndpoint = await _endpointProvider.GetSendEndpoint(new Uri("queue:reset-password"));
                var registerMessage = new ResetPasswordCommand()
                {
                    To = email,
                    UserId = user.Id,
                    Code = randomNumber.ToString()
                };
                sendEndpoint.Send<ResetPasswordCommand>(registerMessage);
                return await Task.FromResult(Response<string>.Success("", ResponseStatus.Success));
            }
            return await Task.FromResult(Response<string>.Fail("User not found", ResponseStatus.NotFound));
        }

        public async Task<Response<NoContent>> ResetPasswordCheckOTP(ResetPasswordCheckOTPDto codeDto)
        {
            try
            {
                UserCredential user = await _repository.GetFirstAsync(u => u.Email == codeDto.Mail);
                if (user == null)
                {
                    return Response<NoContent>.Fail("User Not Found", ResponseStatus.NotFound);
                }
                if (user.ResetPasswordCode != codeDto.Code)
                {
                    return Response<NoContent>.Fail("UnAuthorized", ResponseStatus.Unauthorized);
                }

                return Response<NoContent>.Success(ResponseStatus.Success);
            }
            catch (Exception e)
            {
                return Response<NoContent>.Fail(e.ToString(),ResponseStatus.InitialError);
            }
        }


        public async Task<Response<NoContent>> ResetPassword( ResetPasswordDto passwordDto)
        {
            UserCredential? user = await _repository.GetFirstAsync(u => u.Email == passwordDto.Mail);

            if (passwordDto.NewPassword != passwordDto.NewPasswordAgain)
            {
                return await Task.FromResult(Response<NoContent>.Fail("Passwords does not match!", ResponseStatus.BadRequest));
            }
            if (user != null)
            {
                if (user.ResetPasswordTokenEndDate < DateTime.Now)
                {
                    return await Task.FromResult(Response<NoContent>.Fail("Token expired", ResponseStatus.NotAuthenticated));
                }

                if (user.ResetPasswordCode == passwordDto.Code)
                {
                    user.HashedPassword = HashPassword(passwordDto.NewPassword);
                    user.ResetPasswordCode = null;
                    user.ResetPasswordTokenEndDate = null;
                    _repository.Update(user);
                    return await Task.FromResult(Response<NoContent>.Success(ResponseStatus.Success));
                }

            }

            return await Task.FromResult(Response<NoContent>.Fail("Failed", ResponseStatus.Failed));
        }

        public async Task<Response<string>> ChangePassword(string userId, PasswordChangeDto passwordDto)
        {
            try
            {
                UserCredential? user = await _repository.GetFirstAsync(u => u.Id == userId);
                
                if (user == null)
                {
                    return await Task.FromResult(Response<string>.Fail("Not Found",
                        ResponseStatus.NotFound));
                }

                var verifiedPassword = VerifyPassword(passwordDto.OldPassword, user.HashedPassword);

                if (verifiedPassword == false)
                {
                    return await Task.FromResult(Response<string>.Fail("Not authenticated",
                        ResponseStatus.NotAuthenticated));
                }

                user.HashedPassword = HashPassword(passwordDto.NewPassword);
                _repository.Update(user);

                return await Task.FromResult(Response<string>.Success("Success", ResponseStatus.Success));

            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Some error occurred {e}",
                    ResponseStatus.InitialError));
            }
        }

        // UserName and Email must be unique
        private async Task<Response<string>> CheckUserNameAndEmailUnique(string userName, string email)
        {
            DatabaseResponse response = new();
            var _userName =  await _repository.GetFirstAsync(u => u.UserName == userName);
            if (_userName != null)
                response.ErrorMessage.Add("Username must be unique!");

            var _email =  await _repository.GetFirstAsync(u => u.Email == email);
            if (_email != null)
                response.ErrorMessage.Add("Email already in use!");

            if(_userName == null && _email == null)
            {
                return await Task.FromResult(Response<string>.Success("", ResponseStatus.Success));
            }

            return await Task.FromResult(Response<string>.Fail(response.ErrorMessage, ResponseStatus.InitialError));
        }


        private void UpdateRefreshToken(UserCredential user, TokenDto token, int month)
        {
            user.RefreshToken = token.RefreshToken;
            user.RefreshTokenEndDate = token.ExpiredAt.AddMonths(month);
            _repository.UpdateRefreshToken(user);
        }
 
        private string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            byte[] hash = GetHash(password, salt);
            return Convert.ToBase64String(salt) + "|" + Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            string[] parts = hashedPassword.Split('|');
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] expectedHash = Convert.FromBase64String(parts[1]);
            byte[] actualHash = GetHash(password, salt);
            return StructuralComparisons.StructuralEqualityComparer.Equals(actualHash, expectedHash);
        }

        private byte[] GetHash(string password, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] passwordAndSalt = new byte[passwordBytes.Length + salt.Length];
                Buffer.BlockCopy(passwordBytes, 0, passwordAndSalt, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, passwordAndSalt, passwordBytes.Length, salt.Length);
                return sha256.ComputeHash(passwordAndSalt);
            }
        }

        public async Task<Response<string>> DeleteUser(string id, UserDeleteDto userDto)
        {
            try
            {
                if (!id.IsNullOrEmpty())
                {
                    _repository.DeleteById(id);
                    return await Task.FromResult(Response<string>.Success("Successfully deleted", ResponseStatus.Success));

                }

                return await Task.FromResult(Response<string>.Fail("UnAuthorized", ResponseStatus.NotAuthenticated));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Error occured {e}", ResponseStatus.InitialError));
            }
        }

        public async Task<Response<NoContent>> UpdateProfile(string userId, UserUpdateDto userDto)
        {
            try
            {
                if (userId.IsNullOrEmpty())
                {
                    return await Task.FromResult(Response<NoContent>.Fail("", ResponseStatus.Unauthorized));
                }

                UserCredential user = await _repository.GetFirstAsync(u => u.Id == userId);

                if (user == null)
                {
                    return await Task.FromResult(Response<NoContent>.Fail("User not found", ResponseStatus.NotFound));
                }

                if (user.Id != userId)
                {
                    return await Task.FromResult(Response<NoContent>.Fail("UnAuthorized",ResponseStatus.Unauthorized));
                }

                user.UserName = userDto.UserName;
                user.Email = userDto.Email;

                DatabaseResponse response = _repository.Update(user);

                if (response.IsSuccess)
                {
                    return await Task.FromResult(Response<NoContent>.Success(null, ResponseStatus.Success));
                }

                return await Task.FromResult(Response<NoContent>.Fail("Failed", ResponseStatus.InitialError));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<NoContent>.Fail($"Error occured {e}", ResponseStatus.InitialError));
            }
        }
        public string GenerateResetPasswordToken(string email)
        {
            byte[] tokenBytes = new byte[32];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(tokenBytes);
            }

            string token = Convert.ToBase64String(tokenBytes);

            return token;
        }
    }
}