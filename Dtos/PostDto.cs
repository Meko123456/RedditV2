using Reddit.Models;

namespace Reddit.Dtos
{
    public class PostDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int AuthorId { get; set; }
        public string CommunityName { get; set; }
        public int CommunityID { get; set; }


        public Post CreatePost() {
        return new Post { Title = Title, Content = Content,
            AuthorId = AuthorId, CommunityName = CommunityName, CommunityID = CommunityID };
        }
    }
}
