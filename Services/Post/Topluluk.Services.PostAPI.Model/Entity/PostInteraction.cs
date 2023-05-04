using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.PostAPI.Model.Entity
{
    public class PostInteraction : AbstractEntity
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
        public InteractionEnum InteractionType { get; set; }
    }
}
