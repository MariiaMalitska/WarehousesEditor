using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLitePCL;
using WarehousesEditor.Helpers;
using WarehousesEditor.Models;

namespace WarehousesEditor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly WarehousesEditor.Models.WarehouseDbContext _context;
        private readonly CurrencySynchronizer _synchronizer;

        public IndexModel(ILogger<IndexModel> logger, WarehousesEditor.Models.WarehouseDbContext context, CurrencySynchronizer synchronizer)
        {
            _logger = logger;
            _context = context;
            _synchronizer = synchronizer;
        }

        public DateTime LastUpdated { get; set; }

        public async Task<IActionResult> OnGet()
        {
            if ((LastUpdated = _context.Currencies.FirstOrDefault().DateUpdated) == null)
            {
                _context.Currencies.Add(new Currency() { CurrencyName = "United States dollar", Code = "USD", DateUpdated = DateTime.Now, Rate=1 });
                _context.Currencies.Add(new Currency() { CurrencyName = "Ukrainian hryvnia", Code = "UAH", DateUpdated = DateTime.Now, Rate = 1 });
                _context.Currencies.Add(new Currency() { CurrencyName = "European euro", Code = "EUR", DateUpdated = DateTime.Now, Rate = 1 });
                await _context.SaveChangesAsync();

                await _synchronizer.SynchronizeCurrencies();
                LastUpdated = DateTime.Now;
            }
            //LastUpdated = _context.Currencies.FirstOrDefault().DateUpdated.ToString();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _synchronizer.SynchronizeCurrencies();
                LastUpdated = DateTime.Now;
            }
            catch(Exception e)
            {
                _logger.LogError("Unable to synchronize currencies: " + e.Message);
            }
            return Page();
        }

        
    }
}
