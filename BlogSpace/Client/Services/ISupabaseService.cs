using Supabase.Gotrue;
using BlogSpace.Client.Models;

namespace BlogSpace.Client.Services
{
    public interface ISupabaseService
    {
        Task<User?> GetSession();
        Task<List<Post>> GetPosts();
        Task<List<Post>> GetUserPosts(string userId);
        Task<List<Post>> GetFollowingPosts();
        Task<List<Notification>> GetNotifications();
        Task<Post?> GetPost(string id);
        Task<Post?> GetPostBySlug(string slug);
        Task<Post> CreatePost(Post post);
        Task<Post> UpdatePost(Post post);
        Task DeletePost(string id);
        Task<bool> IsFollowing(string followingId);
        Task Follow(string followingId);
        Task Unfollow(string followingId);
        Task<List<string>> GetFollowingIds();
        Task<List<string>> GetFollowerIds();
        Task SignIn(string email, string password);
        Task SignUp(string email, string password, string username, string fullName);
        Task SignOut();
        Task<UserProfile?> GetUserProfile(string username);
        Task<UserProfile?> GetUserProfileById(string userId);
        Task MarkNotificationAsRead(string notificationId);
        Task MarkAllNotificationsAsRead();
    }
} 