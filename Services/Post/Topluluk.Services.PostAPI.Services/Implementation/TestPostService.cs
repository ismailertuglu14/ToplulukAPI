using MongoDB.Bson;
using Topluluk.Services.PostAPI.Data.Interface;
using Topluluk.Services.PostAPI.Model.Entity;
using Topluluk.Services.PostAPI.Services.Interface;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.PostAPI.Services.Implementation;

public class TestPostService : ITestPostService
{
    private readonly IPostRepository _postRepository;

    public TestPostService(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }


    public async Task<Response<NoContent>> CreatePostsForTest(int count)
    {
        List<Post> posts = new List<Post>();
        for (int i = 0; i < count; i++)
        {
            posts.Add(new Post()
            {
                Description = $"Description {i}"
            });
        }
        await _postRepository.InsertManyAsync(posts);
        return Response<NoContent>.Success(ResponseStatus.Success);
    }
}