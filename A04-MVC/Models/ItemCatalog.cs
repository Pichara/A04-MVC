/*
* FILE         : ItemCatalog.cs
* PROJECT      : A04-MVC
* PROGRAMMER   : Rodrigo Pichara Gomes
* FIRST VERSION: 2026-04-01
* DESCRIPTION  : Represents an inventory item in the catalog
*                Each item belongs to a specific user and category
*/

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A04_MVC.Models
{
    /// <summary>
    /// Entity class representing an inventory item
    /// </summary>
    [Table("ItemCatalog")]
    public class ItemCatalog
    {
        /// <summary>
        /// Foreign key to the user who owns this item
        /// Part of composite key with item details
        /// </summary>
        [Column("UserID")]
        public long UserId { get; set; }

        /// <summary>
        /// Name of the inventory item
        /// </summary>
        [Required]
        [StringLength(80)]
        [Column("Item")]
        public string Item { get; set; } = string.Empty;

        /// <summary>
        /// Price of the item in dollars
        /// </summary>
        [Required]
        [Column("Price")]
        public double Price { get; set; }

        /// <summary>
        /// Date when the item was purchased
        /// </summary>
        [Required]
        [Column("DateBought")]
        public DateTime DateBought { get; set; }

        /// <summary>
        /// Foreign key to the item's category
        /// </summary>
        [Required]
        [Column("CategoryID")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Navigation property to the user who owns this item
        /// </summary>
        [ForeignKey("UserId")]
        public LoginInfo? User { get; set; }

        /// <summary>
        /// Navigation property to the item's category
        /// </summary>
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
}
