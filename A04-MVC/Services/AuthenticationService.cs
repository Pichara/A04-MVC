/*
* FILE         : AuthenticationService.cs
* PROJECT      : A04-MVC
* PROGRAMMER   : Rodrigo Pichara Gomes
* FIRST VERSION: 2026-04-01
* DESCRIPTION  : Implementation of authentication service.
*                Handles user authentication using hashed password storage.
*/

using A04_MVC.Data;
using A04_MVC.Models;

namespace A04_MVC.Services
{
    /// <summary>
    /// Service for handling user authentication and authorization.
    /// </summary>
    public class AuthenticationService : IAuthService
    {
        /// <summary>
        /// Database context for accessing user data.
        /// </summary>
        private readonly MvcDbContext _context;

        /// <summary>
        /// Initializes a new instance of AuthenticationService.
        /// </summary>
        /// <param name="context">Database context.</param>
        public AuthenticationService(MvcDbContext context)
        {
            _context = context;
            return;
        }

        /// <summary>
        /// Authenticates a user by verifying username and password.
        /// </summary>
        /// <param name="username">Username to authenticate.</param>
        /// <param name="password">Password to verify.</param>
        /// <returns>LoginInfo if authentication succeeds, null otherwise.</returns>
        public LoginInfo? AuthenticateUser(string username, string password)
        {
            LoginInfo? result = null;

            // Find user by username and verify the supplied password.
            LoginInfo? user = _context.LoginInfos
                .FirstOrDefault(u => u.Username == username);

            if (user != null && PasswordHashService.VerifyPassword(password, user.Password))
            {
                if (!PasswordHashService.IsPasswordHash(user.Password))
                {
                    user.Password = PasswordHashService.HashPassword(password);
                    _context.SaveChanges();
                }

                result = user;
            }

            return result;
        }

        /// <summary>
        /// Gets user information by user ID.
        /// </summary>
        /// <param name="userId">User ID to look up.</param>
        /// <returns>LoginInfo if found, null otherwise.</returns>
        public LoginInfo? GetUserById(long userId)
        {
            LoginInfo? result = _context.LoginInfos
                .Find(userId);

            return result;
        }

        /// <summary>
        /// Registers a new user with the specified username and password.
        /// </summary>
        /// <param name="username">Username for the new user.</param>
        /// <param name="password">Password for the new user.</param>
        /// <returns>Result indicating success or failure with message.</returns>
        public (bool success, string message) RegisterUser(string username, string password)
        {
            (bool success, string message) result;

            // Check if username already exists
            LoginInfo? existingUser = _context.LoginInfos
                .FirstOrDefault(u => u.Username == username);

            if (existingUser != null)
            {
                result = (false, "Username is already taken");
            }
            else
            {
                // Create new user
                LoginInfo newUser = new LoginInfo
                {
                    Username = username,
                    Password = PasswordHashService.HashPassword(password)
                };

                // Add to database
                _context.LoginInfos.Add(newUser);

                try
                {
                    _context.SaveChanges();
                    result = (true, "Registration successful");
                }
                catch (Exception)
                {
                    result = (false, "An error occurred during registration");
                }
            }

            return result;
        }
    }
}
