using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HostelMS.Models
{
    public class Booking
    {
        public Booking()
        {
            // Initialize collections - will uncomment once all classes are defined
            // Payments = new HashSet<Payment>();
        }

        [Key]
        public int BookingId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int RoomId { get; set; }

        [Required]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public string? Comments { get; set; }

        // NEW: Course property for storing student's course
        [MaxLength(100)]
        [Display(Name = "Course")]
        public string? Course { get; set; }

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public string? RejectionReason { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser? Student { get; set; }

        [ForeignKey("RoomId")]
        public virtual Room? Room { get; set; }

        // public virtual ICollection<Payment> Payments { get; set; }

        // Helper method to get course display name - Fixed to avoid circular dependency
        public string GetCourseDisplayName()
        {
            if (string.IsNullOrEmpty(Course))
                return "Not specified";

            // Static course mapping to avoid circular dependency with ViewModel
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

            return courseNames.TryGetValue(Course, out var displayName) ? displayName : Course;
        }

        // Helper method to check if course is restricted
        public bool IsCourseRestricted()
        {
            if (string.IsNullOrEmpty(Course))
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

            return restrictedCourses.Contains(Course);
        }
    }

    public enum BookingStatus
    {
        Pending,
        Approved,
        Rejected,
        CheckedIn,
        CheckedOut,
        Cancelled
    }
}