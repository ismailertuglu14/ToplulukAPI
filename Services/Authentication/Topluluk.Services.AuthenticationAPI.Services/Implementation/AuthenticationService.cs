using System;
using System.Collections;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Topluluk.Services.AuthenticationAPI.Data.Implementation;
using Topluluk.Services.AuthenticationAPI.Data.Interface;
using Topluluk.Services.AuthenticationAPI.Model.Dto;
using Topluluk.Services.AuthenticationAPI.Model.Entity;
using Topluluk.Services.AuthenticationAPI.Services.Interface;
using Topluluk.Shared;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;
using Topluluk.Shared.Helper;

namespace Topluluk.Services.AuthenticationAPI.Services.Implementation
{
	public class AuthenticationService : IAuthenticationService
	{
        private readonly IAuthenticationRepository _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

		public AuthenticationService(IAuthenticationRepository repository, IMapper mapper, IConfiguration configuration)
		{
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
		}

        public async Task<Response<TokenDto>> SignIn(SignInUserDto userDto)
        {
            TokenHelper _tokenHelper = new TokenHelper(_configuration);

            UserCredential? user = _repository.GetFirst(u => u.UserName == userDto.UserName);
            
            if(user != null)
            {
                var verifiedPassword = VerifyPassword(userDto.Password, user.HashedPassword);
                if(verifiedPassword == true)
                {

                    if (user.Locked == true)
                    {
                        return await Task.FromResult(Response<TokenDto>.Fail($"User locked until {user.LockoutEnd}", ResponseStatus.NotAuthenticated));
                    }

                    TokenDto token = _tokenHelper.CreateAccessToken(user.Id, user.UserName, 2);
                    UpdateRefreshToken(user,token,2);
                    return await Task.FromResult(Response<TokenDto>.Success(token, ResponseStatus.Success));
                }
                // User found but password wrong.
                else
                {
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

            }

            return await Task.FromResult(Response<TokenDto>.Fail("Username or password wrong!", ResponseStatus.Success));
        }

        public async Task<Response<string>> SignUp(CreateUserDto userDto)
        {
            DatabaseResponse response = new();

            UserCredential userCredential = new()
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                HashedPassword = HashPassword(userDto.Password),
            };

            var checkUniqueResult = await CheckUserNameAndEmailUnique(userCredential.UserName, userCredential.Email);

            if(checkUniqueResult.IsSuccess == true)
            {
                response = await _repository.InsertAsync(userCredential);

                // Request to UserService
                UserInsertDto content = new() { Id = response.Data, FirstName = userDto.FirstName, LastName = userDto.LastName, UserName = userDto.UserName, BirthdayDate = DateTime.Now };
                HttpResponseMessage httpResponse = await HttpRequestHelper.handle(content, "https://localhost:7202/user/insertuser", HttpType.POST);
                return await Task.FromResult(Response<string>.Success("Successfull",ResponseStatus.Success)); 
            }

            return await Task.FromResult(Response<string>.Fail(checkUniqueResult.Errors,ResponseStatus.InitialError));

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

    }
}