using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.FeedAPI.Services.Interface
{
    public interface IFeedService
    {
        Task<Response<List<dynamic>>> GetFeedActivities(string userId, int skip = 0, int take = 10);
    }
}
