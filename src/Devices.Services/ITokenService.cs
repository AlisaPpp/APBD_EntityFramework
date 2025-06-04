namespace Devices.Services.TokenService;
public interface ITokenService
{
    string GenerateToken(int userId, string username, string role);
}