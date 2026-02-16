using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using neuranx.Application.Auth.Commands;
using neuranx.Domain.RequestModel;

namespace neuranx.Api.Controllers;

public class AuthController : BaseController<AuthController>
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        Logger.LogInformation("Register request for email: {Email}", model.Email);
        if (!ModelState.IsValid) throw new Exception("Invalid Model");
        var authResult = await Mediator.Send(new RegisterUserCommand(model.UserName, model.Email, model.Password));

        if (!authResult)
        {
            throw new Exception("Registration failed user already exist");
        }
        return Ok(new { message = "success" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        Logger.LogInformation("Login request for email: {Email}", model.Email);
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var authResult = await Mediator.Send(new LoginUserCommand(model.Email, model.Password));

        if (authResult != null && authResult.Success)
        {
            Logger.LogInformation("Login successful for email: {Email}", model.Email);
            return Ok(authResult);
        }

        Logger.LogWarning("Login failed for email: {Email}", model.Email);
        return Unauthorized(new { Errors = authResult?.Errors ?? new List<string> { "Invalid login attempt" } });
    }

    [HttpPost("refresh-token")]
    [Authorize]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var user = CurrentUserService.UserId;
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var authResult = await Mediator.Send(new RefreshTokenCommand(user, request.RefreshToken));

        if (authResult != null && authResult.Success)
        {
            return Ok(authResult);
        }

        return BadRequest(new { Errors = authResult?.Errors ?? new List<string> { "Invalid token" } });
    }

    [HttpGet("GetAllUsers")]
    [Authorize]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await Mediator.Send(new GetAllUsersQuery());
        return Ok(users);
    }
}

public class RefreshTokenRequest
{
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
}
