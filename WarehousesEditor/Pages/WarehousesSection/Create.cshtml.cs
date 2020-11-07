using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WarehousesEditor.Models;

namespace WarehousesEditor.Pages.WarehousesSection
{
    public class CreateModel : PageModel
    {
        private readonly WarehousesEditor.Models.WarehouseDbContext _context;

        public CreateModel(WarehousesEditor.Models.WarehouseDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Warehouse Warehouse { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var temp = await _context.Warehouses.FirstOrDefaultAsync(g => g.Address == Warehouse.Address || g.WarehouseName == Warehouse.WarehouseName);

            if (temp != null)
            {
                ModelState.AddModelError("WarehouseName", "This warehouse already exists");
                return OnGet();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Warehouses.Add(Warehouse);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
