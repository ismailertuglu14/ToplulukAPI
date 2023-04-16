using AutoMapper;
using RestSharp;
using Topluluk.Services.EventAPI.Data.Interface;
using Topluluk.Services.EventAPI.Model.Dto;
using Topluluk.Services.EventAPI.Model.Dto.Http;
using Topluluk.Services.EventAPI.Model.Entity;
using Topluluk.Services.EventAPI.Services.Interface;
using Topluluk.Shared.Dtos;
using ResponseStatus = Topluluk.Shared.Enums.ResponseStatus;

namespace Topluluk.Services.EventAPI.Services.Implementation;

public class EventCommentService : IEventCommentService
{
    
    private readonly IMapper _mapper;
    private readonly RestClient _client;
    private readonly IEventCommentRepository _commentRepository;

    public EventCommentService( IMapper mapper, IEventCommentRepository commentRepository)
    {
        _mapper = mapper;
        _client = new RestClient();
        _commentRepository = commentRepository;
    }


    
    public async Task<Response<string>> CreateComment(string userId, CommentCreateDto dto)
    {
        try
        {
            EventComment comment = _mapper.Map<EventComment>(dto);
            comment.UserId = userId;
            comment.EventId = dto.EventId;
            DatabaseResponse response = await _commentRepository.InsertAsync(comment);
            return await Task.FromResult(Response<string>.Success(response.Data, ResponseStatus.Success));
        }
        catch (Exception e)
        {
            return await Task.FromResult(Response<string>.Fail($"Error occured {e}", ResponseStatus.InitialError));
        }

    }

    public async Task<Response<NoContent>> DeleteComment(string id, string eventId)
    {
        try
        {
            EventComment comment = await _commentRepository.GetFirstAsync(e => e.Id == eventId);

            if (comment == null)
            {
                return await Task.FromResult(Response<NoContent>.Fail("", ResponseStatus.NotFound));
            }
            
            if (!id.Equals(comment.UserId))
            {   
                return await Task.FromResult(Response<NoContent>.Fail("Unatuhorized",
                ResponseStatus.Unauthorized));
            }
            
            _commentRepository.DeleteByExpression(c => c.UserId == comment.UserId);
            return await Task.FromResult(Response<NoContent>.Success(ResponseStatus.Success));
        }
        catch (Exception e)
        {
            return await Task.FromResult(Response<NoContent>.Fail($"Some error occurred: {e}",
                ResponseStatus.InitialError));
        }
    }

    public async Task<Response<List<GetEventCommentDto>>> GetEventComments(string userId, string id, int skip = 0, int take = 10)
    {
        try
        {
            DatabaseResponse response = await _commentRepository.GetAllAsync(take, skip, c => c.EventId == id);
            if (response.Data != null && response.Data.Count > 0)
            {
                List<GetEventCommentDto> dtos = _mapper.Map<List<EventComment>, List<GetEventCommentDto>>(response.Data);
                    
                foreach (var dto in dtos)
                {
                    var userInfoRequest = new RestRequest("https://localhost:7202/User/user-info-comment").AddQueryParameter("id",dto.UserId);
                    var userInfoResponse = await _client.ExecuteGetAsync<Response<GetUserInfoDto>>(userInfoRequest);
                    dto.FirstName = userInfoResponse.Data.Data.FirstName;
                    dto.LastName = userInfoResponse.Data.Data.LastName;
                    dto.ProfileImage = userInfoResponse.Data.Data.ProfileImage;
                    dto.Gender = userInfoResponse.Data.Data.Gender;
                        
                }
                return await Task.FromResult(Response<List<GetEventCommentDto>>.Success(dtos, ResponseStatus.Success));
            }
            return await Task.FromResult(Response<List<GetEventCommentDto>>.Success(null, ResponseStatus.Success));

        }
        catch (Exception e)
        {
            return await Task.FromResult(Response<List<GetEventCommentDto>>.Fail($"Some error occured: {e}", ResponseStatus.InitialError));
        }
    }
}