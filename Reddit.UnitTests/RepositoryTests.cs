using Microsoft.EntityFrameworkCore;
using Reddit;
using Reddit.Models;
using Reddit.Repositories;
namespace Reddit.UnitTests;

public class RepositoryTests
{
    private IPostsRepository GetPostsRepostory()
    {
        var dbName = Guid.NewGuid().ToString();    
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        
        var dbContext = new ApplicationDbContext(options);
        dbContext.Posts.Add(new Post { Title = "Title 1", Content = "Content 1", Upvote = 5, Downvote = 1 });
        dbContext.Posts.Add(new Post { Title = "Title 2", Content = "Content 1", Upvote = 12, Downvote = 1 });
        dbContext.Posts.Add(new Post { Title = "Title 3", Content = "Content 1", Upvote = 3, Downvote = 1 });
        dbContext.Posts.Add(new Post { Title = "Title 4", Content = "Content 1", Upvote = 221, Downvote = 1 }); 
        dbContext.Posts.Add(new Post { Title = "Title 5", Content = "Content 1", Upvote = 5, Downvote = 2123 }); 
        dbContext.Posts.Add(new Post { Title = "Title 6", Content = "Content 1", Upvote = 6, Downvote = 2122 }); 
        dbContext.SaveChanges();
        return new PostsRepository(dbContext);
    }
    [Fact]
    public async Task GetPosts_ReturnsCorrectPagination()
    {
        var postsRepository = GetPostsRepostory();
        var posts = await postsRepository.GetPosts(1, 2, null, null, false);
        Assert.Equal(2, posts.Items.Count);
        Assert.Equal(6, posts.TotalCount);
        Assert.True(posts.HasNextPage);
        Assert.False(posts.HasPreviousPage);
    }

    [Fact]
    public async Task GetPosts_ReturnsCorrect()
    {
        var postsRepository = GetPostsRepostory();
        var posts = await postsRepository.GetPosts(1, 2, null, "popular", false);
        Assert.Equal(2, posts.Items.Count);
        Assert.Equal(6, posts.TotalCount);
        Assert.True(posts.HasNextPage);
        Assert.False(posts.HasPreviousPage);
        Assert.Equal("Title 5", posts.Items.First().Title);
    }
    
    [Fact]
    public async Task GetPosts_InvalidPage_ThrowsArgumentException()
    {
        var repository = GetPostsRepostory();
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository.GetPosts(pageNumber: 0, pageSize: 10, searchTerm: null, sortTerm: null));
        Assert.Equal("pageNumber", exception.ParamName);
    }
    [Fact]
    public async Task GetPosts_InvalidPageSize_ThrowsArgumentOutOfRangeException_When_Page_Size_Is_Zero()
    {
        var repository = GetPostsRepostory();
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository.GetPosts(pageNumber: 1, pageSize: 0, searchTerm: null, sortTerm: null));
        Assert.Equal("pageSize", exception.ParamName);
    }
    [Fact]
    public async Task GetPosts_InvalidPageSize_ThrowsArgumentOutOfRangeException_When_Page_Size_Is_Negative()
    {
        var repository = GetPostsRepostory();
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository.GetPosts(pageNumber: 1, pageSize: -1, searchTerm: null, sortTerm: null));
        Assert.Equal("pageSize", exception.ParamName);
    }   
    [Fact]
    public async Task GetPosts_InvalidPageSize_ThrowsArgumentOutOfRangeException_When_Page_Is_Out_Of_Range()
    {
        var repository = GetPostsRepostory();
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository.GetPosts(pageNumber: 0, pageSize: 2, searchTerm: null, sortTerm: null));
        Assert.Equal("pageNumber", exception.ParamName);
    }
}