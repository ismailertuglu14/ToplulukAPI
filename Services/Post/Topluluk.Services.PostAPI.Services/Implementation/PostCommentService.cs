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
    private readonly ICommentInteractionRepository _commentInteractionRepository;
    public PostCommentService(IMapper mapper, IPostRepository postRepository, IPostCommentRepository commentRepository, ICommentInteractionRepository commentInteractionRepository)
    {
        _mapper = mapper;
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _commentInteractionRepository = commentInteractionRepository;
        _client = new RestClient();
    }

    public async Task<Response<List<CommentGetDto>>> GetComments(string userId, string postId, int skip = 0,int take = 10)
    {
        List<PostComment> response =  await _commentRepository.GetPostCommentsDescendingDate(skip, take, c => !c.IsDeleted && c.PostId == postId && (c.ParentCommentId == "" || c.ParentCommentId == null));
        
        List<CommentGetDto> comments = _mapper.Map<List<PostComment>, List<CommentGetDto>>(response);
        
        IdList userIdList = new ()
        {
            ids = comments.Select(c => c.UserId).ToList()
        };

        var userInfoRequest = new RestRequest(ServiceConstants.API_GATEWAY+"/user/get-user-info-list").AddBody(userIdList);
        var userInfoResponse = await _client.ExecutePostAsync<Response<List<UserInfoDto>>>(userInfoRequest);
        
        List<string> commentIds = comments.Select(c => c.Id).ToList();
        var commentReplyCountTask =  _commentRepository.GetCommentsReplyCounts(commentIds);
        var commentsInteractedTask = _commentInteractionRepository.GetCommentsIsInteracted(userId, commentIds);
        var commentsInteractionCountsTask = _commentInteractionRepository.GetCommentsInteractionCounts(commentIds);
        await Task.WhenAll(commentsInteractedTask, commentReplyCountTask, commentsInteractionCountsTask);
        foreach (var comment in comments)
        {
            var user = userInfoResponse.Data.Data.Where(u => u.Id == comment.UserId).FirstOrDefault();
            if (user == null)
            {
                comments.Remove(comment);
                continue;
            }
            comment.UserId = user.Id;
            comment.Gender = user.Gender;
            comment.FirstName = user.FirstName;
            comment.LastName = user.LastName;
            comment.ProfileImage = user.ProfileImage;
            comment.ReplyCount = commentReplyCountTask.Result.TryGetValue(comment.Id, out int value) ? value : 0;
            comment.IsInteracted = commentsInteractedTask.Result.TryGetValue(comment.Id, out var isInteracted)
                ? isInteracted
                : new CommentInteracted();
            comment.InteractionCounts = commentsInteractionCountsTask.Result.TryGetValue(comment.Id, out var counts)
                ? counts
                : new CommentLikes();
            
            comment.IsEdited = response.Where(c => c.Id == comment.Id).FirstOrDefault().PreviousMessages != null ;
        }
            
        return Response<List<CommentGetDto>>.Success(comments,ResponseStatus.Success);
    }

    public async Task<Response<List<CommentGetDto>>> GetReplies(string commentId, int skip = 0, int take = 10)
    {
        List<PostComment> response =  await _commentRepository.GetPostCommentsDescendingDate(skip, take, c => !c.IsDeleted && c.ParentCommentId == commentId );

        if (response == null || response.Count == 0)
        {
            return Response<List<CommentGetDto>>.Success(new List<CommentGetDto>(),ResponseStatus.Success);
        }
        
        
        List<CommentGetDto> comments = _mapper.Map<List<PostComment>, List<CommentGetDto>>(response);
        
        IdList userIdList = new ()
        {
            ids = comments.Select(c => c.UserId).ToList()
        };

        var userInfoRequest = new RestRequest(ServiceConstants.API_GATEWAY+"/user/get-user-info-list").AddBody(userIdList);
        var userInfoResponse = await _client.ExecutePostAsync<Response<List<UserInfoDto>>>(userInfoRequest);
        
        List<string> commentIds = comments.Select(c => c.Id).ToList();
        
        foreach (var comment in comments)
        {
            var user = userInfoResponse.Data.Data.Where(u => u.Id == comment.UserId).FirstOrDefault();
            if (user == null)
            {
                comments.Remove(comment);
                continue;
            }
            comment.UserId = user.Id;
            comment.Gender = user.Gender;
            comment.FirstName = user.FirstName;
            comment.LastName = user.LastName;
            comment.ProfileImage = user.ProfileImage;
                
            comment.IsEdited = response!.Where(c => c.Id == comment.Id).FirstOrDefault().PreviousMessages != null ;
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

        PostComment comment = await _commentRepository.GetFirstAsync(c => c.Id == commentId);

        if (comment.UserId != userId)
            return Response<NoContent>.Fail("UnAauthorized", ResponseStatus.NotAuthenticated);
            
        _commentRepository.DeleteById(commentId);
        return Response<NoContent>.Success(ResponseStatus.Success);
            
    }

    public async Task<Response<NoContent>> Interaction(string userId, string commentId, int type)
    {
        if (!Enum.IsDefined(typeof(CommentInteractionEnum), type))
            throw new ArgumentException();

        CommentInteraction commentInteraction = await _commentInteractionRepository.GetFirstAsync(c => c.CommentId == commentId);
        CommentInteraction interaction = new()
        {
            UserId = userId,
            CommentId = commentId,
            Type = (CommentInteractionType)type
        };
        if (commentInteraction != null)
        {
            _commentInteractionRepository.DeleteById(commentInteraction);
            await _commentInteractionRepository.InsertAsync(interaction);
        }

        else if (commentInteraction == null && !Enum.IsDefined(typeof(CommentInteractionEnum), type))
        {
            await _commentInteractionRepository.InsertAsync(interaction);
        }

        return Response<NoContent>.Success(ResponseStatus.Success);
    }

}