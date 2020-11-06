using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehousesEditor.Models;

namespace WarehousesEditor.Pages.GoodsSection
{
    public class DeleteModel : PageModel
    {
        private readonly WarehousesEditor.Models.WarehouseDbContext _context;

        public DeleteModel(WarehousesEditor.Models.WarehouseDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Goods Goods { get; set; }
        public IList<GoodsCategory> GoodsCategories { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Goods = await _context.Goods
                .Include(g => g.Currency).Include(g=>g.GoodsCategories).FirstOrDefaultAsync(m => m.GoodsId == id);

            if (Goods == null)
            {
                return NotFound();
            }

            GoodsCategories = await _context.GoodsCategories.Include(g => g.Category).
                Where(x => x.GoodsId == Goods.GoodsId).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Goods = await _context.Goods.FindAsync(id);

            if (Goods != null)
            {
                _context.Goods.Remove(Goods);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }

        public List<GoodsCategory> GetCategories(int goodsId)
        {
            return GoodsCategories.Where(x => x.GoodsId == goodsId).ToList();
        }
    }
}
