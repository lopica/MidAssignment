namespace MidAssignment.Services
{
    public interface IJWTServices
    {
        public string GenerateTokenWithPublicKey(string email, string publicKey = "", bool isRefreshToken = false, string role = "User");
    }
}
