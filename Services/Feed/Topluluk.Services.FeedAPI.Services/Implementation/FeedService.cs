using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Topluluk.Services.FeedAPI.Services.Interface;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.FeedAPI.Services.Implementation
{
    public class FeedService : IFeedService
    {
        private readonly RestClient _client;

        public FeedService()
        {
            _client = new RestClient();
        }
        public async Task<Response<List<dynamic>>> GetFeedActivities(string userId, int skip = 0, int take = 10)
        {
            // UserId ile kendi kullanıcı bilgimizi çekip followings de gelecek olan postun UserId si var mı bakacağız.
            // Eventin UserId si userımızın followings inde varsa onu da alacağız.
            throw new NotImplementedException();
        }
    }
}
