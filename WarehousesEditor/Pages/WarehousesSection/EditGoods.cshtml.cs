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
    public class EditGoodsModel : PageModel
    {
        private readonly WarehousesEditor.Models.WarehouseDbContext _context;

        public EditGoodsModel(WarehousesEditor.Models.WarehouseDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public WarehouseGoods WarehouseGoods { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, int? wid)
        {
            if (id == null || wid==null)
            {
                return NotFound();
            }

            WarehouseGoods = await _context.WarehousesGoods
                .Include(w => w.Goods)
                .Include(w => w.Warehouse).FirstOrDefaultAsync(m => m.WarehouseId == wid && m.GoodsId==id);

            if (WarehouseGoods == null)
            {
                return NotFound();
            }

           ViewData["GoodsId"] = new SelectList(_context.Goods, "GoodsId", "BarcodeNumber");
           ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "WarehouseId", "Address");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(WarehouseGoods).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WarehouseGoodsExists(WarehouseGoods.WarehouseId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Warehouse", new { id = WarehouseGoods.WarehouseId } );
        }

        private bool WarehouseGoodsExists(int id)
        {
            return _context.WarehousesGoods.Any(e => e.WarehouseId == id);
        }
    }
}
