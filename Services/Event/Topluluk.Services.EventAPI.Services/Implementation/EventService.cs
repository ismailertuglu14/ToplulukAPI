using System.Net.Http.Headers;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RestSharp;
using Topluluk.Services.EventAPI.Data.Interface;
using Topluluk.Services.EventAPI.Model.Dto;
using Topluluk.Services.EventAPI.Model.Dto.Http;
using Topluluk.Services.EventAPI.Model.Entity;
using Topluluk.Services.EventAPI.Services.Interface;
using Topluluk.Shared.Constants;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Messages.Event;
using ResponseStatus = Topluluk.Shared.Enums.ResponseStatus;
using _MassTransit = MassTransit;

namespace Topluluk.Services.EventAPI.Services.Implementation
{
    public class EventService : IEventService
    {

        private readonly IMapper _mapper;
        private readonly RestClient _client;
        private readonly IEventRepository _eventRepository;
        private readonly IEventCommentRepository _commentRepository;
        private readonly IEventAttendeesRepository _attendeesRepository;
        private readonly _MassTransit.ISendEndpointProvider _endpointProvider;
        public EventService(_MassTransit.ISendEndpointProvider endpointProvider, IEventRepository eventRepository, IMapper mapper, IEventCommentRepository commentRepository, IEventAttendeesRepository attendeesRepository)
        {
            _endpointProvider = endpointProvider;
            _mapper = mapper;
            _client = new RestClient();
            _eventRepository = eventRepository;
            _commentRepository = commentRepository;
            _attendeesRepository = attendeesRepository;
        }

       

        public async Task<Response<string>> CreateEvent(string userId, string token, CreateEventDto dto)
        {
            try
            {
                Event _event = _mapper.Map<Event>(dto);
                _event.UserId = userId;
                var responseUrls = new Response<List<string>>();
                
                if (userId.IsNullOrEmpty())
                {
                    return await Task.FromResult(Response<string>.Fail("Error occured: User ID cant be null", ResponseStatus.InitialError));

                }

                var userExistRequest =
                    new RestRequest(ServiceConstants.API_GATEWAY + "/User/GetUserById")
                        .AddHeader("Authorization",token).AddQueryParameter("userid", userId);
                var userExistResponse = await _client.ExecuteGetAsync<Response<GetUserInfoDto>>(userExistRequest);
                
                if (userExistResponse.Data?.Data == null)
                {
                    return await Task.FromResult(Response<string>.Fail("UnAuthorized", ResponseStatus.Unauthorized));
                }
                
                
                // check community is exist ;
                if (!dto.CommunityId.IsNullOrEmpty())
                {
                
                    var getParticipiantsRequest = new RestRequest($"https://localhost:7132/Community/Participiants/{dto.CommunityId}");
                    var getParticipiantsResponse = await _client.ExecuteGetAsync<List<string>>(getParticipiantsRequest);

                    if (getParticipiantsResponse.IsSuccessful == false || !getParticipiantsResponse.Data.Contains(userId))
                    {
                        return await Task.FromResult(Response<string>.Fail($"Not participiant of {dto.CommunityId} Community", ResponseStatus.BadRequest));
                    }

                }
                


                if (dto.Files != null && dto.Files.Count > 0)
                {
                    var client = new HttpClient();
                    var formDataContent = new MultipartFormDataContent();
                    
                    foreach (var i in dto.Files)
                    {
                        formDataContent.Add(new StreamContent(i.OpenReadStream())
                        {
                            Headers =
                            {
                                ContentLength = i.Length,
                                ContentType = new MediaTypeHeaderValue(i.ContentType)
                            }
                        },"files",i.FileName);
                    }
                    var responseFiles =
                        await client.PostAsync(ServiceConstants.API_GATEWAY + "/file/event-images", formDataContent);
                    if (responseFiles.IsSuccessStatusCode)
                    {
                        string responseString = await responseFiles.Content.ReadAsStringAsync();

                        responseUrls = JsonConvert.DeserializeObject<Response<List<string>>>(responseString);
                        _event.Images!.AddRange(responseUrls.Data);

                    }
                }

                
                if (_event.IsLocationOnline)
                {
                    _event.LocationURL = dto.Location;
                    _event.LocationPlace = null;
                }
                else
                {
                    _event.LocationURL = null;
                    _event.LocationPlace = dto.Location;
                }

               
                DatabaseResponse response = await _eventRepository.InsertAsync(_event);

                if (response.IsSuccess != true)
                {
                    return await Task.FromResult(Response<string>.Fail("Failed while insterting entity of event", ResponseStatus.InitialError));

                }

                EventAttendee attendee = new();
                attendee.UserId = userId;
                attendee.EventId = response.Data;
                await _attendeesRepository.InsertAsync(attendee);
                return await Task.FromResult(Response<string>.Success(_event.Id, ResponseStatus.Success));

            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Error occured {e}", ResponseStatus.InitialError));

            }
            
        }


        public async Task<Response<string>> DeleteCompletelyEvent(string userId, string id)
        {

            try
            {
                Event _event = await _eventRepository.GetFirstAsync(e => e.Id == id);
                if (_event.UserId == userId)
                {
                    _eventRepository.DeleteCompletely(id);
                    return await Task.FromResult(Response<string>.Success("Deleted Completely", ResponseStatus.Success));
                }

                return await Task.FromResult(Response<string>.Fail("This event not belongs to you!", ResponseStatus.NotAuthenticated));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Some error occured: {e}", ResponseStatus.InitialError));
            }
        }

        public async Task<Response<string>> DeleteEvent(string userId, string id)
        {
            try
            {
                Event _event = await _eventRepository.GetFirstAsync(e => e.Id == id);
               
                if (_event == null) throw new Exception("Not Found"); 
                if (_event.UserId == userId)
                {
                    List<string> usernames = new List<string>();
                    List<string> emails = new List<string>();
                    IdList list = new();

                    var attendees = _attendeesRepository.GetListByExpression(a => a.EventId == id);
                    foreach (var attendee in attendees)
                    {
                        list.ids.Add(attendee.UserId);
                    }
                
                    var request = new RestRequest(ServiceConstants.API_GATEWAY + "/user/get-user-info-list").AddBody(list);
                    var response = await _client.ExecutePostAsync<Response<List<UserEventDeletedDto>>>(request);
                    foreach (var user in response.Data.Data)
                    {
                        usernames.Add(user.FirstName+" "+user.LastName);
                        emails.Add(user.Email);
                    }
                    _eventRepository.DeleteById(id);
                    /*
                    var sendEndpoint = await _endpointProvider.GetSendEndpoint(new Uri("queue:event-deleted"));
                    var registerMessage = new EventDeletedCommand()
                    {
                        EventName = _event.Title,
                        UserNames = usernames,
                        UserMails = emails,
                    }; 
                     await sendEndpoint.Send<EventDeletedCommand>(registerMessage);
                    */
                    return await Task.FromResult(Response<string>.Success("Deleted", ResponseStatus.Success));
                }

                return await Task.FromResult(Response<string>.Fail("This event not belongs to you!", ResponseStatus.NotAuthenticated));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Some error occured: {e}", ResponseStatus.InitialError));
            }
        }

        public async Task<Response<string>> JoinEvent(string userId, string eventId)
        {
            try
            {
                Event _event = await _eventRepository.GetFirstAsync(e => e.Id == eventId);
                if (_event == null) 
                    return Response<string>.Fail("User Not Found",ResponseStatus.NotFound);
                if(_event.IsExpired)
                    return Response<string>.Fail("You can not join because event expired", ResponseStatus.Failed);
               
                var attendeesCountTask = _attendeesRepository.Count(e => e.EventId == _event.Id);
                var isParticipiantTask = _attendeesRepository.AnyAsync(e => e.IsDeleted == false && e.EventId == _event.Id && e.UserId == userId);
                await Task.WhenAll(attendeesCountTask, isParticipiantTask);
                
                if (_event.IsLimited && attendeesCountTask.Result >= _event.ParticipiantLimit)
                {
                    return Response<string>.Fail("Event is full now!", ResponseStatus.BadRequest);
                }

                if (isParticipiantTask.Result)
                {
                    return Response<string>.Success("Already joined", ResponseStatus.Success);
                }

                EventAttendee attendee = new()
                {
                    UserId = userId,
                    EventId = eventId
                };
                await _attendeesRepository.InsertAsync(attendee);

                return Response<string>.Success("Joined", ResponseStatus.Success);

            }
            catch (Exception e)
            {
                
                return await Task.FromResult(Response<string>.Fail($"Some error occured: {e}", ResponseStatus.InitialError));
            }
        }

        public async Task<Response<NoContent>> LeaveEvent(string userId, string eventId)
        {
            try
            {
                Event _event = await _eventRepository.GetFirstAsync(e => e.Id == eventId);
                // If user owner the event
                if (_event.UserId == userId)
                {
                    return await Task.FromResult(Response<NoContent>.Fail("Owner cant left event",
                        ResponseStatus.Failed));
                }
                
                EventAttendee? attendee = await _attendeesRepository.GetFirstAsync(e => e.EventId == eventId && e.UserId == userId);
                if (attendee == null)
                {
                    return await Task.FromResult(Response<NoContent>.Fail("Event Not found", ResponseStatus.NotFound));
                }
                
                _attendeesRepository.DeleteById(attendee);
                return await Task.FromResult(Response<NoContent>.Success(ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<NoContent>.Fail($"Some Error occurred: {e}", ResponseStatus.InitialError));
            }
        }

        public async Task<Response<string>> ExpireEvent(string userId, string id)
        {
            try
            {
                Event _event = await _eventRepository.GetFirstAsync(e => e.Id == id);
                if (_event == null) throw new Exception("Not Found");
                if (_event.UserId == userId)
                {
                    _event.IsExpired = true;
                    DatabaseResponse response = _eventRepository.Update(_event);
                    if (response.IsSuccess)
                    {
                        return await Task.FromResult(Response<string>.Success("Event expired", ResponseStatus.Success));
                    }

                    return await Task.FromResult(Response<string>.Fail("Failed event expire", ResponseStatus.InitialError));
                }

                return await Task.FromResult(Response<string>.Fail("This event not belongs to you!", ResponseStatus.NotAuthenticated));

            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Some error occured: {e}", ResponseStatus.InitialError));
            }
        }

        public async Task<Response<EventDto>> GetEventById(string userId, string token, string id)
        {
            try
            {
                Event _event = await _eventRepository.GetFirstAsync(e => e.Id == id);
                
                if (_event == null)
                {
                    return Response<EventDto>.Fail("Not Found", ResponseStatus.NotFound);
                }
                 EventDto dto = _mapper.Map<EventDto>(_event);
                  var eventOwnerRequest = new RestRequest(ServiceConstants.API_GATEWAY + "/user/GetUserById")
                        .AddQueryParameter("userId",_event.UserId)
                        .AddHeader("Authorization",token);
                    var eventOwnerResponseTask = _client.ExecuteGetAsync<Response<GetUserInfoDto>>(eventOwnerRequest);
                    
                    var commentCountTask =  _commentRepository.Count(c => !c.IsDeleted && c.EventId == id);
                    var commentsTask = _commentRepository.GetAllAsync(10, 0, c => c.EventId == id);
                    var isAttendeedTask =  _attendeesRepository.AnyAsync(c => !c.IsDeleted && c.UserId == userId && c.EventId == _event.Id);
                    var attendeesCountTask =  _attendeesRepository.Count(a => !a.IsDeleted && a.EventId == id);
                    
                    var idList = new IdList() { ids = (commentsTask.Result.Data as List<EventComment>).Select(c => c.UserId).ToList() };
                    var userInfosReqeust = new RestRequest(ServiceConstants.API_GATEWAY + "/user/get-user-info-list").AddBody(idList);
                    var userInfosResponseTask =  _client.ExecutePostAsync<Response<List<GetUserInfoDto>>>(userInfosReqeust);

                    await Task.WhenAll(userInfosResponseTask, eventOwnerResponseTask, commentCountTask,commentsTask, isAttendeedTask, attendeesCountTask);
                   
                    if (commentsTask.Result.Data.Count > 0)
                    {
                      
                        for (int i = 0; i < commentsTask.Result.Data.Count; i++)
                        {
                            var response = userInfosResponseTask.Result;
                            GetUserInfoDto commentOwner = response.Data.Data.Where(u => u.Id == commentsTask.Result.Data[i].UserId)
                                .FirstOrDefault() ?? throw new InvalidOperationException();
                            GetEventCommentDto commentDto = new()
                            {
                                Id = commentsTask.Result.Data[i].EventId,
                                Message = commentsTask.Result.Data[i].Message,
                                ProfileImage = commentOwner.ProfileImage,
                                FirstName = commentOwner.FirstName,
                                LastName = commentOwner.LastName,
                                Gender = commentOwner.Gender,
                                UserId = commentOwner.Id,
                            };
                            dto.Comments?.Add(commentDto);
                        }
                    }

                    dto.CommentCount = commentCountTask.Result;
                    dto.IsAttendeed = isAttendeedTask.Result;
                    dto.AttendeesCount = attendeesCountTask.Result;
                    dto.Location = _event.IsLocationOnline ? _event.LocationURL : _event.LocationPlace;
                    var user = eventOwnerResponseTask.Result.Data.Data;
                    dto.UserId = user.Id;
                    dto.FirstName = user.FirstName;
                    dto.LastName = user.LastName;
                    dto.ProfileImage = user.ProfileImage; 
                    dto.Gender = user.Gender;
                    
                return Response<EventDto>.Success(dto, ResponseStatus.Success);
                
            }
            catch (Exception e)
            {
                return Response<EventDto>.Fail($"Some error occured: {e}", ResponseStatus.InitialError);
            }
        }


        

        public async Task<Response<List<GetEventAttendeesDto>>> GetEventAttendees(string token, string eventId,
            int skip = 0, int take = 10)
        {
            try
            {
                Event _event = await _eventRepository.GetFirstAsync(e => e.Id == eventId);
                if (_event == null) throw new Exception("Event Not found");

                var attendees = _attendeesRepository.GetListByExpressionPaginated(skip,take, e => e.EventId == eventId);
                
                IdList idList = new() { ids = attendees.Select(a => a.UserId).ToList() };
                
                var usersRequest = new RestRequest(ServiceConstants.API_GATEWAY+"/user/get-user-info-list")
                    .AddHeader("Authorization",token).AddBody(idList);
                var usersResponse = await _client.ExecutePostAsync<Response<List<GetUserInfoDto>>>(usersRequest);
                
                List<GetEventAttendeesDto> dtos = _mapper.Map<List<EventAttendee>, List<GetEventAttendeesDto>>(attendees);

                foreach (var dto in dtos)
                {
                    var user = usersResponse.Data.Data.FirstOrDefault(u =>u.Id == dto.Id);
                    dto.Id = user.Id;
                    dto.FirstName = user.FirstName;
                    dto.LastName = user.LastName;
                    dto.ProfileImage = user.ProfileImage;
                    dto.Gender = user.Gender;
                }
                
                return Response<List<GetEventAttendeesDto>>.Success(dtos, ResponseStatus.Success);
            }
            catch (Exception e)
            {
                return Response<List<GetEventAttendeesDto>>.Fail(e.ToString(), ResponseStatus.InitialError);
            }
        }

        public Task<Response<string>> GetEventSuggestions()
        {
            throw new NotImplementedException();
        }

        public async Task<Response<List<EventDto>>> GetUserEvents(string id, string token)
        {

            try
            {
                var events =  _eventRepository.GetListByExpressionPaginated(10, 0, e => e.UserId == id);
                
                var eventOwnerRequest = new RestRequest(ServiceConstants.API_GATEWAY + "/user/GetUserById")
                    .AddQueryParameter("userId",id)
                    .AddHeader("Authorization",token);
                var eventOwnerResponse = await _client.ExecuteGetAsync<Response<GetUserInfoDto>>(eventOwnerRequest);

                var dtos = _mapper.Map<List<Event>, List<EventDto>>(events);
                var eventIds = dtos.Select(e => e.Id).ToList();
                var eventsComments = await _commentRepository.GetEventCommentCounts(eventIds);
                
                var user = eventOwnerResponse.Data.Data;
                
                byte i = 0;
                foreach (var dto in dtos)
                {
                    var commentsTask = _commentRepository.GetAllAsync(10, 0, c => c.EventId == dto.Id);
                    var isAttendeedTask =  _attendeesRepository.AnyAsync(c => !c.IsDeleted && c.UserId == id && c.EventId == dto.Id);
                    var attendeesCountTask =  _attendeesRepository.Count(a => !a.IsDeleted && a.EventId == dto.Id);
                    await Task.WhenAll(commentsTask, isAttendeedTask, attendeesCountTask);

                    dto.CommentCount = eventsComments.FirstOrDefault(c => c.Key == dto.Id).Value;
                    var comments = commentsTask.Result.Data as List<EventComment>; 
                    dto.Comments = _mapper.Map<List<EventComment>,List<GetEventCommentDto>>(comments);
                    dto.IsAttendeed = isAttendeedTask.Result;
                    dto.AttendeesCount = attendeesCountTask.Result;
                    dto.Location = events[i].IsLocationOnline ? events[i].LocationURL : events[i].LocationPlace;

                    
                    dto.UserId = user.Id;
                    dto.FirstName = user.FirstName;
                    dto.LastName = user.LastName;
                    dto.ProfileImage = user.ProfileImage;
                    dto.Gender = user.Gender;
                    i++;
                }
                
                
                return await Task.FromResult(Response<List<EventDto>>.Success(dtos, ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<List<EventDto>>.Fail($"Some error occured: {e}", ResponseStatus.InitialError));
            }

        }
    }
}

