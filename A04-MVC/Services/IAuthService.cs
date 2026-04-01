/*
* FILE         : IAuthService.cs
* PROJECT      : A04-MVC
* PROGRAMMER   : Rodrigo Pichara Gomes
* FIRST VERSION: 2026-04-01
* DESCRIPTION  : Interface for authentication service.
*                Defines methods for user login and session management.
*/

using A04_MVC.Models;

namespace A04_MVC.Services
{
    /// <summary>
    /// Interface for authentication and session management services.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user with username and password.
        /// </summary>
        /// <param name="username">Username to authenticate.</param>
        /// <param name="password">Password to verify.</param>
        /// <returns>LoginInfo if authentication succeeds, null otherwise.</returns>
        LoginInfo? AuthenticateUser(string username, string password);

        /// <summary>
        /// Gets user information by user ID.
        /// </summary>
        /// <param name="userId">User ID to look up.</param>
        /// <returns>LoginInfo if found, null otherwise.</returns>
        LoginInfo? GetUserById(long userId);

        /// <summary>
        /// Registers a new user with the specified username and password.
        /// </summary>
        /// <param name="username">Username for the new user.</param>
        /// <param name="password">Password for the new user.</param>
        /// <returns>Result indicating success or failure with message.</returns>
        (bool success, string message) RegisterUser(string username, string password);
    }
}
