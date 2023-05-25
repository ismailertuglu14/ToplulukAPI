using System;
using AutoMapper;
using Topluluk.Services.PostAPI.Model.Dto;
using Topluluk.Services.PostAPI.Model.Dto.Http;
using Topluluk.Services.PostAPI.Model.Entity;

namespace Topluluk.Services.PostAPI.Model.Mapper
{
	public class GeneralMapper : Profile
	{
		public GeneralMapper()
		{
			CreateMap<CreatePostDto, Post>().ForMember(d => d.Files, opt => opt.Ignore());
			CreateMap<CommentCreateDto, PostComment>();
			CreateMap<Post, GetPostDto>()
										.ForMember(d => d.SharedAt, s => s.MapFrom(s => s.CreatedAt));


			CreateMap<PostComment, CommentGetDto>();
			CreateMap<Post, GetPostForFeedDto>();
			CreateMap<Post, GetPostByIdDto>();
			CreateMap<PostInteraction, UserInfoDto>().ForMember(d => d.Id, s => s.MapFrom(s => s.UserId));
		}
	}
}

