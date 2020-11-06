using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WarehousesEditor.Models
{
    public partial class Currency
    {
        public Currency()
        {
            Goods = new HashSet<Goods>();
        }

        public int CurrencyId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The name must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "Name")]
        public string CurrencyName { get; set; }

        [Required]
        [StringLength(3, ErrorMessage = "The code must be {1} characters long.", MinimumLength = 3)]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:0.00000}", ApplyFormatInEditMode = true)]
        [Display(Name = "Rate")]
        public decimal Rate { get; set; }

        [Display(Name = "Last Updated")]
        public DateTime DateUpdated { get; set; }

        public virtual ICollection<Goods> Goods { get; set; }
    }
}
