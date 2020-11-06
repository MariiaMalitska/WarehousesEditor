using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehousesEditor.Models;

namespace WarehousesEditor.Pages.WarehousesSection
{
    public class DeleteModel : PageModel
    {
        private readonly WarehousesEditor.Models.WarehouseDbContext _context;

        public DeleteModel(WarehousesEditor.Models.WarehouseDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Warehouse Warehouse { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Warehouse = await _context.Warehouses.FirstOrDefaultAsync(m => m.WarehouseId == id);

            if (Warehouse == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Warehouse = await _context.Warehouses.FindAsync(id);

            if (Warehouse != null)
            {
                _context.Warehouses.Remove(Warehouse);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
