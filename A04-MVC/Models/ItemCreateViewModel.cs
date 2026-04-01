/*
* FILE         : ItemCreateViewModel.cs
* PROJECT      : A04-MVC
* PROGRAMMER   : Rodrigo Pichara Gomes
* FIRST VERSION: 2026-04-01
* DESCRIPTION  : View model for creating new inventory items
*                Contains item details for the add/edit form
*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace A04_MVC.Models
{
    /// <summary>
    /// View model for creating or editing inventory items
    /// </summary>
    public class ItemCreateViewModel
    {
        /// <summary>
        /// Name of the item
        /// </summary>
        [Required(ErrorMessage = "Item name is required")]
        [StringLength(80, ErrorMessage = "Item name cannot exceed 80 characters")]
        [Display(Name = "Item Name")]
        public string Item { get; set; } = string.Empty;

        /// <summary>
        /// Date when the item was purchased
        /// </summary>
        [Required(ErrorMessage = "Purchase date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date Purchased")]
        public DateTime DateBought { get; set; }

        /// <summary>
        /// Price of the item
        /// </summary>
        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Display(Name = "Price")]
        public double Price { get; set; }

        /// <summary>
        /// Category ID for the item
        /// </summary>
        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        /// <summary>
        /// List of available categories for dropdown
        /// </summary>
        public IEnumerable<SelectListItem>? Categories { get; set; }
    }
}
