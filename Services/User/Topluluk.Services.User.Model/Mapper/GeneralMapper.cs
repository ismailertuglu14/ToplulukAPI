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
            CreateMap<_User, GetUserAfterLoginDto>()
                .ForMember(d => d.FollowersCount, s => s.MapFrom(s => s.Followers.Count))
                .ForMember(d => d.FollowingsCount, s => s.MapFrom(s => s.Followings.Count));
        }		
	}
}

