using HostelMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HostelMS.ViewModels
{
    public class BookingCreateViewModel
    {
        // Selected hostel information
        public int HostelId { get; set; }
        public string HostelName { get; set; } = string.Empty;
        
        // Room selection
        [Required(ErrorMessage = "Please select a room")]
        [Display(Name = "Select Room")]
        public int RoomId { get; set; }
        
        public List<SelectListItem> AvailableRooms { get; set; } = new List<SelectListItem>();
        
        // Booking information
        [Display(Name = "Check-in Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Check-in date is required")]
        public DateTime CheckInDate { get; set; }
        
        [Display(Name = "Check-out Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Check-out date is required")]
        public DateTime CheckOutDate { get; set; }
        
        [Display(Name = "Additional Comments")]
        public string? Comments { get; set; }
        
        // Payment information
        [Required(ErrorMessage = "Payment amount is required")]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Display(Name = "Payment Amount (UGX)")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        
        [Required(ErrorMessage = "Payment method is required")]
        [Display(Name = "Payment Method")]
        public PaymentMethod PaymentMethod { get; set; }
        
        [Required(ErrorMessage = "Transaction reference is required")]
        [Display(Name = "Transaction Reference/ID")]
        public string TransactionReference { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Payment proof is required")]
        [Display(Name = "Proof of Payment")]
        public IFormFile? PaymentProof { get; set; }
        
        // User ID (automatically set)
        public string UserId { get; set; } = string.Empty;
    }
}