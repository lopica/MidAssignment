namespace MidAssignment.DTOs
{
    public record LoginDto(string Email, string Password, bool? IsRememberLogin = false);
}