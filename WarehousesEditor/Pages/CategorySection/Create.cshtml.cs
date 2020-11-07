using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WarehousesEditor.Models;

namespace WarehousesEditor.Pages.CategorySection
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
        public Category Category { get; set; }

        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var temp = await _context.Categories.FirstOrDefaultAsync(g => g.CategoryName == Category.CategoryName);

            if (temp != null)
            {
                ModelState.AddModelError("CategoryName", "This category already exists");
                return OnGet();
            }

            _context.Categories.Add(Category);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
