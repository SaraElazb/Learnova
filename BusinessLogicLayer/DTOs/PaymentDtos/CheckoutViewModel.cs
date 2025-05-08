using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.DTOs.PaymentDtos
{
    public class CheckoutViewModel
    {
        public int OrderId { get; set; }
        
        public decimal TotalAmount { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }
        
        [Required(ErrorMessage = "ZIP/Postal code is required")]
        public string PostalCode { get; set; }
        
        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }
        
        public string StripeToken { get; set; }
    }
} 