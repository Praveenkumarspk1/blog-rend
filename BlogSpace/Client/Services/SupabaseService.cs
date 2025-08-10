using Supabase;
using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;
using Microsoft.Extensions.Configuration;
using BlogSpace.Client.Models;
using System.Text.Json;
using Postgrest.Models;

namespace BlogSpace.Client.Services
{
    public class SupabaseService : ISupabaseService
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly IConfiguration _configuration;

        public SupabaseService(IConfiguration configuration)
        {
            _configuration = configuration;
            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true
            };

            // Use the provided Supabase URL and anon key directly
            var url = "https://lwsflfvyvmdqsqxylvtp.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Imx3c2ZsZnZ5dm1kcXNxeHlsdnRwIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTE1MTMxNjksImV4cCI6MjA2NzA4OTE2OX0.in-eHHM8SGQzUJJvycQ7i2Y-1ozwkeJQCBnGNVbqcDk";
            _supabaseClient = new Supabase.Client(url, key, options);
        }

        public async Task<User?> GetSession()
        {
            try
            {
                var session = await _supabaseClient.Auth.RetrieveSessionAsync();
                return session?.User;
            }
            catch
            {
                return null;
            }
        }

        public async Task SignIn(string email, string password)
        {
            try
            {
                await _supabaseClient.Auth.SignIn(email, password);
            }
            catch (GotrueException)
            {
                // Re-throw GotrueException so it can be handled by the calling code
                throw;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                throw new Exception($"Sign in failed: {ex.Message}", ex);
            }
        }

        public async Task SignUp(string email, string password, string username, string fullName)
        {
            try
            {
                var signUpOptions = new Supabase.Gotrue.SignUpOptions
                {
                    Data = new Dictionary<string, object>
                    {
                        { "username", username },
                        { "full_name", fullName }
                    }
                };
                
                var response = await _supabaseClient.Auth.SignUp(email, password, signUpOptions);
                if (response.User == null)
                    throw new Exception("Registration failed");
                // Do NOT insert into profiles manually! The trigger does it.
            }
            catch (GotrueException)
            {
                // Re-throw GotrueException so it can be handled by the calling code
                throw;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                throw new Exception($"Registration failed: {ex.Message}", ex);
            }
        }

        public async Task SignOut()
        {
            try
            {
                await _supabaseClient.Auth.SignOut();
            }
            catch (GotrueException)
            {
                // Re-throw GotrueException so it can be handled by the calling code
                throw;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                throw new Exception($"Sign out failed: {ex.Message}", ex);
            }
        }

        public async Task<UserProfile?> GetUserProfile(string username)
        {
            try
            {
                var response = await _supabaseClient.From<UserProfile>()
                    .Select("*")
                    .Filter("username", Postgrest.Constants.Operator.Equals, username)
                    .Single();
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user profile for username '{username}': {ex.Message}");
                return null;
            }
        }

        public async Task<UserProfile?> GetUserProfileById(string userId)
        {
            try
            {
                var response = await _supabaseClient.From<UserProfile>()
                    .Select("*")
                    .Filter("id", Postgrest.Constants.Operator.Equals, userId)
                    .Single();
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user profile for ID '{userId}': {ex.Message}");
                return null;
            }
        }

        public async Task<List<Post>> GetPosts()
        {
            var response = await _supabaseClient.From<Post>()
                .Select("*")
                .Order("created_at", Postgrest.Constants.Ordering.Descending)
                .Get();
            return response.Models;
        }

        public async Task<List<Post>> GetUserPosts(string userId)
        {
            var response = await _supabaseClient.From<Post>()
                .Select("*")
                .Filter("author_id", Postgrest.Constants.Operator.Equals, userId)
                .Order("created_at", Postgrest.Constants.Ordering.Descending)
                .Get();
            return response.Models;
        }

        public async Task<List<Post>> GetFollowingPosts()
        {
            var session = await GetSession();
            if (session?.Id == null) return new List<Post>();

            var follows = await _supabaseClient.From<Follow>()
                .Select("following_id")
                .Filter("follower_id", Postgrest.Constants.Operator.Equals, session.Id)
                .Get();

            var followingIds = follows.Models.Select(f => f.FollowingId).ToList();
            if (!followingIds.Any()) return new List<Post>();

            var response = await _supabaseClient.From<Post>()
                .Select("*")
                .Filter("author_id", Postgrest.Constants.Operator.In, followingIds)
                .Order("created_at", Postgrest.Constants.Ordering.Descending)
                .Get();
            return response.Models;
        }

        public async Task<List<Notification>> GetNotifications()
        {
            var session = await GetSession();
            if (session?.Id == null) return new List<Notification>();

            var response = await _supabaseClient.From<Notification>()
                .Select("*")
                .Filter("user_id", Postgrest.Constants.Operator.Equals, session.Id)
                .Order("created_at", Postgrest.Constants.Ordering.Descending)
                .Get();
            return response.Models;
        }

        public async Task<Post?> GetPost(string id)
        {
            var response = await _supabaseClient.From<Post>()
                .Select("*")
                .Filter("id", Postgrest.Constants.Operator.Equals, id)
                .Single();
            return response;
        }

        public async Task<Post?> GetPostBySlug(string slug)
        {
            var response = await _supabaseClient.From<Post>()
                .Select("*")
                .Filter("slug", Postgrest.Constants.Operator.Equals, slug)
                .Single();
            return response;
        }

        public async Task<Post> CreatePost(Post post)
        {
            var session = await GetSession();
            if (session?.Id == null)
                throw new UnauthorizedAccessException("User not authenticated");

            try
            {
                Console.WriteLine($"Creating post with AuthorId: {session.Id}");
                Console.WriteLine($"Post Title: {post.Title}");
                Console.WriteLine($"Post Tags: [{string.Join(", ", post.Tags)}]");
                
                // Create a new post object with proper values
                var newPost = new Post
                {
                    Id = Guid.NewGuid().ToString(), // Generate a new GUID for the post ID
                    Title = post.Title,
                    Content = post.Content,
                    Summary = post.Summary,
                    Slug = post.Slug,
                    Tags = post.Tags,
                    AuthorId = session.Id!,
                    Published = post.Published,
                    Visibility = post.Visibility,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                Console.WriteLine($"New Post AuthorId: '{newPost.AuthorId}'");
                Console.WriteLine($"New Post Title: '{newPost.Title}'");
                Console.WriteLine($"New Post Content Length: {newPost.Content?.Length ?? 0}");
                Console.WriteLine($"New Post Tags: [{string.Join(", ", newPost.Tags)}]");
                
                var response = await _supabaseClient.From<Post>()
                    .Insert(newPost);
                var result = response.Models.FirstOrDefault();
                if (result == null)
                    throw new Exception("Failed to create post");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating post: {ex.Message}");
                Console.WriteLine($"Error details: {ex}");
                throw;
            }
        }

        public async Task<Post> UpdatePost(Post post)
        {
            post.UpdatedAt = DateTime.UtcNow;
            var response = await _supabaseClient.From<Post>()
                .Filter("id", Postgrest.Constants.Operator.Equals, post.Id)
                .Update(post);
            var result = response.Models.FirstOrDefault();
            if (result == null)
                throw new Exception("Failed to update post");
            return result;
        }

        public async Task DeletePost(string id)
        {
            await _supabaseClient.From<Post>()
                .Filter("id", Postgrest.Constants.Operator.Equals, id)
                .Delete();
        }

        public async Task<bool> IsFollowing(string followingId)
        {
            var session = await GetSession();
            if (session?.Id == null) return false;

            var response = await _supabaseClient.From<Follow>()
                .Select("*")
                .Filter("follower_id", Postgrest.Constants.Operator.Equals, session.Id)
                .Filter("following_id", Postgrest.Constants.Operator.Equals, followingId)
                .Get();
            return response.Models.Any();
        }

        public async Task Follow(string followingId)
        {
            var session = await GetSession();
            if (session?.Id == null) throw new UnauthorizedAccessException();

            var follow = new Follow
            {
                FollowerId = session.Id!,
                FollowingId = followingId,
                Status = "accepted",
                CreatedAt = DateTime.UtcNow
            };

            await _supabaseClient.From<Follow>().Insert(follow);
        }

        public async Task Unfollow(string followingId)
        {
            var session = await GetSession();
            if (session?.Id == null) throw new UnauthorizedAccessException();

            await _supabaseClient.From<Follow>()
                .Filter("follower_id", Postgrest.Constants.Operator.Equals, session.Id)
                .Filter("following_id", Postgrest.Constants.Operator.Equals, followingId)
                .Delete();
        }

        public async Task<List<string>> GetFollowingIds()
        {
            var session = await GetSession();
            if (session?.Id == null) return new List<string>();

            var response = await _supabaseClient.From<Follow>()
                .Select("following_id")
                .Filter("follower_id", Postgrest.Constants.Operator.Equals, session.Id)
                .Get();
            return response.Models.Select(f => f.FollowingId).ToList();
        }

        public async Task<List<string>> GetFollowerIds()
        {
            var session = await GetSession();
            if (session?.Id == null) return new List<string>();

            var response = await _supabaseClient.From<Follow>()
                .Select("follower_id")
                .Filter("following_id", Postgrest.Constants.Operator.Equals, session.Id)
                .Get();
            return response.Models.Select(f => f.FollowerId).ToList();
        }

        public async Task MarkNotificationAsRead(string notificationId)
        {
            var updateData = new Notification { Read = true };
            await _supabaseClient.From<Notification>()
                .Filter("id", Postgrest.Constants.Operator.Equals, notificationId)
                .Update(updateData);
        }

        public async Task MarkAllNotificationsAsRead()
        {
            var session = await GetSession();
            if (session?.Id == null) return;

            var updateData = new Notification { Read = true };
            await _supabaseClient.From<Notification>()
                .Filter("user_id", Postgrest.Constants.Operator.Equals, session.Id)
                .Filter("read", Postgrest.Constants.Operator.Equals, false)
                .Update(updateData);
        }
    }
} 