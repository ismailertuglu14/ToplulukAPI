using AutoMapper;
using RestSharp;
using Topluluk.Services.PostAPI.Data.Interface;
using Topluluk.Services.PostAPI.Model.Dto;
using Topluluk.Services.PostAPI.Model.Dto.Http;
using Topluluk.Services.PostAPI.Model.Entity;
using Topluluk.Services.PostAPI.Services.Interface;
using Topluluk.Shared.Constants;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Exceptions;
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
        var response = await _commentRepository.GetAllAsync(take, skip, c => c.PostId == postId);
        
        List<CommentGetDto> comments = _mapper.Map<List<PostComment>, List<CommentGetDto>>(response.Data);
        
        IdList userIdList = new ()
        {
            ids = comments.Select(c => c.UserId).ToList()
        };

        var userInfoRequest = new RestRequest(ServiceConstants.API_GATEWAY+"/user/get-user-info-list").AddBody(userIdList);
        var userInfoResponse = await _client.ExecutePostAsync<Response<List<UserInfoDto>>>(userInfoRequest);
                
                
        foreach (var comment in comments)
        {
            var user = userInfoResponse.Data.Data.Where(u => u.Id == comment.UserId).FirstOrDefault();
            comment.UserId = user.Id;
            comment.Gender = user.Gender;
            comment.FirstName = user.FirstName;
            comment.LastName = user.LastName;
            comment.ProfileImage = user.ProfileImage;
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
        PostComment? comment = await _commentRepository.GetFirstAsync(c => c.Id == commentDto.CommentId);

        if (comment == null)
            throw new NotFoundException("Comment Not Found");

        if (comment.UserId != userId)
            throw new UnauthorizedAccessException();
        
        var previousMessage = new PreviousMessage()
        {
            Message = comment.Message,
            EditedDate = DateTime.Now
        };
            
        if (comment.PreviousMessages == null)
        {
            comment.PreviousMessages = new List<PreviousMessage>();
        }
            
        comment.PreviousMessages.Add(previousMessage);
        comment.Message = commentDto.Message;
        _commentRepository.Update(comment);
            
        return Response<NoContent>.Success(ResponseStatus.Success);
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