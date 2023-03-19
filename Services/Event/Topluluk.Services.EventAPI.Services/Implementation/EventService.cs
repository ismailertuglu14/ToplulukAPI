using System;
using AutoMapper;
using Topluluk.Services.EventAPI.Model.Dto;
using Topluluk.Services.EventAPI.Services.Interface;
using Topluluk.Shared.Dtos;

using RestSharp;
using Topluluk.Services.EventAPI.Model.Dto.Http;
using Topluluk.Services.EventAPI.Model.Entity;
using Topluluk.Services.EventAPI.Data.Interface;

namespace Topluluk.Services.EventAPI.Services.Implementation
{
    public class EventService : IEventService
    {

        private readonly IMapper _mapper;
        private readonly RestClient _client;
        private readonly IEventRepository _eventRepository;
        public EventService(IEventRepository eventRepository, IMapper mapper)
        {
            _mapper = mapper;
            _client = new RestClient();
            _eventRepository = eventRepository;
        }

        public async Task<Response<string>> CreateEvent(CreateEventDto dto)
        {
            Event _event = new();

            if (dto.UserId == null || dto.UserId == String.Empty)
            {
                return await Task.FromResult(Response<string>.Fail($"Error occured: User ID cant be null", Shared.Enums.ResponseStatus.InitialError));

            }
            // check community is exist ;
            // check is user is participiant of community
            if (dto.CommunityId != null && dto.CommunityId != String.Empty)
            {
                
                var getParticipiantsRequest = new RestRequest($"https://localhost:7132/Community/Participiants/{dto.CommunityId}");
                var getParticipiantsResponse = await _client.ExecuteGetAsync<List<string>>(getParticipiantsRequest);

                if (getParticipiantsResponse.IsSuccessful == false || !getParticipiantsResponse.Data.Contains(dto.UserId))
                {
                    return await Task.FromResult(Response<string>.Fail($"Not participiant of {dto.CommunityId} Community", Shared.Enums.ResponseStatus.BadRequest));
                }

            }



            try
            {
                // Get User's FirstName lastname and UserImage
                //var userInfoRequest = new RestRequest("https://localhost:7202/User/GetUserById").AddQueryParameter("userId", dto.UserId);
                //var userInfoResponse = await _client.ExecuteGetAsync<Response<GetUserInfoDto>>(userInfoRequest);
                _event.CommunityId = dto.CommunityId;
                _event.UserId = dto.UserId!;
                _event.IsLimited = dto.IsLimited ?? false;
                _event.Location = dto.Location ?? "";
                _event.ParticipiantLimit = dto.AttendeesLimit ?? 0;
                _event.Title = dto.Title;
                _event.StartDate = dto.StartDate;
                _event.EndDate = dto.EndDate;
                _event.Description = dto.Description;

                DatabaseResponse response = await _eventRepository.InsertAsync(_event);

                if (response.IsSuccess != true)
                {
                    return await Task.FromResult(Response<string>.Fail("Failed while insterting entity of event", Shared.Enums.ResponseStatus.InitialError));

                }

                return await Task.FromResult(Response<string>.Success("Success", Shared.Enums.ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Error occured {e}", Shared.Enums.ResponseStatus.InitialError));

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
                    return await Task.FromResult(Response<string>.Success("Deleted Completely", Shared.Enums.ResponseStatus.Success));
                }
                else
                {
                    return await Task.FromResult(Response<string>.Fail("This event not belongs to you!", Shared.Enums.ResponseStatus.NotAuthenticated));

                }
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Some error occured: {e}", Shared.Enums.ResponseStatus.InitialError));
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
                    _eventRepository.DeleteById(id);
                    return await Task.FromResult(Response<string>.Success("Deleted", Shared.Enums.ResponseStatus.Success));
                }
                else
                {
                    return await Task.FromResult(Response<string>.Fail("This event not belongs to you!", Shared.Enums.ResponseStatus.NotAuthenticated));

                }
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Some error occured: {e}", Shared.Enums.ResponseStatus.InitialError));
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
                    if (response.IsSuccess == true)
                    {
                        return await Task.FromResult(Response<string>.Success("Event expired", Shared.Enums.ResponseStatus.Success));
                    }
                    else
                    {
                        return await Task.FromResult(Response<string>.Fail("Failed event expire", Shared.Enums.ResponseStatus.InitialError));

                    }
                }
                else
                {
                    return await Task.FromResult(Response<string>.Fail("This event not belongs to you!", Shared.Enums.ResponseStatus.NotAuthenticated));
                }

            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Some error occured: {e}", Shared.Enums.ResponseStatus.InitialError));
            }
        }

        public Task<Response<string>> GetEventById(string userId, string id)
        {
            throw new NotImplementedException();
        }

        public Task<Response<string>> GetEventSuggestions()
        {
            throw new NotImplementedException();
        }

        public async Task<Response<List<FeedEventDto>>> GetUserEvents(string id)
        {

            try
            {
                DatabaseResponse response = await _eventRepository.GetAllAsync(5, 0, e => e.UserId == id);
                if (response.IsSuccess == true)
                {
                    List<FeedEventDto> dto = _mapper.Map<List<Event>, List<FeedEventDto>>(response.Data);
                    
                    int i = 0;
                    foreach(var e in dto)
                    {
                        e.EventId = response.Data[i].Id;
                    }
                    return await Task.FromResult(Response<List<FeedEventDto>>.Success(dto, Shared.Enums.ResponseStatus.Success));

                }
                return await Task.FromResult(Response<List<FeedEventDto>>.Fail("Some error occured", Shared.Enums.ResponseStatus.InitialError));

            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<List<FeedEventDto>>.Fail($"Some error occured: {e}", Shared.Enums.ResponseStatus.InitialError));
            }

            }
        }
}

