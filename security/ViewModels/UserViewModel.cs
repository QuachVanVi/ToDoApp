using System;
using System.ComponentModel.DataAnnotations;

namespace security.ViewModels;

public class UserViewModel
{
    [Required]
    public string FirstName { get; set; } = "";

    [Required]
    public string LastName { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";


    public string Password { get; set; } = "";

    public string Role { get; set; } = "";
}