using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.EventAPI.Model.Dto
{
    public class GetEventAttendeesDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfileImage { get; set; }
        public GenderEnum Gender { get; set; }
    }
}
