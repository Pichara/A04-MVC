/*
* FILE         : LoginController.cs
* PROJECT      : A04-MVC
* PROGRAMMER   : Rodrigo Pichara Gomes
* FIRST VERSION: 2026-04-01
* DESCRIPTION  : Controller for user login and logout operations
*                Manages authentication sessions using cookie-based auth
*/

using A04_MVC.Models;
using A04_MVC.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace A04_MVC.Controllers
{
    /// <summary>
    /// Controller handling user authentication and session management
    /// </summary>
    public class LoginController : Controller
    {
        /// <summary>
        /// Authentication service for validating credentials
        /// </summary>
        private readonly IAuthService _authService;

        /// <summary>
        /// Logger for error tracking and diagnostics
        /// </summary>
        private readonly ILogger<LoginController> _logger;

        /// <summary>
        /// Initializes a new instance of LoginController
        /// </summary>
        /// <param name="authService">Authentication service.</param>
        /// <param name="logger">Logger instance.</param>
        public LoginController(IAuthService authService, ILogger<LoginController> logger)
        {
            _authService = authService;
            _logger = logger;
            return;
        }

        /// <summary>
        /// Displays the login form
        /// </summary>
        /// <returns>Login view.</returns>
        [HttpGet]
        public IActionResult Index()
        {
            IActionResult result = View();

            return result;
        }

        /// <summary>
        /// Processes login form submission
        /// </summary>
        /// <param name="model">Login view model with credentials.</param>
        /// <returns>Redirect to catalog on success, back to login on failure.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                // Authenticate user
                LoginInfo? user = _authService.AuthenticateUser(model.Username, model.Password);

                if (user != null)
                {
                    // Create claims for the user
                    List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.Username)
                    };

                    // Create identity and principal
                    ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                    // Sign in with cookie authentication
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    _logger.LogInformation("User {Username} logged in successfully", model.Username);

                    result = RedirectToAction("Index", "Catalog");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password");
                    result = View(model);
                }
            }
            else
            {
                result = View(model);
            }

            return result;
        }

        /// <summary>
        /// Logs out the current user
        /// </summary>
        /// <returns>Redirect to login page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            IActionResult result;

            // Sign out user
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            _logger.LogInformation("User logged out");

            result = RedirectToAction("Index", "Login");

            return result;
        }

        /// <summary>
        /// Displays the registration form
        /// </summary>
        /// <returns>Register view.</returns>
        [HttpGet]
        public IActionResult Register()
        {
            IActionResult result = View();

            return result;
        }

        /// <summary>
        /// Processes registration form submission
        /// </summary>
        /// <param name="model">Register view model with user details.</param>
        /// <returns>Redirect to login on success, back to register on failure.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                // Register new user
                (bool success, string message) = _authService.RegisterUser(model.Username, model.Password);

                if (success)
                {
                    _logger.LogInformation("New user {Username} registered successfully", model.Username);

                    // Add success message to TempData to display on login page
                    TempData["RegistrationSuccess"] = "Registration successful! Please login.";

                    result = RedirectToAction("Index", "Login");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, message);
                    result = View(model);
                }
            }
            else
            {
                result = View(model);
            }

            return result;
        }
    }
}
