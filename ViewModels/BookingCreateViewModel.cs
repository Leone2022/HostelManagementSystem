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

        // Course selection - ADDED FOR COURSE RESTRICTION FUNCTIONALITY
        [Required(ErrorMessage = "Please select your course")]
        [Display(Name = "Course")]
        public string CourseSelection { get; set; } = string.Empty;
        
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

        // ADDED: Helper method to check if the selected course is restricted
        public bool IsRestrictedCourse()
        {
            if (string.IsNullOrEmpty(CourseSelection))
                return false;

            var restrictedCourses = new HashSet<string>
            {
                // ALL NURSING COURSES
                "BSc_Nursing", "Dip_Nursing", "Cert_Nursing",
                
                // ALL CERTIFICATE PROGRAMS
                "Cert_IT", "Cert_Childhood", "Cert_Networks", 
                "Cert_Office", "Cert_Repair",
                
                // BRIDGING COURSES
                "HEC_Sciences"
            };

            return restrictedCourses.Contains(CourseSelection);
        }

        // ADDED: Helper method to get course display name
        public string GetCourseDisplayName()
        {
            if (string.IsNullOrEmpty(CourseSelection))
                return "Not specified";

            var courseNames = new Dictionary<string, string>
            {
                // UNDERGRADUATE DEGREE PROGRAMS
                // School of Business
                { "BBA_Accounting", "Bachelor of Business Administration in Accounting" },
                { "BBA_Finance", "Bachelor of Business Administration in Finance" },
                { "BBA_Insurance", "Bachelor of Business Administration in Insurance" },
                { "BBA_Marketing", "Bachelor of Business Administration in Marketing" },
                { "BBA_HRM", "Bachelor of Business Administration in Human Resource Management" },
                { "BBA_Economics", "Bachelor of Business Administration in Economics" },
                { "BBA_Entrepreneurship", "Bachelor of Business Administration in Entrepreneurship" },
                { "BBA_Management", "Bachelor of Business Administration in Management" },
                { "BBA_Secretarial", "Bachelor of Business Administration in Secretarial Studies & Office Management" },
                { "BBA_BIS", "Bachelor of Business Administration in Business Information Systems" },
                { "BBA_Trade", "Bachelor of Business Administration in International Trade" },
                
                // School of Computing and Informatics
                { "BSc_Networks", "Bachelor of Science in Computer Networks and System Administration" },
                { "BSc_Software", "Bachelor of Science in Software Engineering and Application Development" },
                { "BBA_BIS_Computing", "Bachelor of Business Administration in Business Information Systems" },
                
                // School of Social Sciences
                { "BA_Development", "Bachelor of Arts in Development Studies" },
                { "BPA_Management", "Bachelor of Public Administration and Management" },
                { "BA_SocialWork", "Bachelor of Arts in Social Work and Social Administration" },
                { "BSc_Counseling", "Bachelor of Science in Counseling and Psychology" },
                { "BSA_Sociology", "Bachelor of Social Administration and Sociology" },
                
                // School of Education
                { "BA_Education", "Bachelor of Arts with Education" },
                { "BSc_Education", "Bachelor of Science with Education" },
                
                // School of Theology
                { "B_Theology", "Bachelor of Theology" },
                { "BA_Religious", "Bachelor of Arts in Religious Studies with Chaplaincy" },
                { "B_Development_Ministry", "Bachelor of Development Ministry" },
                { "B_Evangelism", "Bachelor of Evangelism and Church Growth" },
                { "B_Biblical_Counseling", "Bachelor of Biblical Counseling" },
                
                // School of Health Sciences
                { "BSc_Nursing", "Bachelor of Nursing Science" }, // RESTRICTED
                { "BSc_Food", "Bachelor of Science in Food Technology and Human Nutrition" },
                
                // School of Agricultural and Environmental Sciences
                { "BSc_Agriculture", "Bachelor of Science in Agriculture" },
                { "BSc_Agribusiness", "Bachelor of Science in Agribusiness Innovation and Management" },
                { "BSc_Environmental", "Bachelor of Science in Environmental Science" },
                { "BSc_Statistics", "Bachelor of Science in Statistics" },
                { "BSc_Biochemistry", "Bachelor of Science in Biochemistry" },
                
                // POSTGRADUATE DEGREE PROGRAMS
                // Master's Programs
                { "MBA", "Master of Business Administration" },
                { "MA_Education", "Master of Arts in Education Management" },
                { "MA_Development", "Master of Arts in Development Studies" },
                { "MA_English", "Master of Arts in English Literature" },
                { "MSc_Counseling", "Master of Science in Counseling Psychology" },
                { "MPH", "Master of Public Health" },
                { "MSc_IS", "Master of Science in Information Systems" },
                { "MSc_Software_Masters", "Master of Science in Software Engineering and Application Development" },
                { "MSc_Security", "Master of Science in Network Security" },
                { "MSc_Rural", "Master of Science in Rural Development" },
                { "M_LocalGov", "Master in Local Government Management" },
                { "MIT_Management", "Master of Information Technology and Management" },
                { "MSc_Palliative", "Master of Science in Palliative Care" },
                { "MIS", "Master of Information Science" },
                
                // Doctoral Programs
                { "PhD_DevEducation", "Doctor of Philosophy in Developmental Education" },
                { "PhD_Rural", "Doctor of Philosophy in Rural Development" },
                { "PhD_Communication", "Doctor of Philosophy in Developmental Communication" },
                { "PhD_EducationMgmt", "PhD in Educational Management" },
                { "PhD_Environmental", "PhD in Environmental Management" },
                
                // DIPLOMA PROGRAMS
                { "Dip_BA_Accounting", "Diploma in Business Administration in Accounting" },
                { "Dip_Secretarial", "Diploma in Secretarial Studies & Office Management" },
                { "Dip_Development", "Diploma in Development Studies" },
                { "Dip_Education", "Diploma in Education" },
                { "Dip_Nursing", "Diploma in Nursing" }, // RESTRICTED
                { "Dip_Food", "Diploma in Food Science & Processing Technology" },
                { "Dip_Biomedical", "Diploma in Biomedical Engineering & Technology" },
                { "Dip_Lab", "Diploma in Science Laboratory Technology" },
                { "Dip_Forensics", "Diploma in Computer Forensics" },
                { "Dip_SocialWork", "Diploma in Social Work and Social Administration" },
                { "Dip_Counseling", "Diploma in Counseling" },
                
                // CERTIFICATE PROGRAMS (ALL RESTRICTED)
                { "Cert_IT", "Certificate in Information Technology" }, // RESTRICTED
                { "Cert_Nursing", "Certificate in Nursing Program" }, // RESTRICTED
                { "Cert_Childhood", "Certificate in Early Childhood Education" }, // RESTRICTED
                { "Cert_Networks", "Certificate in Small Business Computer Networks" }, // RESTRICTED
                { "Cert_Office", "Certificate in Office Automation and Data Management" }, // RESTRICTED
                { "Cert_Repair", "Certificate in Computer Repair and Maintenance" }, // RESTRICTED
                
                // BRIDGING COURSES (ALL RESTRICTED)
                { "HEC_Sciences", "Higher Education Certificate (Biological and Physical Sciences)" } // RESTRICTED
            };

            return courseNames.TryGetValue(CourseSelection, out var displayName) ? displayName : CourseSelection;
        }

        // ADDED: Helper method to get restriction message
        public string GetRestrictionMessage()
        {
            return "Unfortunately, you cannot book accommodation through this system. Under this course, you are required to stay in the school premises. Please visit the Office of the Dean of Students to be assigned a room.";
        }

        // ADDED: Helper method to get all available courses for dropdown
        public static List<SelectListItem> GetAllCourses()
        {
            return new List<SelectListItem>
            {
                // Default option
                new SelectListItem { Value = "", Text = "-- Select Your Course --" },
                
                // UNDERGRADUATE DEGREE PROGRAMS
                // School of Business
                new SelectListItem { Value = "BBA_Accounting", Text = "Bachelor of Business Administration in Accounting", Group = new SelectListGroup { Name = "School of Business" } },
                new SelectListItem { Value = "BBA_Finance", Text = "Bachelor of Business Administration in Finance", Group = new SelectListGroup { Name = "School of Business" } },
                new SelectListItem { Value = "BBA_Insurance", Text = "Bachelor of Business Administration in Insurance", Group = new SelectListGroup { Name = "School of Business" } },
                new SelectListItem { Value = "BBA_Marketing", Text = "Bachelor of Business Administration in Marketing", Group = new SelectListGroup { Name = "School of Business" } },
                new SelectListItem { Value = "BBA_HRM", Text = "Bachelor of Business Administration in Human Resource Management", Group = new SelectListGroup { Name = "School of Business" } },
                new SelectListItem { Value = "BBA_Economics", Text = "Bachelor of Business Administration in Economics", Group = new SelectListGroup { Name = "School of Business" } },
                new SelectListItem { Value = "BBA_Entrepreneurship", Text = "Bachelor of Business Administration in Entrepreneurship", Group = new SelectListGroup { Name = "School of Business" } },
                new SelectListItem { Value = "BBA_Management", Text = "Bachelor of Business Administration in Management", Group = new SelectListGroup { Name = "School of Business" } },
                new SelectListItem { Value = "BBA_Secretarial", Text = "Bachelor of Business Administration in Secretarial Studies & Office Management", Group = new SelectListGroup { Name = "School of Business" } },
                new SelectListItem { Value = "BBA_BIS", Text = "Bachelor of Business Administration in Business Information Systems", Group = new SelectListGroup { Name = "School of Business" } },
                new SelectListItem { Value = "BBA_Trade", Text = "Bachelor of Business Administration in International Trade", Group = new SelectListGroup { Name = "School of Business" } },
                
                // School of Computing and Informatics
                new SelectListItem { Value = "BSc_Networks", Text = "Bachelor of Science in Computer Networks and System Administration", Group = new SelectListGroup { Name = "School of Computing and Informatics" } },
                new SelectListItem { Value = "BSc_Software", Text = "Bachelor of Science in Software Engineering and Application Development", Group = new SelectListGroup { Name = "School of Computing and Informatics" } },
                new SelectListItem { Value = "BBA_BIS_Computing", Text = "Bachelor of Business Administration in Business Information Systems", Group = new SelectListGroup { Name = "School of Computing and Informatics" } },
                
                // School of Social Sciences
                new SelectListItem { Value = "BA_Development", Text = "Bachelor of Arts in Development Studies", Group = new SelectListGroup { Name = "School of Social Sciences" } },
                new SelectListItem { Value = "BPA_Management", Text = "Bachelor of Public Administration and Management", Group = new SelectListGroup { Name = "School of Social Sciences" } },
                new SelectListItem { Value = "BA_SocialWork", Text = "Bachelor of Arts in Social Work and Social Administration", Group = new SelectListGroup { Name = "School of Social Sciences" } },
                new SelectListItem { Value = "BSc_Counseling", Text = "Bachelor of Science in Counseling and Psychology", Group = new SelectListGroup { Name = "School of Social Sciences" } },
                new SelectListItem { Value = "BSA_Sociology", Text = "Bachelor of Social Administration and Sociology", Group = new SelectListGroup { Name = "School of Social Sciences" } },
                
                // School of Education
                new SelectListItem { Value = "BA_Education", Text = "Bachelor of Arts with Education", Group = new SelectListGroup { Name = "School of Education" } },
                new SelectListItem { Value = "BSc_Education", Text = "Bachelor of Science with Education", Group = new SelectListGroup { Name = "School of Education" } },
                
                // School of Theology
                new SelectListItem { Value = "B_Theology", Text = "Bachelor of Theology", Group = new SelectListGroup { Name = "School of Theology" } },
                new SelectListItem { Value = "BA_Religious", Text = "Bachelor of Arts in Religious Studies with Chaplaincy", Group = new SelectListGroup { Name = "School of Theology" } },
                new SelectListItem { Value = "B_Development_Ministry", Text = "Bachelor of Development Ministry", Group = new SelectListGroup { Name = "School of Theology" } },
                new SelectListItem { Value = "B_Evangelism", Text = "Bachelor of Evangelism and Church Growth", Group = new SelectListGroup { Name = "School of Theology" } },
                new SelectListItem { Value = "B_Biblical_Counseling", Text = "Bachelor of Biblical Counseling", Group = new SelectListGroup { Name = "School of Theology" } },
                
                // School of Health Sciences
                new SelectListItem { Value = "BSc_Nursing", Text = "Bachelor of Nursing Science ⚠️ RESTRICTED", Group = new SelectListGroup { Name = "School of Health Sciences" } },
                new SelectListItem { Value = "BSc_Food", Text = "Bachelor of Science in Food Technology and Human Nutrition", Group = new SelectListGroup { Name = "School of Health Sciences" } },
                
                // School of Agricultural and Environmental Sciences
                new SelectListItem { Value = "BSc_Agriculture", Text = "Bachelor of Science in Agriculture", Group = new SelectListGroup { Name = "School of Agricultural and Environmental Sciences" } },
                new SelectListItem { Value = "BSc_Agribusiness", Text = "Bachelor of Science in Agribusiness Innovation and Management", Group = new SelectListGroup { Name = "School of Agricultural and Environmental Sciences" } },
                new SelectListItem { Value = "BSc_Environmental", Text = "Bachelor of Science in Environmental Science", Group = new SelectListGroup { Name = "School of Agricultural and Environmental Sciences" } },
                new SelectListItem { Value = "BSc_Statistics", Text = "Bachelor of Science in Statistics", Group = new SelectListGroup { Name = "School of Agricultural and Environmental Sciences" } },
                new SelectListItem { Value = "BSc_Biochemistry", Text = "Bachelor of Science in Biochemistry", Group = new SelectListGroup { Name = "School of Agricultural and Environmental Sciences" } },
                
                // POSTGRADUATE DEGREE PROGRAMS
                // Master's Programs
                new SelectListItem { Value = "MBA", Text = "Master of Business Administration", Group = new SelectListGroup { Name = "Master's Programs" } },
                new SelectListItem { Value = "MA_Education", Text = "Master of Arts in Education Management", Group = new SelectListGroup { Name = "Master's Programs" } },
                new SelectListItem { Value = "MA_Development", Text = "Master of Arts in Development Studies", Group = new SelectListGroup { Name = "Master's Programs" } },
                new SelectListItem { Value = "MA_English", Text = "Master of Arts in English Literature", Group = new SelectListGroup { Name = "Master's Programs" } },
                new SelectListItem { Value = "MSc_Counseling", Text = "Master of Science in Counseling Psychology", Group = new SelectListGroup { Name = "Master's Programs" } },
                new SelectListItem { Value = "MPH", Text = "Master of Public Health", Group = new SelectListGroup { Name = "Master's Programs" } },
                new SelectListItem { Value = "MSc_IS", Text = "Master of Science in Information Systems", Group = new SelectListGroup { Name = "Master's Programs" } },
                new SelectListItem { Value = "MSc_Software_Masters", Text = "Master of Science in Software Engineering and Application Development", Group = new SelectListGroup { Name = "Master's Programs" } },
                new SelectListItem { Value = "MSc_Security", Text = "Master of Science in Network Security", Group = new SelectListGroup { Name = "Master's Programs" } },
                new SelectListItem { Value = "MSc_Rural", Text = "Master of Science in Rural Development", Group = new SelectListGroup { Name = "Master's Programs" } },
                new SelectListItem { Value = "M_LocalGov", Text = "Master in Local Government Management", Group = new SelectListGroup { Name = "Master's Programs" } },
                new SelectListItem { Value = "MIT_Management", Text = "Master of Information Technology and Management", Group = new SelectListGroup { Name = "Master's Programs" } },
                new SelectListItem { Value = "MSc_Palliative", Text = "Master of Science in Palliative Care", Group = new SelectListGroup { Name = "Master's Programs" } },
                new SelectListItem { Value = "MIS", Text = "Master of Information Science", Group = new SelectListGroup { Name = "Master's Programs" } },
                
                // Doctoral Programs
                new SelectListItem { Value = "PhD_DevEducation", Text = "Doctor of Philosophy in Developmental Education", Group = new SelectListGroup { Name = "Doctoral Programs" } },
                new SelectListItem { Value = "PhD_Rural", Text = "Doctor of Philosophy in Rural Development", Group = new SelectListGroup { Name = "Doctoral Programs" } },
                new SelectListItem { Value = "PhD_Communication", Text = "Doctor of Philosophy in Developmental Communication", Group = new SelectListGroup { Name = "Doctoral Programs" } },
                new SelectListItem { Value = "PhD_EducationMgmt", Text = "PhD in Educational Management", Group = new SelectListGroup { Name = "Doctoral Programs" } },
                new SelectListItem { Value = "PhD_Environmental", Text = "PhD in Environmental Management", Group = new SelectListGroup { Name = "Doctoral Programs" } },
                
                // DIPLOMA PROGRAMS
                new SelectListItem { Value = "Dip_BA_Accounting", Text = "Diploma in Business Administration in Accounting", Group = new SelectListGroup { Name = "Diploma Programs" } },
                new SelectListItem { Value = "Dip_Secretarial", Text = "Diploma in Secretarial Studies & Office Management", Group = new SelectListGroup { Name = "Diploma Programs" } },
                new SelectListItem { Value = "Dip_Development", Text = "Diploma in Development Studies", Group = new SelectListGroup { Name = "Diploma Programs" } },
                new SelectListItem { Value = "Dip_Education", Text = "Diploma in Education", Group = new SelectListGroup { Name = "Diploma Programs" } },
                new SelectListItem { Value = "Dip_Nursing", Text = "Diploma in Nursing ⚠️ RESTRICTED", Group = new SelectListGroup { Name = "Diploma Programs" } },
                new SelectListItem { Value = "Dip_Food", Text = "Diploma in Food Science & Processing Technology", Group = new SelectListGroup { Name = "Diploma Programs" } },
                new SelectListItem { Value = "Dip_Biomedical", Text = "Diploma in Biomedical Engineering & Technology", Group = new SelectListGroup { Name = "Diploma Programs" } },
                new SelectListItem { Value = "Dip_Lab", Text = "Diploma in Science Laboratory Technology", Group = new SelectListGroup { Name = "Diploma Programs" } },
                new SelectListItem { Value = "Dip_Forensics", Text = "Diploma in Computer Forensics", Group = new SelectListGroup { Name = "Diploma Programs" } },
                new SelectListItem { Value = "Dip_SocialWork", Text = "Diploma in Social Work and Social Administration", Group = new SelectListGroup { Name = "Diploma Programs" } },
                new SelectListItem { Value = "Dip_Counseling", Text = "Diploma in Counseling", Group = new SelectListGroup { Name = "Diploma Programs" } },
                
                // CERTIFICATE PROGRAMS (ALL RESTRICTED)
                new SelectListItem { Value = "Cert_IT", Text = "Certificate in Information Technology ⚠️ RESTRICTED", Group = new SelectListGroup { Name = "Certificate Programs (RESTRICTED)" } },
                new SelectListItem { Value = "Cert_Nursing", Text = "Certificate in Nursing Program ⚠️ RESTRICTED", Group = new SelectListGroup { Name = "Certificate Programs (RESTRICTED)" } },
                new SelectListItem { Value = "Cert_Childhood", Text = "Certificate in Early Childhood Education ⚠️ RESTRICTED", Group = new SelectListGroup { Name = "Certificate Programs (RESTRICTED)" } },
                new SelectListItem { Value = "Cert_Networks", Text = "Certificate in Small Business Computer Networks ⚠️ RESTRICTED", Group = new SelectListGroup { Name = "Certificate Programs (RESTRICTED)" } },
                new SelectListItem { Value = "Cert_Office", Text = "Certificate in Office Automation and Data Management ⚠️ RESTRICTED", Group = new SelectListGroup { Name = "Certificate Programs (RESTRICTED)" } },
                new SelectListItem { Value = "Cert_Repair", Text = "Certificate in Computer Repair and Maintenance ⚠️ RESTRICTED", Group = new SelectListGroup { Name = "Certificate Programs (RESTRICTED)" } },
                
                // BRIDGING COURSES (ALL RESTRICTED)
                new SelectListItem { Value = "HEC_Sciences", Text = "Higher Education Certificate (Biological and Physical Sciences) ⚠️ RESTRICTED", Group = new SelectListGroup { Name = "Bridging Courses (RESTRICTED)" } }
            };
        }
    }
}