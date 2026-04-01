/*
* FILE         : Category.cs
* PROJECT      : A04-MVC
* PROGRAMMER   : Rodrigo Pichara Gomes
* FIRST VERSION: 2026-04-01
* DESCRIPTION  : Represents an inventory category for organizing items
*                Categories include Furniture, Electronics, Appliances, etc
*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A04_MVC.Models
{
    /// <summary>
    /// Entity class representing an inventory category
    /// </summary>
    [Table("Category")]
    public class Category
    {
        /// <summary>
        /// Unique identifier for the category (Primary Key)
        /// </summary>
        [Key]
        [Column("CategoryID")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Name of the category (e.g., Furniture, Electronics)
        /// </summary>
        [Required]
        [StringLength(50)]
        [Column("CategoryName")]
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Navigation property for items in this category
        /// </summary>
        public ICollection<ItemCatalog> ItemCatalogs { get; set; } = new List<ItemCatalog>();
    }
}
