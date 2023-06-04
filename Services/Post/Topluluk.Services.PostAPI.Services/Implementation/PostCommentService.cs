using AutoMapper;
using RestSharp;
using Topluluk.Services.PostAPI.Data.Interface;
using Topluluk.Services.PostAPI.Model.Dto;
using Topluluk.Services.PostAPI.Model.Dto.Http;
using Topluluk.Services.PostAPI.Model.Entity;
using Topluluk.Services.PostAPI.Services.Interface;
using Topluluk.Shared.Constants;
using Topluluk.Shared.Dtos;
using ResponseStatus = Topluluk.Shared.Enums.ResponseStatus;

namespace Topluluk.Services.PostAPI.Services.Implementation;

public class PostCommentService : IPostCommentService
{
    private readonly IMapper _mapper;
    private readonly IPostRepository _postRepository;
    private readonly IPostCommentRepository _commentRepository;
    private readonly RestClient _client;
    
    public PostCommentService(IMapper mapper, IPostRepository postRepository, IPostCommentRepository commentRepository)
    {
        _mapper = mapper;
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _client = new RestClient();
    }

    public async Task<Response<List<CommentGetDto>>> GetComments(string userId, string postId, int take = 10, int skip = 0)
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
            
        return Response<List<CommentGetDto>>.Success(comments,ResponseStatus.Success);
    }

    public async Task<Response<NoContent>> CreateComment(CommentCreateDto commentDto)
    {
        PostComment comment = _mapper.Map<PostComment>(commentDto);

        Post? post = await _postRepository.GetFirstAsync(p => p.Id == commentDto.PostId);

        if (post != null)
        {
            await _commentRepository.InsertAsync(comment);
            return Response<NoContent>.Success( ResponseStatus.Success);
        }

        return Response<NoContent>.Fail("Post not found", ResponseStatus.NotFound);

    }

    public async Task<Response<NoContent>> UpdateComment(string userId, CommentUpdateDto commentDto)
    {
        try
        {
            
            PostComment? comment = await _commentRepository.GetFirstAsync(c => c.Id == commentDto.CommentId && c.UserId == userId );
            
            if (comment == null)
                return Response<NoContent>.Fail("Not Found", ResponseStatus.NotFound);

            comment.Message = commentDto.Message;

            _commentRepository.Update(comment);
            
            return Response<NoContent>.Success(ResponseStatus.Success);
        }
        catch (Exception e)
        {
            return Response<NoContent>.Fail(e.ToString(), ResponseStatus.InitialError);
        }
    }

    public async Task<Response<NoContent>> DeleteComment(string userId, string commentId)
    {
        try
        {
            PostComment commemt = await _commentRepository.GetFirstAsync(c => c.Id == commentId);

            if (commemt.UserId != userId)
                return Response<NoContent>.Fail("UnAauthorized", ResponseStatus.NotAuthenticated);
            
            _commentRepository.DeleteById(commentId);
            return Response<NoContent>.Success(ResponseStatus.Success);

        }
        catch (Exception e)
        {
            return Response<NoContent>.Fail(e.ToString(), ResponseStatus.InitialError);

        }
    }
}