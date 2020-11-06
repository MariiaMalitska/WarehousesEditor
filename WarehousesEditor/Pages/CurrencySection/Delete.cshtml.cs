using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehousesEditor.Models;

namespace WarehousesEditor.Pages.CurrencySection
{
    public class DeleteModel : PageModel
    {
        private readonly WarehousesEditor.Models.WarehouseDbContext _context;

        public DeleteModel(WarehousesEditor.Models.WarehouseDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Currency Currency { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Currency = await _context.Currencies.FirstOrDefaultAsync(m => m.CurrencyId == id);

            if (Currency == null)
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

            Currency = await _context.Currencies.FindAsync(id);

            if (Currency != null)
            {
                _context.Currencies.Remove(Currency);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
