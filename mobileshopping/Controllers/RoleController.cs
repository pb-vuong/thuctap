using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using mobileshopping.Models;
using NuGet.Protocol.Plugins;

[Route("api/role")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly UserManager<User> _userManager; 

    public RoleController(RoleManager<IdentityRole<int>> roleManager, UserManager<User> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {
        var roleExist = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            // Khởi tạo gán Name
            var result = await _roleManager.CreateAsync(new IdentityRole<int> { Name = roleName });

            if (result.Succeeded)
                return Ok(new { message = $"Role {roleName} đã được tạo thành công!" });
        }
        return BadRequest(new { message = "Role đã tồn tại!" });
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignRole([FromBody] UserRoleModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return NotFound(new { message = "User không tồn tại!" });

        var result = await _userManager.AddToRoleAsync(user, model.Role);
        if (result.Succeeded)
            return Ok(new { message = $"User {model.Email} đã được gán quyền {model.Role}!" });

        return BadRequest(result.Errors);
    }
}

public class UserRoleModel
{
    public string Email { get; set; }
    public string Role { get; set; }
}