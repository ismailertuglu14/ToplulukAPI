﻿using System;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.EventAPI.Model.Dto.Http
{
	public class GetUserInfoDto
	{
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string? ProfileImage { get; set; }
        public GenderEnum Gender { get; set; }

        public GetUserInfoDto()
        {
        }
    }
}

