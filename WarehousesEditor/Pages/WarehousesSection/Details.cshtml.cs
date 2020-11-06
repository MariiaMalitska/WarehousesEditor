using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WarehousesEditor.Models;

namespace WarehousesEditor.Pages.WarehousesSection
{
    public class DetailsModel : PageModel
    {
        private readonly WarehousesEditor.Models.WarehouseDbContext _context;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(WarehousesEditor.Models.WarehouseDbContext context, ILogger<DetailsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public int CurrencyId { get; set; }

        [BindProperty]
        public decimal Sum { get; set; }

        [BindProperty]
        public Warehouse Warehouse { get; set; }

        [BindProperty]
        public IList<WarehouseGoods> WarehouseGoods { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Warehouse = await _context.Warehouses.Include(x=>x.WarehousesGoods).FirstOrDefaultAsync(m => m.WarehouseId == id);

            if (Warehouse == null)
            {
                return NotFound();
            }

            WarehouseGoods = await _context.WarehousesGoods.Include(x=>x.Goods).
                Where(x => x.WarehouseId == Warehouse.WarehouseId).OrderBy(x=>x.Goods.GoodsName).ToListAsync();

            ViewData["CurrencyId"] = new SelectList(_context.Currencies, "CurrencyId", "Code");
            Sum = 0;
            CurrencyId = 1;

            return Page();
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

        public decimal GetSum()
        {
            var currency = _context.Currencies.FirstOrDefault(x => x.CurrencyId == CurrencyId);
            decimal sum = 0;
            foreach(var wg in WarehouseGoods)
            {
                sum += (wg.Amount*wg.Goods.BaseCurrencyPrice*currency.Rate);
            }
            Sum = sum;
            return sum;
        }
    }
}
