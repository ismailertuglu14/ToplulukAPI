using System;
using AutoMapper;
using Topluluk.Services.User.Model.Dto;
using Topluluk.Services.User.Model.Dto.Http;
using _User = Topluluk.Services.User.Model.Entity.User;
namespace Topluluk.Services.User.Model.Mapper
{
	public class GeneralMapper : Profile
	{
		public GeneralMapper()
		{
			CreateMap<UserInsertDto, _User>().ReverseMap();
			CreateMap<_User, UserSuggestionsDto>();
			CreateMap<_User, UserSearchResponseDto>().ReverseMap();

			CreateMap<_User, GetUserByIdDto>();
											

            CreateMap<_User, GetCommunityOwnerDto>();
            CreateMap<_User, UserInfoForCommentDto>();
            CreateMap<_User, UserInfoForPostDto>();
			CreateMap<_User, GetUserAfterLoginDto>();
               
			CreateMap<_User, FollowingRequestDto>();
			CreateMap<_User, FollowingUserDto>();
			CreateMap<_User, FollowerUserDto>();
			CreateMap<_User, UserFollowRequestDto>();

			CreateMap<_User, UserSuggestionsDto>();
		}		
	}
}

