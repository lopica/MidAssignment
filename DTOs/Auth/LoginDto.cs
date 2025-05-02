namespace MidAssignment.DTOs.Auth
{
    public record LoginDto(string Email, string Password, bool? IsRememberLogin = false);
}