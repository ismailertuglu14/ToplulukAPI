using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using RestSharp;
using Topluluk.Services.User.Data.Interface;
using Topluluk.Services.User.Model.Dto;
using Topluluk.Services.User.Model.Entity;
using Topluluk.Services.User.Services.Interface;
using Topluluk.Shared.Constants;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Exceptions;
using _User = Topluluk.Services.User.Model.Entity.User;
using ResponseStatus = Topluluk.Shared.Enums.ResponseStatus;

namespace Topluluk.Services.User.Services.Implementation;

public class FollowService : IFollowService
{

        private readonly IUserRepository _userRepository;
        private readonly IUserFollowRepository _followRepository;
        private readonly IUserFollowRequestRepository _followRequestRepository;
        private readonly IMapper _mapper;
        private readonly RestClient _client;
        public FollowService(IUserRepository userRepository, IUserFollowRepository followRepository, IUserFollowRequestRepository followRequestRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _followRepository = followRepository;
            _followRequestRepository = followRequestRepository;
            _mapper = mapper;
            _client = new RestClient();
        }
        public async Task<Response<NoContent>> FollowUser(string userId, UserFollowDto userFollowInfo)
        {
            
            if (userId == userFollowInfo.TargetId)
                return Response<NoContent>.Fail("You can not follow yourself",
                    ResponseStatus.BadRequest);
                
            _User targetUser = await _userRepository.GetFirstAsync(u => u.Id == userFollowInfo.TargetId);
                
            if(targetUser==null)
                return Response<NoContent>.Fail("User not found", ResponseStatus.NotFound);

            bool isFollowing =
                await _followRepository.AnyAsync(f => !f.IsDeleted && f.SourceId == userId && f.TargetId == userFollowInfo.TargetId);

            if (isFollowing)
                return Response<NoContent>.Success(ResponseStatus.Success);
                
            if (targetUser.IsPrivate)
            {
                var isAlreadySent = await _followRequestRepository.AnyAsync(f =>
                    !f.IsDeleted && f.SourceId == userId && f.TargetId == userFollowInfo.TargetId);
                if (isAlreadySent)
                {
                    return Response<NoContent>.Success(ResponseStatus.Success);
                }

                FollowRequest requestDto = new FollowRequest(userId, targetUser.Id);
                    
                await _followRequestRepository.InsertAsync(requestDto);
                return Response<NoContent>.Success(ResponseStatus.Success);
            }
            else
            {
                UserFollow userFollow = new() { SourceId = userId, TargetId = userFollowInfo.TargetId };
                await _followRepository.InsertAsync(userFollow);

                return Response<NoContent>.Success( ResponseStatus.Success);
            }


        }

        public async Task<Response<NoContent>> UnFollowUser(string userId, UserFollowDto userFollowInfo)
        {
            UserFollow? isSourceFollowingTarget = await _followRepository.GetFirstAsync(f => f.SourceId == userId && f.TargetId == userFollowInfo.TargetId);

            if (isSourceFollowingTarget == null)
            {
                return Response<NoContent>.Fail("Failed", ResponseStatus.Failed);
            }

            _followRepository.DeleteCompletely(isSourceFollowingTarget.Id);

            return Response<NoContent>.Success(ResponseStatus.Success);
        }
        
        public async Task<Response<NoContent>> RemoveFollowRequest(string userId, string targetId)
        {
            try
            {
                FollowRequest? followRequest = await _followRequestRepository.GetFirstAsync(f => f.SourceId == userId && f.TargetId == targetId);
                
                if (followRequest != null)
                {
                    _followRequestRepository.DeleteCompletely(followRequest.Id);
                    return Response<NoContent>.Success(ResponseStatus.Success);
                }

                return Response<NoContent>.Fail("Not Found", ResponseStatus.NotFound);
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<NoContent>.Fail($"{e}", ResponseStatus.InitialError));

            }
        }

        public async Task<Response<string>> RemoveUserFromFollowers(string userId, UserFollowDto userFollowInfo)
        {
            try
            {
                UserFollow? userFollow = await _followRepository.GetFirstAsync(f => f.SourceId == userFollowInfo.TargetId && f.TargetId == userId);

                if (userFollow == null)
                {
                    return await Task.FromResult(Response<string>.Fail("Failed", ResponseStatus.Failed));
                }

                _followRepository.DeleteCompletely(userFollow.Id);
                return await Task.FromResult(Response<string>.Success("Successfully unfollowed!", ResponseStatus.Success));
            }
            catch
            {
                return await Task.FromResult(Response<string>.Fail("Some error occured", ResponseStatus.InitialError));
            }
        }
        public async Task<Response<NoContent>> AcceptFollowRequest(string id, string targetId)
        {
            try
            {

                var document = await _followRequestRepository.GetFirstAsync(f => f.SourceId == targetId && f.TargetId == id);
                if (document == null) throw new ArgumentNullException(nameof(FollowRequest));
                var insertDocument = new UserFollow()
                {
                    SourceId = targetId,
                    TargetId = id,
                };
                await _followRepository.InsertAsync(insertDocument);
                _followRequestRepository.DeleteByExpression(f => f.SourceId == targetId && f.TargetId == id); 

                return Response<NoContent>.Success(ResponseStatus.Success);
            }
            catch (Exception e)
            {
                return Response<NoContent>.Fail(e.ToString(), ResponseStatus.InitialError);
            }
        }
        
        
        
        public async Task<Response<List<UserFollowRequestDto>>> GetFollowerRequests(string id, string userId, int skip = 0, int take = 10)
        {
            try
            {
                if (!id.Equals(userId))
                {
                    return await Task.FromResult(Response<List<UserFollowRequestDto>>.Fail("",
                        ResponseStatus.Unauthorized));
                }

                var incomingRequests = await _followRequestRepository.GetListByExpressionAsync(f => !f.IsDeleted && f.TargetId == userId);

                List<string> requestIds = incomingRequests.Select(i => i.SourceId).ToList();

                List<_User> users = _userRepository.GetListByExpression(u => requestIds.Contains(u.Id));
                var dto = _mapper.Map<List<_User>, List<UserFollowRequestDto>>(users);

                return await Task.FromResult(Response<List<UserFollowRequestDto>>.Success(dto, ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<List<UserFollowRequestDto>>.Fail($"Some error occurred: {e}",
                    ResponseStatus.InitialError));
            }

        }

        public async Task<Response<List<UserSuggestionsDto>>> GetUserFollowSuggestions(string userId)
        {
            // Source user's followings
            var followings = await _followRepository.GetListByExpressionAsync(f => f.SourceId == userId);
            var followingIds = followings.Select(f => f.TargetId).ToList();

            var targetIds =  _followRepository.GetListByExpressionAsync(f => followingIds.Contains(f.SourceId) && f.TargetId != userId).Result
                .Select(f => f.TargetId)
                .Distinct()
                .ToList();

            targetIds.RemoveAll(id => followingIds.Contains(id));
            List<_User> users = await _userRepository.GetListByExpressionAsync(u => targetIds.Contains(u.Id));
            List<UserSuggestionsDto> dtos = _mapper.Map<List<_User>, List<UserSuggestionsDto>>(users);
            
            foreach (var dto in dtos)
            {
                dto.MutualFriendCount = await _followRepository.Count(f => !f.IsDeleted && followingIds.Contains(f.SourceId) && f.TargetId == dto.Id);
                dto.IsFollowRequested = await _followRequestRepository.AnyAsync(f =>
                    !f.IsDeleted && f.SourceId == userId && f.TargetId == dto.Id);
            }
            
            return Response<List<UserSuggestionsDto>>.Success(dtos, ResponseStatus.Success);
        }
        
        
        public async Task<Response<List<string>>> GetUserFollowings(string id)
        {
            _User user = await _userRepository.GetFirstAsync(u => u.Id == id);
            if (user == null)
                return await Task.FromResult(Response<List<string>>.Fail("User Not found", ResponseStatus.BadRequest));
            List<string> followingIds = new();
            var followings = _followRepository.GetListByExpression(f => f.SourceId == id);
            foreach (var follow in followings)
            {
                followingIds.Add(follow.TargetId);
            }
            return await Task.FromResult(Response<List<string>>.Success(followingIds, ResponseStatus.Success));

        }
        
        public async Task<Response<NoContent>> DeclineFollowRequest(string id, string targetId)
        {
            FollowRequest? followRequest = await _followRequestRepository.GetFirstAsync(f => f.SourceId == targetId && f.TargetId == id);

            if (followRequest == null)
                throw new NotFoundException();
                
            _followRequestRepository.DeleteById(followRequest.Id);
            return Response<NoContent>.Success(ResponseStatus.Success);
        }

           // Takip edilen kullanıcıları ve o kullanıcıların bilgilerini getireceğiz
        public async Task<Response<List<FollowingUserDto>>> GetFollowingUsers(string id, string userId, int skip = 0, int take = 10)
        {
            try
            {
                if (userId.IsNullOrEmpty())
                {
                    return await Task.FromResult(Response<List<FollowingUserDto>>.Fail("Bad Request", ResponseStatus.BadRequest));
                }

                _User user = await _userRepository.GetFirstAsync(u => u.Id == userId);
                if (user == null)
                {
                    return await Task.FromResult(Response<List<FollowingUserDto>>.Fail("", ResponseStatus.NotFound));
                }
                // Target Id leri elimde. Bu target id ler ile kullanıcı bilgilerini alıcaz.
                DatabaseResponse followingsResponse = await _followRepository.GetAllAsync(take, skip, f => f.SourceId == userId);
                List<string> x = new();
                foreach (var y in followingsResponse.Data)
                {
                    x.Add(y.TargetId);
                }
                DatabaseResponse followingUsers = await _userRepository.GetAllAsync(take, skip, fu => x.Contains(fu.Id));
                List<FollowingUserDto> dtos = _mapper.Map<List<_User>, List<FollowingUserDto>>(followingUsers.Data);

                return await Task.FromResult(Response<List<FollowingUserDto>>.Success(dtos, ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<List<FollowingUserDto>>.Fail($"Some error occurred: {e}", ResponseStatus.InitialError));
            }
        }

        public async Task<Response<List<FollowerUserDto>>> GetFollowerUsers(string id, string userId, int skip = 0, int take = 10)
        {
            try
            {
                if (id.IsNullOrEmpty() || userId.IsNullOrEmpty())
                {
                    return await Task.FromResult(Response<List<FollowerUserDto>>.Fail("", ResponseStatus.BadRequest));
                }

                _User? user = await _userRepository.GetFirstAsync(u => u.Id == userId);

                if (user == null)
                {
                    return await Task.FromResult(Response<List<FollowerUserDto>>.Fail($"User Not Found!", ResponseStatus.NotFound));
                }

                List<UserFollow> followers = _followRepository.GetListByExpressionPaginated(skip,take,f => f.TargetId == userId);

                List<_User> users = _userRepository.GetListByExpressionPaginated(skip,take, u => followers.Any(f => f.SourceId == u.Id));
                List<FollowerUserDto> followersDto = _mapper.Map<List<_User>, List<FollowerUserDto>>(users);

                return await Task.FromResult(Response<List<FollowerUserDto>>.Success(followersDto, ResponseStatus.Success));

            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<List<FollowerUserDto>>.Fail($"Some error occurred: {e}", ResponseStatus.InitialError));

            }
        }
}