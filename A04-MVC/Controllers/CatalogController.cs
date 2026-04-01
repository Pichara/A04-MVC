/*
* FILE         : CatalogController.cs
* PROJECT      : A04-MVC
* PROGRAMMER   : Rodrigo Pichara Gomes
* FIRST VERSION: 2026-04-01
* DESCRIPTION  : Controller for inventory catalog CRUD operations
*                Implements multi-threaded processing for catalog operations
*                User-specific data isolation through authorization checks
*/

using A04_MVC.Data;
using A04_MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace A04_MVC.Controllers
{
    /// <summary>
    /// Controller for managing user's inventory catalog
    /// All operations are scoped to the authenticated user's data
    /// Uses Tasks for asynchronous, multi-threaded operations
    /// </summary>
    [Authorize]
    public class CatalogController : Controller
    {
        /// <summary>
        /// Database context for data access
        /// </summary>
        private readonly MvcDbContext _context;

        /// <summary>
        /// Logger for tracking catalog operations
        /// </summary>
        private readonly ILogger<CatalogController> _logger;

        /// <summary>
        /// Initializes a new instance of CatalogController
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="logger">Logger instance.</param>
        public CatalogController(MvcDbContext context, ILogger<CatalogController> logger)
        {
            _context = context;
            _logger = logger;
            return;
        }

        /// <summary>
        /// Gets current user's ID from claims
        /// </summary>
        /// <returns>User ID if authenticated, 0 otherwise.</returns>
        private long GetCurrentUserId()
        {
            long result = 0;

            if (User.Identity?.IsAuthenticated == true)
            {
                string? userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (long.TryParse(userIdString, out long userId))
                {
                    result = userId;
                }
            }

            return result;
        }

        /// <summary>
        /// Displays the user's inventory catalog
        /// </summary>
        /// <returns>Catalog index view with user's items.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IActionResult result;

            long userId = GetCurrentUserId();

            if (userId == 0)
            {
                result = RedirectToAction("Index", "Login");
            }
            else
            {
                // Asynchronously fetch user's items with category data
                List<ItemCatalog> items = await _context.ItemCatalogs
                    .Where(i => i.UserId == userId)
                    .Include(i => i.Category)
                    .OrderBy(i => i.Item)
                    .ToListAsync();

                ItemListViewModel model = new ItemListViewModel
                {
                    Items = items,
                    UserId = userId
                };

                result = View(model);
            }

            return result;
        }

        /// <summary>
        /// Displays form to add a new item
        /// </summary>
        /// <returns>Create item view with category dropdown.</returns>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            IActionResult result;

            long userId = GetCurrentUserId();

            if (userId == 0)
            {
                result = RedirectToAction("Index", "Login");
            }
            else
            {
                // Asynchronously fetch categories for dropdown
                List<Category> categories = await _context.Categories
                    .OrderBy(c => c.CategoryName)
                    .ToListAsync();

                ItemCreateViewModel model = new ItemCreateViewModel
                {
                    Categories = categories.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName
                    }).ToList()
                };

                result = View(model);
            }

            return result;
        }

        /// <summary>
        /// Processes new item creation
        /// </summary>
        /// <param name="model">Item create view model.</param>
        /// <returns>Redirect to catalog on success, back to form on failure.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemCreateViewModel model)
        {
            IActionResult result;

            long userId = GetCurrentUserId();

            if (userId == 0)
            {
                result = RedirectToAction("Index", "Login");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    ItemCatalog newItem = new ItemCatalog
                    {
                        UserId = userId,
                        Item = model.Item,
                        Price = model.Price,
                        DateBought = model.DateBought,
                        CategoryId = model.CategoryId
                    };

                    // Add to database asynchronously
                    _context.ItemCatalogs.Add(newItem);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("User {UserId} created item {Item}", userId, model.Item);

                    result = RedirectToAction("Index");
                }
                else
                {
                    // Repopulate categories on validation failure
                    List<Category> categories = await _context.Categories
                        .OrderBy(c => c.CategoryName)
                        .ToListAsync();

                    model.Categories = categories.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName
                    }).ToList();

                    result = View(model);
                }
            }

            return result;
        }

        /// <summary>
        /// Displays form to edit an existing item
        /// </summary>
        /// <param name="id">Composite key identifier (userId|item|categoryId encoded).</param>
        /// <returns>Edit item view if authorized, unauthorized otherwise.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            IActionResult result;

            long userId = GetCurrentUserId();

            if (userId == 0)
            {
                result = RedirectToAction("Index", "Login");
            }
            else
            {
                // Parse encoded ID (format: userId|item|categoryId)
                string[] parts = id.Split('|');
                if (parts.Length != 3)
                {
                    result = NotFound();
                }
                else
                {
                    long itemUserId = long.Parse(parts[0]);
                    string itemName = parts[1];
                    int categoryId = int.Parse(parts[2]);

                    // Verify ownership and fetch item
                    ItemCatalog? item = await _context.ItemCatalogs
                        .Include(i => i.Category)
                        .FirstOrDefaultAsync(i =>
                            i.UserId == itemUserId &&
                            i.Item == itemName &&
                            i.CategoryId == categoryId);

                    if (item == null || item.UserId != userId)
                    {
                        result = Forbid();
                    }
                    else
                    {
                        // Get categories for dropdown
                        List<Category> categories = await _context.Categories
                            .OrderBy(c => c.CategoryName)
                            .ToListAsync();

                        ItemCreateViewModel model = new ItemCreateViewModel
                        {
                            Item = item.Item,
                            Price = item.Price,
                            DateBought = item.DateBought,
                            CategoryId = item.CategoryId,
                            Categories = categories.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                            {
                                Value = c.CategoryId.ToString(),
                                Text = c.CategoryName
                            }).ToList()
                        };

                        result = View(model);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Processes item update
        /// </summary>
        /// <param name="id">Original item identifier.</param>
        /// <param name="model">Updated item data.</param>
        /// <returns>Redirect to catalog on success.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ItemCreateViewModel model)
        {
            IActionResult result;

            long userId = GetCurrentUserId();

            if (userId == 0)
            {
                result = RedirectToAction("Index", "Login");
            }
            else
            {
                // Parse original ID
                string[] parts = id.Split('|');
                if (parts.Length != 3)
                {
                    result = NotFound();
                }
                else
                {
                    long itemUserId = long.Parse(parts[0]);
                    string itemName = parts[1];
                    int categoryId = int.Parse(parts[2]);

                    if (ModelState.IsValid)
                    {
                        // Find original item
                        ItemCatalog? item = await _context.ItemCatalogs
                            .FirstOrDefaultAsync(i =>
                                i.UserId == itemUserId &&
                                i.Item == itemName &&
                                i.CategoryId == categoryId);

                        if (item == null || item.UserId != userId)
                        {
                            result = Forbid();
                        }
                        else
                        {
                            // Update item (requires delete and insert for composite key change)
                            _context.ItemCatalogs.Remove(item);

                            ItemCatalog updatedItem = new ItemCatalog
                            {
                                UserId = userId,
                                Item = model.Item,
                                Price = model.Price,
                                DateBought = model.DateBought,
                                CategoryId = model.CategoryId
                            };

                            _context.ItemCatalogs.Add(updatedItem);
                            await _context.SaveChangesAsync();

                            _logger.LogInformation("User {UserId} updated item {OldItem} to {NewItem}", userId, itemName, model.Item);

                            result = RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        // Repopulate categories on validation failure
                        List<Category> categories = await _context.Categories
                            .OrderBy(c => c.CategoryName)
                            .ToListAsync();

                        model.Categories = categories.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                        {
                            Value = c.CategoryId.ToString(),
                            Text = c.CategoryName
                        }).ToList();

                        result = View(model);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Displays delete confirmation page
        /// </summary>
        /// <param name="id">Item identifier.</param>
        /// <returns>Delete confirmation view.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            IActionResult result;

            long userId = GetCurrentUserId();

            if (userId == 0)
            {
                result = RedirectToAction("Index", "Login");
            }
            else
            {
                string[] parts = id.Split('|');
                if (parts.Length != 3)
                {
                    result = NotFound();
                }
                else
                {
                    long itemUserId = long.Parse(parts[0]);
                    string itemName = parts[1];
                    int categoryId = int.Parse(parts[2]);

                    ItemCatalog? item = await _context.ItemCatalogs
                        .Include(i => i.Category)
                        .FirstOrDefaultAsync(i =>
                            i.UserId == itemUserId &&
                            i.Item == itemName &&
                            i.CategoryId == categoryId);

                    if (item == null || item.UserId != userId)
                    {
                        result = Forbid();
                    }
                    else
                    {
                        result = View(item);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Processes item deletion
        /// </summary>
        /// <param name="id">Item identifier.</param>
        /// <returns>Redirect to catalog.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            IActionResult result;

            long userId = GetCurrentUserId();

            if (userId == 0)
            {
                result = RedirectToAction("Index", "Login");
            }
            else
            {
                string[] parts = id.Split('|');
                if (parts.Length != 3)
                {
                    result = NotFound();
                }
                else
                {
                    long itemUserId = long.Parse(parts[0]);
                    string itemName = parts[1];
                    int categoryId = int.Parse(parts[2]);

                    ItemCatalog? item = await _context.ItemCatalogs
                        .FirstOrDefaultAsync(i =>
                            i.UserId == itemUserId &&
                            i.Item == itemName &&
                            i.CategoryId == categoryId);

                    if (item == null || item.UserId != userId)
                    {
                        result = Forbid();
                    }
                    else
                    {
                        string deletedItemName = item.Item;
                        _context.ItemCatalogs.Remove(item);
                        await _context.SaveChangesAsync();

                        _logger.LogInformation("User {UserId} deleted item {Item}", userId, deletedItemName);

                        result = RedirectToAction("Index");
                    }
                }
            }

            return result;
        }
    }
}
