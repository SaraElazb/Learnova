using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.PaymentDtos
{
    public class PaymentModel
    {
        public decimal Amount { get; set; } 
        public string Currency { get; set; } 
        public string Description { get; set; } 
        public string StripeToken { get; set; } 
    }
}
