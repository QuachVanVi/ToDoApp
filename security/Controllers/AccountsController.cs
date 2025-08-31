
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using security.Models;
using security.ViewModels;



namespace security.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(SignInManager<User> signInManager) : ControllerBase
{
    private readonly HtmlSanitizer _htmlSanitizer = new();


    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {


        if (model == null || string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
            return BadRequest("Username and password are required.");

        var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);

        if (!result.Succeeded)
            return Unauthorized("Invalid username or password.");

        var user = await signInManager.UserManager.FindByNameAsync(model.UserName);
        if (user == null)
        {
            return Unauthorized("User not found.");
        }

        var roles = await signInManager.UserManager.GetRolesAsync(user);

        Console.WriteLine($"User {user.Email} roles: {string.Join(",", roles)}");
        return Ok(new
        {
            message = "Logged in successfully",
            role = roles
        });

    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    // [ValidateAntiForgeryToken]
    public async Task<ActionResult> RegisterUser(UserViewModel model)
    {
        if (!ModelState.IsValid) return ValidationProblem();

        model.Email = _htmlSanitizer.Sanitize(model.Email);
        model.FirstName = _htmlSanitizer.Sanitize(model.FirstName);
        model.LastName = _htmlSanitizer.Sanitize(model.LastName);
        model.Password = _htmlSanitizer.Sanitize(model.Password);


        ModelState.Clear();
        TryValidateModel(model);

        if (!ModelState.IsValid) return ValidationProblem();

        var user = new User
        {
            UserName = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email
        };

        var result = await signInManager.UserManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(model.Role) &&
                (model.Role == "Admin" || model.Role == "User"))
            {
                await signInManager.UserManager.AddToRoleAsync(user, model.Role);
            }

            return Ok(new { success = true, role = model.Role });
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.Code, error.Description);
        }

        return ValidationProblem();
    }


    // [ValidateAntiForgeryToken]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return NoContent();
    }
}

