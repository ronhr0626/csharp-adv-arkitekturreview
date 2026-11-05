using Microsoft.AspNetCore.Mvc;
using TaskService.Api.Data;
using TaskService.Api.DTO;
using TaskService.Api.Models;
using TaskService.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace TaskService.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ITokenReader _tokenReader;
    private readonly IPasswordHasher _passwordHasher;
    private readonly TaskDbContext _db;
    private readonly IJwtService _jwtService;

    public AuthController(ITokenReader tokenReader, IPasswordHasher passwordHasher, TaskDbContext db, IJwtService jwtService)
    {
        _tokenReader = tokenReader;
        _passwordHasher = passwordHasher;
        _db = db;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Username and password are required");
        }

        var username = request.Username.Trim();

        if (_db.Users.Any(u => u.Username == username))
        {
            return Conflict("Username is already taken");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            PasswordHash = _passwordHasher.Hash(request.Password)
        };

        _db.Users.Add(user);
        _db.SaveChanges();

        // Never return the password hash to the client
        return Created($"/auth/users/{user.Id}", new { user.Id, user.Username });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _db.Users.FirstOrDefault(u => u.Username == request.Username.Trim());

        // Same answer for wrong username and wrong password, so nobody can guess which usernames exist
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Wrong username or password");
        }

        var token = _jwtService.CreateToken(user);
        return Ok(new { token });
    }
    
    [Authorize]
    [HttpGet("token")]
    public IActionResult ReadToken()
    {
        var result = _tokenReader.ReadToken(Request.Headers.Authorization.ToString());

        if (result is null)
        {
            return BadRequest("Invalid or unreadable token");
        }

        return Ok(result);
    }
}
