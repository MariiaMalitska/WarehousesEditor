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
    public class WarehouseModel : PageModel
    {
        private readonly WarehousesEditor.Models.WarehouseDbContext _context;

        public WarehouseModel(WarehousesEditor.Models.WarehouseDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Warehouse Warehouse { get; set; }

        [BindProperty]
        public IList<WarehouseGoods> WarehouseGoods { get; set; }

        [BindProperty]
        public int CurrencyId { get; set; }

        [BindProperty]
        public string Sum { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Warehouse = await _context.Warehouses.Include(x => x.WarehousesGoods).FirstOrDefaultAsync(m => m.WarehouseId == id);

            if (Warehouse == null)
            {
                return NotFound();
            }

            WarehouseGoods = await _context.WarehousesGoods.Include(x => x.Goods).
                Where(x => x.WarehouseId == Warehouse.WarehouseId).OrderBy(x => x.Goods.GoodsName).ToListAsync();

            ViewData["CurrencyId"] = new SelectList(_context.Currencies, "CurrencyId", "Code");
            Sum = "";
            CurrencyId = 1;

            return Page();
        }

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Page();
        //    }

        //    _context.Attach(Warehouse).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!WarehouseExists(Warehouse.WarehouseId))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return RedirectToPage("./Index");
        //}

        private bool WarehouseExists(int id)
        {
            return _context.Warehouses.Any(e => e.WarehouseId == id);
        }
        public async Task<IActionResult> OnPostDeleteGoodsAsync(int id, int wid)
        {
            var warehouseGoods = await _context.WarehousesGoods.FirstOrDefaultAsync(w => w.GoodsId == id && w.WarehouseId == wid);

            if (warehouseGoods != null)
            {
                _context.WarehousesGoods.Remove(warehouseGoods);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Details", new { id = wid });
        }

        public string GetCurrencyCode(int currencyId)
        {
            return _context.Currencies.FirstOrDefault(x => x.CurrencyId == currencyId).Code;
        }

        public async Task<IActionResult> OnPostGetSumAsync()
        {
            var currency = await _context.Currencies.FirstOrDefaultAsync(x => x.CurrencyId == CurrencyId);
            var warehouseGoods = await _context.WarehousesGoods.Include(x => x.Goods).
                Where(x => x.WarehouseId == Warehouse.WarehouseId).OrderBy(x => x.Goods.GoodsName).ToListAsync();
            decimal sum = 0;
            foreach (var wg in warehouseGoods)
            {
                sum += (wg.Amount * wg.Goods.BaseCurrencyPrice * currency.Rate);
            }
            Sum = "Sum in "+currency.CurrencyName+" is: "+sum.ToString();

            Warehouse = await _context.Warehouses.Include(x => x.WarehousesGoods).FirstOrDefaultAsync(m => m.WarehouseId == Warehouse.WarehouseId);

            WarehouseGoods = await _context.WarehousesGoods.Include(x => x.Goods).
                Where(x => x.WarehouseId == Warehouse.WarehouseId).OrderBy(x => x.Goods.GoodsName).ToListAsync();

            ViewData["CurrencyId"] = new SelectList(_context.Currencies, "CurrencyId", "Code");

            return Page();
        }
    }
}
