namespace AutoDocAi.DTOs.UserDTOs;

public class Register
{
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}