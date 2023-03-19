using System;
using AutoMapper;
using Topluluk.Services.PostAPI.Model.Dto;
using Topluluk.Services.PostAPI.Model.Entity;

namespace Topluluk.Services.PostAPI.Model.Mapper
{
	public class GeneralMapper : Profile
	{
		public GeneralMapper()
		{
			CreateMap<CreatePostDto, Post>();
			CreateMap<CommentCreateDto, PostComment>();
			CreateMap<Post,GetPostDto>().ForMember(d => d.InteractionCount, s => s.MapFrom(s => s.Interactions.Count));
			CreateMap<PostComment, CommentGetDto>().ForMember(d => d.InteractionCount, s => s.MapFrom(s => s.Interactions.Count));
		}
	}
}

