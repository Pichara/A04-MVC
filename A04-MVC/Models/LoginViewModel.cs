/*
* FILE         : LoginViewModel.cs
* PROJECT      : A04-MVC
* PROGRAMMER   : Rodrigo Pichara Gomes
* FIRST VERSION: 2026-04-01
* DESCRIPTION  : View model for user login form
*                Contains username and password for authentication
*/

using System.ComponentModel.DataAnnotations;

namespace A04_MVC.Models
{
    /// <summary>
    /// View model for the login form
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Username for login
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [StringLength(25, ErrorMessage = "Username cannot exceed 25 characters")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password for login
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, ErrorMessage = "Password cannot exceed 50 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
    }
}
