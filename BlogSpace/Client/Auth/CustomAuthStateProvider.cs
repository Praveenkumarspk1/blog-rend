using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using BlogSpace.Client.Services;
using Supabase.Gotrue;

namespace BlogSpace.Client.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ISupabaseService _supabaseService;

        public CustomAuthStateProvider(ISupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = await _supabaseService.GetSession();

            if (user == null)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, "User")
            };

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "Supabase")));
        }

        public void NotifyAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
} 