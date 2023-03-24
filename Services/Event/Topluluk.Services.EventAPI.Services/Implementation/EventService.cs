﻿using System;
using AutoMapper;
using Topluluk.Services.EventAPI.Model.Dto;
using Topluluk.Services.EventAPI.Services.Interface;
using Topluluk.Shared.Dtos;

using RestSharp;
using Topluluk.Services.EventAPI.Model.Dto.Http;
using Topluluk.Services.EventAPI.Model.Entity;
using Topluluk.Services.EventAPI.Data.Interface;
using ResponseStatus = Topluluk.Shared.Enums.ResponseStatus;

namespace Topluluk.Services.EventAPI.Services.Implementation
{
    public class EventService : IEventService
    {

        private readonly IMapper _mapper;
        private readonly RestClient _client;
        private readonly IEventRepository _eventRepository;
        private readonly IEventCommentRepository _commentRepository;

        public EventService(IEventRepository eventRepository, IMapper mapper, IEventCommentRepository commentRepository)
        {
            _mapper = mapper;
            _client = new RestClient();
            _eventRepository = eventRepository;
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
                return await Task.FromResult(Response<string>.Success(response.Data, Shared.Enums.ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Error occured {e}", Shared.Enums.ResponseStatus.InitialError));
            }

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

                return await Task.FromResult(Response<string>.Success(_event.Id, Shared.Enums.ResponseStatus.Success));
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

        public async Task<Response<string>> JoinEvent(string userId, string eventId)
        {
            try
            {
                Event _event = await _eventRepository.GetFirstAsync(e => e.Id == eventId);
                if (_event == null) throw new Exception("Not Found");

                if (_event.IsLimited == true && _event.Attendees.Count <= _event.ParticipiantLimit && !_event.Attendees.Contains(userId))
                {
                    _event.Attendees.Add(userId);
                    _eventRepository.Update(_event);
                    return await Task.FromResult(Response<string>.Success("Joined", Shared.Enums.ResponseStatus.Success));
                }
                else if (_event.IsLimited == false && !_event.Attendees.Contains(userId))
                {
                    _event.Attendees.Add(userId);
                    _eventRepository.Update(_event);
                    return await Task.FromResult(Response<string>.Success("Joined", Shared.Enums.ResponseStatus.Success));
                }
                else if(_event.IsLimited == true && _event.Attendees.Count >= _event.ParticipiantLimit)
                {
                    return await Task.FromResult(Response<string>.Fail("Event is full now!", Shared.Enums.ResponseStatus.BadRequest));
                }
                else if (_event.Attendees.Contains(userId))
                {
                    return await Task.FromResult(Response<string>.Success("Already joined", Shared.Enums.ResponseStatus.Success));

                }
                return await Task.FromResult(Response<string>.Fail("Error occured but why :)", Shared.Enums.ResponseStatus.InitialError));
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

        public async Task<Response<GetEventByIdDto>> GetEventById(string userId, string id)
        {
            try
            {
                Event _event = await _eventRepository.GetFirstAsync(e => e.Id == id);

                if (_event != null)
                {
                    GetEventByIdDto dto = _mapper.Map<GetEventByIdDto>(_event);
                    dto.CommentCount = await _commentRepository.Count(c => c.EventId == id);
                    return await Task.FromResult(Response<GetEventByIdDto>.Success(dto, Shared.Enums.ResponseStatus.Success));
                }

                return await Task.FromResult(Response<GetEventByIdDto>.Fail("Not Found", Shared.Enums.ResponseStatus.NotFound));               

            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<GetEventByIdDto>.Fail($"Some error occured: {e}", Shared.Enums.ResponseStatus.InitialError));
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
                    return await Task.FromResult(Response<List<GetEventCommentDto>>.Success(dtos, Shared.Enums.ResponseStatus.Success));
                }
                return await Task.FromResult(Response<List<GetEventCommentDto>>.Success(null, Shared.Enums.ResponseStatus.Success));

            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<List<GetEventCommentDto>>.Fail($"Some error occured: {e}", Shared.Enums.ResponseStatus.InitialError));
            }
        }

        public async Task<Response<List<GetEventAttendeesDto>>> GetEventAttendees(string userId, string eventId,
            int skip = 0, int take = 10)
        {
            try
            {
                Event _event = await _eventRepository.GetFirstAsync(e => e.Id == eventId);
                if (_event == null) throw new Exception("Event Not found");
                List<GetEventAttendeesDto> dto = new();
                foreach (var user in _event.Attendees)
                {
                    var userInfoRequest =
                        new RestRequest("https://localhost:7202/user/user-info-comment").AddQueryParameter("id", user);
                    var userInfoResponse = await _client.ExecuteGetAsync<Response<GetUserInfoDto>>(userInfoRequest);
                    dto.Add(new()
                        {Id = userInfoResponse.Data.Data.Id,FirstName = userInfoResponse.Data.Data.FirstName,
                            LastName = userInfoResponse.Data.Data.LastName,ProfileImage = userInfoResponse.Data.Data.ProfileImage, Gender = userInfoResponse.Data.Data.Gender});
                }


                return await Task.FromResult(
                    Response<List<GetEventAttendeesDto>>.Success(dto, ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<List<GetEventAttendeesDto>>.Fail($"Some error occured {e}",
                    ResponseStatus.InitialError));
            }
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

