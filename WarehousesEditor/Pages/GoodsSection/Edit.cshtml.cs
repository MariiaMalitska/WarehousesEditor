using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WarehousesEditor.Models;

namespace WarehousesEditor.Pages.GoodsSection
{
    public class EditModel : PageModel
    {
        private readonly WarehousesEditor.Models.WarehouseDbContext _context;

        public EditModel(WarehousesEditor.Models.WarehouseDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Goods Goods { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Goods = await _context.Goods
                .Include(g => g.Currency).Include(g => g.GoodsCategories).FirstOrDefaultAsync(m => m.GoodsId == id);

            if (Goods == null)
            {
                return NotFound();
            }

            var categories = _context.Categories.Select(c =>
                                  new {
                                      c.CategoryId,
                                      c.CategoryName
                                  }).ToList();

            ViewData["Categories"] = new MultiSelectList(categories, "CategoryId", "CategoryName", Goods.GoodsCategories.Select(x => x.CategoryId).ToArray());
            ViewData["CurrencyId"] = new SelectList(_context.Currencies, "CurrencyId", "Code");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var temp = await _context.Goods.FirstOrDefaultAsync(g => g.GoodsName == Goods.GoodsName);

            if (temp != null && temp.GoodsId != Goods.GoodsId)
            {
                ModelState.AddModelError("GoodsName", "These goods already exist");
                return await OnGetAsync(Goods.GoodsId);
            }

            _context.Attach(Goods).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GoodsExists(Goods.GoodsId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // updating categories
            _context.GoodsCategories.RemoveRange(_context.GoodsCategories.Where(x=>x.GoodsId==Goods.GoodsId));
            var categories = Request.Form["Categories"];
            foreach (var categoryId in categories)
            {
                _context.GoodsCategories.Add(new GoodsCategory() { CategoryId = int.Parse(categoryId), GoodsId = Goods.GoodsId });
            }
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private bool GoodsExists(int id)
        {
            return _context.Goods.Any(e => e.GoodsId == id);
        }
    }
}
