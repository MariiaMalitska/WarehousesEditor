using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WarehousesEditor.Helpers;
using WarehousesEditor.Models;

namespace WarehousesEditor.Pages.CurrencySection
{
    public class CreateModel : PageModel
    {
        private readonly ILogger<CreateModel> _logger;
        private readonly WarehousesEditor.Models.WarehouseDbContext _context;
        private readonly CurrencySynchronizer _synchronizer;

        public CreateModel(ILogger<CreateModel> logger, WarehousesEditor.Models.WarehouseDbContext context, CurrencySynchronizer synchronizer)
        {
            _logger = logger;
            _context = context;
            _synchronizer = synchronizer;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Currency Currency { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var temp = await _context.Currencies.FirstOrDefaultAsync(g => g.Code == Currency.Code || g.CurrencyName == Currency.CurrencyName);

            if (temp != null)
            {
                ModelState.AddModelError("Code", "This currency already exists");
                return OnGet();
            }

            Currency.Code = Currency.Code.ToUpper();

            // automatic rate
            try
            {
                var rate = await _synchronizer.GetRateToUah(Currency.Code);
                var coef = await _synchronizer.GetCoef();
                Currency.Rate = decimal.Parse(coef)/decimal.Parse(rate);
                Currency.DateUpdated = DateTime.Now;
                _context.Currencies.Add(Currency);
                await _context.SaveChangesAsync();               
            }
            catch(Exception e)
            {
                ModelState.AddModelError("Code", "Can't get rate for current currency code.");
                _logger.LogError("Couldn't get rate: " + e.Message);

                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
