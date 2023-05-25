using Topluluk.Shared.Dtos;

namespace Topluluk.Services.PostAPI.Services.Interface;

public interface ITestPostService
{
    Task<Response<NoContent>> CreatePostsForTest(int count);

}