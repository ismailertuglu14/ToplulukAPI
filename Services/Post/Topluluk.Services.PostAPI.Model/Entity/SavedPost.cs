using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.PostAPI.Model.Entity
{
    public class SavedPost : AbstractEntity
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
    }
}
