using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.ChatAPI.Model.Entity
{
    public class Group : AbstractEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<string> Participiants { get; set; }

        public Group()
        {
            Participiants = new List<string>();
        }
    }
}
