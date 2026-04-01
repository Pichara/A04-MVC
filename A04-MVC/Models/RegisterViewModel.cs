/*
* FILE         : RegisterViewModel.cs
* PROJECT      : A04-MVC
* PROGRAMMER   : Rodrigo Pichara Gomes
* FIRST VERSION: 2026-04-01
* DESCRIPTION  : View model for user registration form
*                Contains username, password, and confirm password fields
*/

using System.ComponentModel.DataAnnotations;

namespace A04_MVC.Models
{
    /// <summary>
    /// View model for the registration form
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// Username for registration
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [StringLength(25, ErrorMessage = "Username cannot exceed 25 characters")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password for registration
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, ErrorMessage = "Password cannot exceed 50 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Password confirmation field
        /// </summary>
        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
