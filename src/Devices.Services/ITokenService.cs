namespace Devices.Services.TokenService;
public interface ITokenService
{
    string GenerateToken(string username, string role);
}