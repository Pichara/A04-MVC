/*
* FILE         : LoginInfo.cs
* PROJECT      : A04-MVC
* PROGRAMMER   : Rodrigo Pichara Gomes
* FIRST VERSION: 2026-04-01
* DESCRIPTION  : Represents a user account for authentication and authorization
*                Stores user credentials and links to inventory items
*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A04_MVC.Models
{
    /// <summary>
    /// Entity class representing user login information and authentication data
    /// </summary>
    [Table("LoginInfo")]
    public class LoginInfo
    {
        /// <summary>
        /// Unique identifier for the user (Primary Key)
        /// </summary>
        [Key]
        [Column("UserID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserId { get; set; }

        /// <summary>
        /// Username for login (must be unique)
        /// </summary>
        [Required]
        [StringLength(25)]
        [Column("Username")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Hashed password for authentication
        /// </summary>
        [Required]
        [StringLength(256)]
        [Column("Password")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Navigation property for user's catalog items
        /// </summary>
        public ICollection<ItemCatalog> ItemCatalogs { get; set; } = new List<ItemCatalog>();
    }
}
