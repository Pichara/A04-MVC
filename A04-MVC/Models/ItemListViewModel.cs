/*
* FILE         : ItemListViewModel.cs
* PROJECT      : A04-MVC
* PROGRAMMER   : Rodrigo Pichara Gomes
* FIRST VERSION: 2026-04-01
* DESCRIPTION  : View model for displaying inventory items in a table
*                Contains filtered list of items for the current user
*/

using System.Collections.Generic;

namespace A04_MVC.Models
{
    /// <summary>
    /// View model for the item list display
    /// </summary>
    public class ItemListViewModel
    {
        /// <summary>
        /// List of inventory items to display
        /// </summary>
        public List<ItemCatalog> Items { get; set; } = new List<ItemCatalog>();

        /// <summary>
        /// Current logged-in user ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Search filter text
        /// </summary>
        public string? SearchTerm { get; set; }
    }
}
