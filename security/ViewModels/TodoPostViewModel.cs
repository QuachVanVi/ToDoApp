using System.ComponentModel.DataAnnotations;

namespace security.ViewModels;

public class ToDoPostViewModel
{
    [Required(ErrorMessage = "Titeln måste anges.")]
  [MinLength(2, ErrorMessage = "Titeln måste vara minst 2 tecken.")]
        [MaxLength(100, ErrorMessage = "Titeln får inte vara längre än 100 tecken.")]
    public string Title { get; set; } = "";

}