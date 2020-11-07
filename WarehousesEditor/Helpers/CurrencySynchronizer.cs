using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WarehousesEditor.Models;

namespace WarehousesEditor.Helpers
{
    public class CurrencySynchronizer
    {
        private readonly WarehouseDbContext _context;
        private readonly ILogger<CurrencySynchronizer> _logger;

        private readonly string BaseCurrency = "USD";

        public CurrencySynchronizer(WarehouseDbContext context, ILogger<CurrencySynchronizer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SynchronizeCurrencies()
        {
            var currencies = await _context.Currencies.ToListAsync();
            decimal? uahUsdCoef = null;
            try
            {
                var coef = await GetCoef();
                uahUsdCoef = decimal.Parse(coef);
            }
            catch (Exception e)
            {
                _logger.LogError("Coef retraction went wrong: " + e.Message);
            }

            if (uahUsdCoef != null)
            {
                foreach (var currency in currencies)
                {
                    try
                    {
                        if (currency.Code == "UAH")
                        {
                            currency.Rate = (decimal)uahUsdCoef;
                        }
                        else if (currency.Code == "USD")
                        {
                            currency.Rate = 1.0m;
                        }
                        else
                        {
                            var rate = await GetRate(currency.Code);
                            currency.Rate = (decimal)uahUsdCoef/(decimal.Parse(rate));
                        }
                        currency.DateUpdated = DateTime.Now;

                        _context.Attach(currency).State = EntityState.Modified;

                        try
                        {
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            _logger.LogError("Couldn't update db");
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Couldn't get " + currency.Code + " rate: " + e.Message);
                    }
                }
            }
        }

        public async Task<string> GetCoef()
        {
            return await GetRate(BaseCurrency);
        }

        public async Task<string> GetRate(string currencyCode)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://bank.gov.ua/");
                var response = await client.GetAsync($"NBUStatService/v1/statdirectory/exchange?valcode=" + currencyCode + "&json");
                var stringResult = await response.Content.ReadAsStringAsync();
                dynamic arr = JsonConvert.DeserializeObject(stringResult);
                string rate = arr[0].rate;
                return rate;
            }
        }
    }
}
