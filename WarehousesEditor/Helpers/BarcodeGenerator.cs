using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehousesEditor.Helpers
{
    public class BarcodeGenerator
    {
        private Random random = new Random();
        private int _length;

        public BarcodeGenerator(int length)
        {
            _length = length;
        }

        public string GenerateBarcode()
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, _length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
