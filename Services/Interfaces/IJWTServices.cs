namespace MidAssignment.Services.Interfaces
{
    public interface IJWTServices
    {
        public string GenerateTokenWithPublicKey(string email, string publicKey = "", bool isRefreshToken = false, string role = "User");
        public CookieOptions AccessTokenCookieOption();
        public CookieOptions RefreshTokenCookieOption();
    }
}
