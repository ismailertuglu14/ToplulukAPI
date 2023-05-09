using AutoMapper;
using Topluluk.Services.PostAPI.Data.Interface;
using Topluluk.Services.PostAPI.Model.Dto;
using Topluluk.Services.PostAPI.Model.Entity;
using Topluluk.Services.PostAPI.Services.Interface;
using Topluluk.Shared.Dtos;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Topluluk.Shared.Enums;
using Topluluk.Services.PostAPI.Model.Dto.Http;
using RestSharp;
using Topluluk.Services.POSTAPI.Model.Dto.Http;
using Microsoft.IdentityModel.Tokens;
using Topluluk.Shared.Constants;
using ResponseStatus = Topluluk.Shared.Enums.ResponseStatus;


namespace Topluluk.Services.PostAPI.Services.Implementation
{
	public class PostService : IPostService
	{

        private readonly IPostRepository _postRepository;
        private readonly ISavedPostRepository _savedPostRepository;
        private readonly IPostInteractionRepository _postInteractionRepository;
        private readonly IPostCommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private readonly RestClient _client;

        public PostService(IPostRepository postRepository, IPostInteractionRepository postInteractionRepository, ISavedPostRepository savedPostRepository, IPostCommentRepository commentRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _savedPostRepository = savedPostRepository;
            _postInteractionRepository = postInteractionRepository;
            _mapper = mapper;
            _commentRepository = commentRepository;
            _client = new RestClient();
        }


        public async Task<Response<List<GetPostInteractionDto>>> GetInteractions(string userId, string postId, int take = 10, int skip = 0)
        {
            try
            {
                Post? post = await _postRepository.GetFirstAsync(p => p.Id == postId);
                if (post == null) return Response<List<GetPostInteractionDto>>.Fail("", ResponseStatus.NotFound);
                
                List<PostInteraction> interactions =
                    _postInteractionRepository.GetListByExpressionPaginated(skip, take, i => i.PostId == postId);
                IdList idList = new() { ids = interactions.Select(i => i.UserId).ToList() };

                var userRequest = new RestRequest(ServiceConstants.API_GATEWAY + "/user/get-user-info-list").AddBody(idList);
                var userResponse = await _client.ExecutePostAsync<Response<List<UserInfoDto>>>(userRequest);
                
                var interactionDtos = _mapper.Map<List<PostInteraction>, List<GetPostInteractionDto>>(interactions);
                
                if (userResponse.Data != null)
                {
                    for (int i = 0; i < interactions.Count; i++)
                    {
                        var user = userResponse.Data.Data.FirstOrDefault(u => u.Id == interactionDtos[i].UserId);
                        if (user == null)
                        {
                            interactionDtos.Remove(interactionDtos[i]);
                            continue;
                        }

                        interactionDtos[i].UserId = user.Id;
                        interactionDtos[i].FirstName = user.FirstName;
                        interactionDtos[i].LastName= user.LastName;
                        interactionDtos[i].ProfileImage = user.ProfileImage;
                        interactionDtos[i].Gender = user.Gender;
                    }
                }
                
                
                return Response<List<GetPostInteractionDto>>.Success(interactionDtos,ResponseStatus.Success);
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<List<GetPostInteractionDto>>.Fail($"Some error occurred: {e}",
                    ResponseStatus.InitialError));
            }
        }

        public async Task<Response<string>> RemoveInteraction(string userId, string postId)
        {
            try
            {
                PostInteraction? _interaction =
                    await _postInteractionRepository.GetFirstAsync(pi => pi.PostId == postId);
                
                if (_interaction == null) throw new Exception("Not found");
                if (_interaction.UserId == userId)
                {
                    _postInteractionRepository.DeleteCompletely(_interaction.Id);
                    return await Task.FromResult(Response<string>.Success("Success", ResponseStatus.Success));
                }
                else
                {
                    return await Task.FromResult(Response<string>.Fail("Not found", ResponseStatus.NotFound));
                }
                

            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Some error occurred: {e}",
                    ResponseStatus.InitialError));
            }
        }

        public async Task<Response<string>> SavePost(string userId, string postId)
        {
            try
            {
                if (userId.IsNullOrEmpty()) throw new Exception("User Not Found");
                SavedPost? _savedPost = await _savedPostRepository.GetFirstAsync(sp => sp.PostId == postId);

                if (_savedPost == null)
                {
                    SavedPost savedPost = new SavedPost
                    {
                        PostId = postId,
                        UserId = userId
                    };

                    DatabaseResponse response = await _savedPostRepository.InsertAsync(savedPost);

                    if (response.IsSuccess == false) throw new Exception("Some error occurred");

                    return await Task.FromResult(Response<string>.Success("Success", ResponseStatus.Success));
                }
                else
                {
                    _savedPostRepository.Delete(_savedPost.Id);
                    return await Task.FromResult(Response<string>.Success("Success", ResponseStatus.Success));
                }
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Some error occurred: {e}",
                    ResponseStatus.InitialError));
            }
        }

        public async Task<Response<string>> Comment(CommentCreateDto commentDto)
        {
            PostComment comment = _mapper.Map<PostComment>(commentDto);

            Post? post = await _postRepository.GetFirstAsync(p => p.Id == commentDto.PostId);

            if (post != null)
            {
                await _commentRepository.InsertAsync(comment);
                return await Task.FromResult(Response<string>.Success("Success", Shared.Enums.ResponseStatus.Success));
            }

            return await Task.FromResult(Response<string>.Fail("Post not found", Shared.Enums.ResponseStatus.NotFound));
        }

        public async Task<Response<List<GetPostForFeedDto>>> GetPostForFeedScreen(string userId, string token,
            int skip = 0, int take = 10)
        {
            try
            {
                // Request 2: Get user followings
                var getUserFollowingsRequest =
                    new RestRequest(ServiceConstants.API_GATEWAY + "/user/user-followings").AddQueryParameter("id",
                        userId);
                var getUserFollowingsTask = _client.ExecuteGetAsync<Response<List<string>>>(getUserFollowingsRequest);

                // Request 3: Get users' info list
                var getUserFollowingsResponse = getUserFollowingsTask.Result;
                if (getUserFollowingsResponse.IsSuccessful == false)
                {
                    return await Task.FromResult(
                        Response<List<GetPostForFeedDto>>.Fail("Failed", ResponseStatus.Failed));
                }

                List<Post> response = await _postRepository.GetPostsWithDescending(skip, take,
                    p => p.IsDeleted == false && (getUserFollowingsResponse.Data.Data.Contains(p.UserId) || p.UserId == userId));
                
                IdList idList = new() { ids =  response.Select(p => p.UserId).ToList() };
                
                var usersRequest =
                    new RestRequest(ServiceConstants.API_GATEWAY + "/user/get-user-info-list").AddBody(idList);
                var usersTask = _client.ExecutePostAsync<Response<List<UserInfoDto>>>(usersRequest);
                
                await Task.WhenAll( getUserFollowingsTask, usersTask);

                List<GetPostForFeedDto> dtos = _mapper.Map<List<Post>, List<GetPostForFeedDto>>(response);
                var usersResponse = usersTask.Result;
                
                for (int i = 0; i < response.Count; i++)
                {
                    var user = usersResponse.Data.Data.Where(u => u.Id == dtos[i].UserId)
                        .FirstOrDefault();
                    if (user == null)
                    {
                        dtos.Remove(dtos[i]);
                        continue;
                    }
                    dtos[i].UserId = user.Id;
                    dtos[i].FirstName = user.FirstName;
                    dtos[i].LastName = user.LastName;
                    dtos[i].ProfileImage = user.ProfileImage;
                    dtos[i].Gender = user.Gender;
                    dtos[i].IsFollowing = getUserFollowingsResponse.Data.Data.Contains(user.Id);

                    var isUserInteracted = await _postInteractionRepository.GetFirstAsync(p => p.PostId == dtos[i].Id && p.UserId == userId);
                    if (isUserInteracted != null)
                    {
                        dtos[i].IsInteracted = new PostInteractedDto()
                        {
                            Interaction = isUserInteracted.InteractionType
                        };
                    }
                    List<PostInteraction> interactions =
                        _postInteractionRepository.GetListByExpression(p => p.PostId == dtos[i].Id);
                    if (interactions != null)
                    {
                        dtos[i].InteractionPreviews = interactions
                            .GroupBy(x => x.InteractionType)
                            .OrderByDescending(x => x.Count())
                            .Take(3)
                            .Select(x => new PostInteractionPreviewDto()
                            {
                                Interaction = x.Key,
                                InteractionCount = x.Count()
                            })
                            .ToList();
                    }
                    dtos[i].InteractionCount = await _postInteractionRepository.Count(p => p.PostId == dtos[i].Id);
                    
                    
                    
                    dtos[i].CommentCount =
                        await _commentRepository.Count(c => c.PostId == response[i].Id && c.IsDeleted == false);
                    dtos[i].IsSaved =
                        await _savedPostRepository.AnyAsync(sp => sp.PostId == response[i].Id && sp.UserId == userId);
                    if (!response[i].CommunityLink.IsNullOrEmpty())
                    {
                        // Get-community-title and image request
                        var communityInfoRequest =
                            new RestRequest("https://localhost:7149/api/community/community-info-post-link")
                                .AddQueryParameter("id", response[i].CommunityLink);
                        var communityInfoTask =
                            _client.ExecuteGetAsync<Response<CommunityInfoPostLinkDto>>(communityInfoRequest);
                    }
                }

                return await Task.FromResult(Response<List<GetPostForFeedDto>>.Success(dtos, ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<List<GetPostForFeedDto>>.Fail($"Some error occurred: {e}",
                    ResponseStatus.InitialError));
            }
        }

        public async Task<Response<string>> Create(string userId, CreatePostDto postDto)
        {

            try
            {
                if (userId.IsNullOrEmpty()) throw new Exception("User not found");


                Post post = _mapper.Map<Post>(postDto);
                DatabaseResponse response = new();

                post.UserId = userId;
                var isUserParticipiantRequest =
                    new RestRequest(ServiceConstants.API_GATEWAY + $"/Community/Participiants/{post.CommunityId}");
                var isUserParticipiantResponse =
                    await _client.ExecuteGetAsync<Response<List<string>>>(isUserParticipiantRequest);
                if (isUserParticipiantResponse.IsSuccessful && isUserParticipiantResponse.Data.Data.Contains(userId))
                {
                    post.CommunityId = postDto.CommunityId;
                }

                Response<List<string>>? responseData = new();
                using (var client = new HttpClient())
                {
                    if (postDto.Files != null)
                    {
                        var imageContent = new MultipartFormDataContent();
                        foreach (var postDtoFile in postDto.Files)
                        {
                            var stream = new MemoryStream();
                            postDtoFile.CopyTo(stream);
                            var fileContent = new ByteArrayContent(stream.ToArray());
                            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                            imageContent.Add(fileContent, "Files", postDtoFile.FileName);
                        }


                        var responseClient =
                            await client.PostAsync("https://localhost:7165/file/upload-post-files", imageContent);

                        if (responseClient.IsSuccessStatusCode)
                        {
                            responseData = await responseClient.Content.ReadFromJsonAsync<Response<List<string>>>();

                            if (responseData != null)
                            {
                                foreach (var _response in responseData.Data)
                                {
                                    post.Files.Add(new FileModel(_response));
                                }
                            }
                        }
                        else
                        {
                            return await Task.FromResult(Response<string>.Fail(
                                "Failed while uploading image with http client", ResponseStatus.InitialError));
                        }
                    }

                }
                response = await _postRepository.InsertAsync(post);

                return await Task.FromResult(Response<string>.Success(response.Data,
                    Shared.Enums.ResponseStatus.Success));
                
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Some error occurred: {e}",
                    ResponseStatus.InitialError));
            }

        }

        public async Task<Response<string>> Delete(PostDeleteDto postDto)
        {

            Post post = await _postRepository.GetFirstAsync(p => p.Id == postDto.PostId);
            if (post != null)
            {
                _postInteractionRepository.DeleteByExpression(p => p.PostId == post.Id);
                _commentRepository.DeleteByExpression(p =>p.PostId == post.Id);
                _savedPostRepository.DeleteByExpression(p => p.PostId == post.Id);
                _postRepository.DeleteById(post.Id);

                return await Task.FromResult(Response<string>.Success("Success", Shared.Enums.ResponseStatus.Success));
            }
            return await Task.FromResult(Response<string>.Fail("Failed", Shared.Enums.ResponseStatus.NotFound));
        }

        public async Task<Response<string>> DeleteComment(string userId, string commentId)
        {
            try
            {
                PostComment commemt = await _commentRepository.GetFirstAsync(c => c.Id == commentId);

                if (commemt.UserId == userId)
                {
                    _commentRepository.DeleteById(commentId);
                    return await Task.FromResult(Response<string>.Success("Successfully deleted", ResponseStatus.Success));
                }
                return await Task.FromResult(Response<string>.Fail("UnAauthorized", ResponseStatus.NotAuthenticated));

            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Error occured {e}", ResponseStatus.InitialError));

            }
        }

        public async Task<Response<List<CommentGetDto>?>> GetComments(string userId, string postId, int take = 10, int skip = 0)
        {
            try
            {
                DatabaseResponse response = await _commentRepository.GetAllAsync(take, skip, c => c.PostId == postId);
                byte i = 0;
                List<CommentGetDto> comments = _mapper.Map<List<PostComment>, List<CommentGetDto>>(response.Data);
                IdList userIdList = new IdList() { };
                foreach (var comment in comments)
                {
                    userIdList.ids.Add(comment.UserId);
                }
                var userInfoRequest = new RestRequest(ServiceConstants.API_GATEWAY+"/user/get-user-info-list").AddBody(userIdList);
                var userInfoResponse = await _client.ExecutePostAsync<Response<List<UserInfoDto>>>(userInfoRequest);
                
                
                foreach (var comment in comments)
                {
                    comment.UserId = userInfoResponse.Data.Data.Where(u => u.Id == comment.UserId).FirstOrDefault().Id;
                    comment.Gender = userInfoResponse.Data.Data.Where(u => u.Id == comment.UserId).FirstOrDefault().Gender;
                    comment.FirstName = userInfoResponse.Data.Data.Where(u => u.Id == comment.UserId).FirstOrDefault().FirstName;
                    comment.LastName = userInfoResponse.Data.Data.Where(u => u.Id == comment.UserId).FirstOrDefault().LastName;
                    comment.ProfileImage = userInfoResponse.Data.Data.Where(u => u.Id == comment.UserId).FirstOrDefault().ProfileImage;
                }
                return await Task.FromResult(Response<List<CommentGetDto>?>.Success(comments, Shared.Enums.ResponseStatus.Success));
            }
            catch (Exception e)
            {
             return await Task.FromResult(Response<List<CommentGetDto>?>.Fail($"Some error occured: {e}", Shared.Enums.ResponseStatus.InitialError));

            }
        }

        public async Task<Response<string>> GetCommunityPosts(string communityId, int skip = 0, int take = 10)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
             return await Task.FromResult(Response<string>.Fail($"Some error occured: {e}", Shared.Enums.ResponseStatus.InitialError));
            }

        }

        public async Task<Response<GetPostByIdDto>> GetPostById(string postId, string sourceUserId, bool isDeleted = false)
        {

            try
            {
                Post post = await _postRepository.GetFirstAsync(p => p.Id == postId);

                GetPostByIdDto postDto = _mapper.Map<GetPostByIdDto>(post);

             //   postDto.InteractionCount = post.Interactions.Count;

                var _comments = await _commentRepository.GetAllAsync(10, 0, c => c.PostId == postId);
                if (_comments.Data != null && _comments.Data.Count > 0)
                {
                    postDto.CommentCount = _comments.Data?.Count;
                    CommentGetDto commentDto = new();

                    List<CommentGetDto> commentDtos = _mapper.Map<List<PostComment>, List<CommentGetDto>>(_comments.Data);

                    var ids = commentDtos.Select(comment => comment.UserId).ToList();
                    var idList = new IdList { ids = ids };

                    var request = new RestRequest(ServiceConstants.API_GATEWAY + "/user/get-user-info-list").AddBody(idList);
                    var response = await _client.ExecutePostAsync<Response<List<UserInfoDto>>>(request);

                    for (int i = 0; i < _comments.Data.Count; i++)
                    {
                        UserInfoDto user = response.Data.Data.Where(u => u.Id == commentDtos[i].UserId)
                            .FirstOrDefault() ?? throw new InvalidOperationException();
                        commentDtos[i].ProfileImage = user.ProfileImage;
                        commentDtos[i].FirstName = user.FirstName;
                        commentDtos[i].LastName = user.LastName;
                        commentDtos[i].Gender = user.Gender;
                        postDto.Comments?.Add(commentDtos[i]);
                    }

                }


                if (post.CommunityId != null)
                {
                    // Get community title request
                    var communityGetTitleRequest = new RestRequest("https://localhost:7132/Community/getCommunityTitle").AddParameter("id", post.CommunityId);
                    var communityGetTitleResponse = await _client.ExecuteGetAsync<Response<string>>(communityGetTitleRequest);
                    var communityResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Response<string>>(communityGetTitleResponse.Content);
                    postDto.CommunityTitle = communityResponse?.Data;
                }

                var userInfoRequest = new RestRequest("https://localhost:7202/User/GetUserInfoForPost")
                    .AddParameter("id", post.UserId)
                    .AddParameter("sourceUserId", sourceUserId);
                var userInfoResponse = await _client.ExecuteGetAsync<Response<UserInfoGetResponse>>(userInfoRequest);

                postDto.UserId = userInfoResponse.Data.Data.UserId;
                postDto.FirstName = userInfoResponse.Data.Data.FirstName;
                postDto.LastName = userInfoResponse.Data.Data.LastName;
                postDto.IsUserFollowing = userInfoResponse.Data.Data.IsUserFollowing;
                postDto.ProfileImage = userInfoResponse.Data.Data.ProfileImage;
                postDto.Gender = userInfoResponse.Data.Data.Gender;
                postDto.UserName = userInfoResponse.Data.Data.UserName;




                return await Task.FromResult(Response<GetPostByIdDto>.Success(postDto, Shared.Enums.ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<GetPostByIdDto>.Fail($"Some error occurred: {e} ", ResponseStatus.InitialError));
            }
        }

        // Feed ekranındaki postlar için hazırlanmış metod.
        public async Task<Response<List<GetPostDto>>> GetPosts(string userId, int take = 10, int skip = 0)
        {
            // Communit servise istek atıp takip ettiğimiz kullanıcıların id listesini döndürcez.
            //var userCommunitiesRequest = new RestRequest("https://localhost:7132/community/user-communities").AddQueryParameter("id", userId);
            ////var userCommunitiesResponse = await _client.ExecuteGetAsync<Response<List<string>>>(userCommunitiesRequest);
            //List<string> userCommunities = new();

            //if (userCommunitiesResponse.IsSuccessful == true)
            //{
            //    foreach (var community in userCommunitiesResponse.Data.Data)
            //    {user
            //        userCommunities.Add(community);
            //    }
            //}


            // Kullanıcının takip ettiği kullanıcıların postları, kendi postları, kendi katıldığı topluluklarının postları
            var response = await _postRepository.GetAllAsync(take, skip, p => p.UserId == userId);
            List<GetPostDto> posts = _mapper.Map<List<Post>, List<GetPostDto>>(response.Data);


            return await Task.FromResult(Response<List<GetPostDto>>.Success(posts, Shared.Enums.ResponseStatus.Success));

        }

        // Kullanıcı ekranında kullanıcının paylaşımlarını listelemek için kullanılacak action
        public async Task<Response<List<GetPostForFeedDto>>> GetUserPosts(string userId, string id, int take = 10,
            int skip = 0)
        {
            try
            {
                DatabaseResponse response = await _postRepository.GetAllAsync(take, skip, p => p.UserId == id);
                    var getUserInfoRequest = new RestRequest("https://localhost:7149/api/user/GetUserInfoForPost")
                        .AddQueryParameter("id", id).AddQueryParameter("sourceUserId", userId);
                    var getUserInfoResponse =
                        await _client.ExecuteGetAsync<Response<UserInfoGetResponse>>(getUserInfoRequest);

                if (getUserInfoResponse.Data.Data == null)
                {
                    return await Task.FromResult(Response<List<GetPostForFeedDto>>.Fail("User Not Found", ResponseStatus.NotFound));
                }

                    List<GetPostForFeedDto> dtos = _mapper.Map<List<Post>, List<GetPostForFeedDto>>(response.Data);
                    var commentCounts = await _commentRepository.GetPostCommentCounts(dtos.Select(p => p.Id).ToList());
                    int i = 0;
                    foreach (var dto in response.Data as List<Post>)
                    {
                        dtos[i].UserId = getUserInfoResponse.Data.Data.UserId;
                        dtos[i].FirstName = getUserInfoResponse.Data.Data.FirstName;
                        dtos[i].LastName = getUserInfoResponse.Data.Data.LastName;
                        dtos[i].ProfileImage = getUserInfoResponse.Data.Data.ProfileImage;

                        dtos[i].CommentCount = commentCounts.FirstOrDefault(c => c.Key == dtos[i].Id).Value;
                        //dtos[i].IsSaved = await _savedPostRepository.AnyAsync(sp => sp.PostId == dto.Id && sp.UserId == userId);
    
                        if (!dto.CommunityLink.IsNullOrEmpty())
                        {
                            var communityInfoRequest =
                                new RestRequest("https://localhost:7149/api/community/community-info-post-link")
                                    .AddQueryParameter("id", dto.CommunityLink);
                            var communityInfoResponse =
                                await _client.ExecuteGetAsync<Response<CommunityInfoPostLinkDto>>(communityInfoRequest);

                            dtos[i].Community = new() { Id = communityInfoResponse.Data.Data.Id, CoverImage = communityInfoResponse.Data.Data.CoverImage ?? "", Title = communityInfoResponse.Data.Data.Title };
                        }
                        else if (!dto.EventLink.IsNullOrEmpty())
                        {
                            // Get-event-title and image request
                            dtos[i].Event = new() { Id = "test", CoverImage = "test", Title = "test" };
                        }

                        i++;
                    }
                    return await Task.FromResult(
                        Response<List<GetPostForFeedDto>>.Success(dtos, ResponseStatus.Success));
                

            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<List<GetPostForFeedDto>>.Fail($"Some error occured {e}",
                    ResponseStatus.InitialError));

            }
        }


        public async Task<Response<string>> Interaction(string userId,string postId, PostInteractionCreateDto interactionCreate)
        {

            try
            {
                Post? post = await _postRepository.GetFirstAsync(p => p.Id == postId);
                if (post == null) throw new Exception("Post not found");
                if (!Enum.IsDefined(typeof(InteractionEnum), interactionCreate.InteractionType))
                {
                    return await Task.FromResult(Response<string>.Fail("Invalid InteractionType value", ResponseStatus.BadRequest));
                }
                PostInteraction _interaction = new()
                {
                    PostId = postId,
                    UserId = userId,
                    InteractionType = interactionCreate.InteractionType
                };

                PostInteraction? _interactionDb = await _postInteractionRepository.GetFirstAsync(i => i.PostId == postId && i.UserId == userId);
                if (_interactionDb == null)
                { 
                    await _postInteractionRepository.InsertAsync(_interaction);
                    return await Task.FromResult(Response<string>.Success("Success", Shared.Enums.ResponseStatus.Success));
                }
                _postInteractionRepository.DeleteCompletely(_interactionDb.Id);
                await _postInteractionRepository.InsertAsync(_interaction);

                return await Task.FromResult(Response<string>.Success("Success", Shared.Enums.ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Some error occurred: {e}",
                    ResponseStatus.InitialError));
            }
        }
        
        public Task<Response<string>> Update()
        {
            throw new NotImplementedException();
        }

        public Task<Response<string>> UpdateComment(string userId, string commentId, string newComment)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<List<GetPostForFeedDto>>> GetSavedPosts(string userId, int take = 10, int skip = 0)
        {
            try
            {
                if (userId != null)
                {
                    DatabaseResponse response = await _savedPostRepository.GetAllAsync(take, skip, sp => sp.UserId == userId);
                    List<GetPostForFeedDto> dtos = new();
                    
                    List<string> PostIds = new List<string>();
                    PostIds = (response.Data as List<SavedPost>).Select(p => p.PostId).ToList();

                    DatabaseResponse response2 = await _postRepository.GetAllAsync(take, skip, p => PostIds.Contains(p.Id));
                    UserIdListDto userIds = new();
                    userIds.Ids = (response2.Data as List<Post>).Select(r => r.UserId).ToList(); 

                    
                    var getUserListRequest = new RestRequest($"https://localhost:7149/api/user/get-user-info-list")
                                                .AddQueryParameter("skip", skip)
                                                .AddQueryParameter("take", take)
                                                .AddBody(userIds);

                    var getUserListResponse = await _client.ExecutePostAsync<Response<List<GetUserByIdDto>>>(getUserListRequest);
                    
                    byte i = 0;
                    foreach (var dto in response.Data as List<SavedPost>)
                    {
                        Post post = await _postRepository.GetFirstAsync(p => p.Id == dto.PostId);
                        dtos.Add(_mapper.Map<GetPostForFeedDto>(post));
                       
                        dtos[i].FirstName = getUserListResponse.Data.Data.Where(u => u.Id == post.UserId).FirstOrDefault().FirstName;
                        dtos[i].LastName = getUserListResponse.Data.Data.Where(u => u.Id == post.UserId).FirstOrDefault().LastName;
                        dtos[i].ProfileImage = getUserListResponse.Data.Data.Where(u => u.Id == post.UserId).FirstOrDefault().ProfileImage;
                        dtos[i].IsSaved = true;
                        //dtos[i].IsFollowing = getUserListResponse.Data[i].;
                        i++;
                    }

                    return await Task.FromResult(Response<List<GetPostForFeedDto>>.Success(dtos,ResponseStatus.Success));
                }
                else
                {
                    return await Task.FromResult(Response<List<GetPostForFeedDto>>.Fail("User Not Found",ResponseStatus.NotFound));
                }
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<List<GetPostForFeedDto>>.Fail($"Some error occurred: {e}",
                    ResponseStatus.InitialError));
            }

         }

        public async Task<Response<bool>> DeletePosts(string userId)
        {
            try
            {
                if (!userId.IsNullOrEmpty())
                {
                    bool result = await _postRepository.DeletePosts(userId);

                    if (result == true)
                    {
                        bool result2 = await _commentRepository.DeletePostsComments(userId);
                        bool result3 = await _savedPostRepository.DeleteSavedPostsByUserId(userId);
                    }

                    return await Task.FromResult(Response<bool>.Success(result, ResponseStatus.Success));
                }
                else
                {
                    return await Task.FromResult(Response<bool>.Fail("Not authorized", ResponseStatus.BadRequest));
                }
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<bool>.Fail($"Some error occurred: {e}",
                    ResponseStatus.InitialError));
            }
        }
    }
}

