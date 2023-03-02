using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;

namespace Topluluk.Shared.Helper
{
	public static class HttpRequestHelper
	{
		public static async Task<HttpResponseMessage> handle<T>(T? content,string endpoint, HttpType httpType, string mediaType = "application/json")
        {
            var httpClient = new HttpClient();
            var contentHttp = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, mediaType);
			HttpResponseMessage responseHttp = new();
			switch (httpType)
			{
				case HttpType.GET:
					responseHttp = await httpClient.GetAsync(endpoint);
					break;
                case HttpType.POST:
                    responseHttp = await httpClient.PostAsync(endpoint, contentHttp);
                    break;
                case HttpType.PUT:
                    responseHttp = await httpClient.PutAsync(endpoint, contentHttp);
                    break;
                case HttpType.DELETE:
                    responseHttp = await httpClient.DeleteAsync(endpoint);
                    break;
                default:
                    break;
            }

            return responseHttp;
		}
	}
}

