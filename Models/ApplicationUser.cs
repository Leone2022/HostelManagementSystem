// Path: Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace HostelMS.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            MaintenanceRequests = new HashSet<MaintenanceRequest>();
            // Initialize non-nullable string properties
            FirstName = string.Empty;
            LastName = string.Empty;
        }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string? UserRole { get; set; } // Admin, Dean, Warden, Student

        // Enhanced Student-specific fields
        public string? StudentId { get; set; }
        public string? Course { get; set; }
        public string? Year { get; set; }
        public string? ParentName { get; set; }
        public string? ParentContact { get; set; }
        public string? Address { get; set; }
        public string? Nationality { get; set; }
        public string? Country { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? ProfilePicture { get; set; }
        public string? IdentificationType { get; set; }
        public string? IdentificationNumber { get; set; }

        // New fields for accommodation tracking
        public int? CurrentHostelId { get; set; } // Changed from string to int?
        public string? CurrentRoomNumber { get; set; }
        public bool IsVerified { get; set; } = false;
        public DateTime? VerificationDate { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public bool IsTemporaryAssignment { get; set; } = true;
        public DateTime? ProbationEndDate { get; set; }
        public bool IsBoarding { get; set; } = true; // To track boarding vs day status
        public DateTime? LastCheckInTime { get; set; }
        public DateTime? LastCheckOutTime { get; set; }
        public bool IsCurrentlyInHostel { get; set; } = false;

        // Account approval properties
        public bool IsApproved { get; set; } = false;
        public DateTime? RegistrationDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string? ApprovedBy { get; set; }

        // Foreign key relationships
        [ForeignKey("CurrentHostelId")]
        public virtual Hostel? CurrentHostel { get; set; }

        // Navigation property
        public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; }
    }
}