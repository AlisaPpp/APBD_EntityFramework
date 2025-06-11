using Microsoft.AspNetCore.Mvc;
using Devices.Services;
using Devices.Entities;
using Devices.Entities.DTOs;
using Devices.Services.TokenService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Devices.API;

[Route("/api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly DeviceDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly PasswordHasher<Account> _passwordHasher = new();
    private readonly ILogger<AuthController> _logger;

    public AuthController(DeviceDbContext context, ITokenService tokenService, ILogger<AuthController> logger)
    {
        _context = context;
        _tokenService = tokenService;
        _logger = logger;
    }
    
    [HttpPost]
    [Route("/api/auth")]
    public async Task<IActionResult> Auth(LoginDto user, CancellationToken cancellationToken)
    {
        var foundUser = await _context.Accounts.Include(a => a.Role)
            .FirstOrDefaultAsync(a => a.Username.Equals(user.Login), cancellationToken);
        if (foundUser == null)
        {
            _logger.LogError($"User with username {user.Login} does not exist");
            return Unauthorized();
        }
            
        var verificationResult = _passwordHasher.VerifyHashedPassword(foundUser, foundUser.Password, user.Password);
        
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            _logger.LogError($"User with username {user.Login} provided incorrect password");
            return Unauthorized();
        }

        var accessToken = new TokenResponseDto
        {
            AccessToken = _tokenService.GenerateToken(
                foundUser.Id,
                foundUser.Username,
                foundUser.Role.Name
            )
        };

        return Ok(accessToken);
    }
}