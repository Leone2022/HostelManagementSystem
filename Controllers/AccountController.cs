using HostelMS.Models;
using HostelMS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // For CountAsync and AnyAsync
using HostelMS.Services; // For IEmailService

namespace HostelMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailService _emailService;

        public AccountController(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Login(string userType = "student", string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string userType = "student", string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        // Check if user is of the correct type based on the login form used
                        bool isStaffLogin = userType.Equals("staff", StringComparison.OrdinalIgnoreCase);
                        bool isStaffUser = user.UserRole == "Admin" || user.UserRole == "Dean" || user.UserRole == "Warden";
                        
                        // If student tries to log in through staff portal or vice versa, show error
                        if ((isStaffLogin && !isStaffUser) || (!isStaffLogin && isStaffUser))
                        {
                            await _signInManager.SignOutAsync();
                            
                            if (isStaffLogin)
                                ModelState.AddModelError(string.Empty, "You cannot access staff portal with a student account. Please use the student login tab.");
                            else
                                ModelState.AddModelError(string.Empty, "You cannot access student portal with a staff account. Please use the staff login tab.");
                            
                            return View(model);
                        }
                        
                        // Check if student account is approved
                        if (user.UserRole == "Student" && !user.IsApproved)
                        {
                            await _signInManager.SignOutAsync();
                            ModelState.AddModelError(string.Empty, "Your account is pending approval. Please check back later or contact the administrator.");
                            return View(model);
                        }

                        // Redirect based on role
                        if (user.UserRole == "Admin")
                        {
                            // Check for pending student approvals and set notification
                            var pendingCount = await _context.Users
                                .Where(u => u.UserRole == "Student" && !u.IsApproved)
                                .CountAsync();
                                
                            if (pendingCount > 0)
                            {
                                TempData["PendingApprovals"] = pendingCount;
                            }
                            
                            return RedirectToAction("AdminDashboard", "Dashboard");
                        }
                        else if (user.UserRole == "Dean")
                        {
                            return RedirectToAction("DeanDashboard", "Dashboard");
                        }
                        else if (user.UserRole == "Warden")
                        {
                            return RedirectToAction("WardenDashboard", "Dashboard");
                        }
                        else if (user.UserRole == "Student")
                        {
                            return RedirectToAction("StudentDashboard", "Dashboard");
                        }
                    }
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your email and password.");
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                // Generate password reset token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action(
                    "ResetPassword", 
                    "Account",
                    new { email = user.Email, token = token },
                    protocol: Request.Scheme);

                // Send email with reset link
                string subject = "Reset Your Password";
                string message = $@"
                    <html>
                    <body>
                        <h2>Reset Your Password</h2>
                        <p>Please reset your password by clicking the link below:</p>
                        <p><a href='{callbackUrl}'>Reset Password</a></p>
                        <p>If you did not request a password reset, please ignore this email.</p>
                    </body>
                    </html>";

                await _emailService.SendEmailAsync(user.Email, subject, message);

                return RedirectToAction("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            var model = new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return RedirectToAction("ResetPasswordConfirmation");
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    // Log activity
                    var activity = new StudentActivity
                    {
                        UserId = user.Id,
                        UserName = $"{user.FirstName} {user.LastName}",
                        ActivityType = "Password Reset",
                        Description = "Password was reset successfully",
                        Timestamp = DateTime.Now
                    };
                    _context.StudentActivities.Add(activity);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("ResetPasswordConfirmation");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Check if email already exists
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "This email is already registered.");
                    return View(model);
                }

                // Check if student ID already exists
                var existingStudentId = await _userManager.Users.AnyAsync(u => u.StudentId == model.StudentId);
                if (existingStudentId)
                {
                    ModelState.AddModelError(string.Empty, "This Student ID is already registered.");
                    return View(model);
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    StudentId = model.StudentId,
                    Course = model.Course,
                    Year = model.Year,
                    ParentName = model.ParentName,
                    ParentContact = model.ParentContact,
                    Address = model.Address,
                    Nationality = model.Nationality,
                    Country = model.Country,
                    EmergencyContactName = model.EmergencyContactName,
                    EmergencyContactPhone = model.EmergencyContactPhone,
                    UserRole = "Student",
                    
                    // Set these fields for the approval process
                    IsApproved = false,  // New students need approval
                    RegistrationDate = DateTime.Now
                };

                // Process profile picture if uploaded
                if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
                {
                    // Create directory if it doesn't exist
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "students");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generate unique filename
                    string uniqueFileName = $"{Guid.NewGuid()}_{model.ProfilePicture.FileName}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfilePicture.CopyToAsync(fileStream);
                    }

                    // Save relative path to database
                    user.ProfilePicture = $"/images/students/{uniqueFileName}";
                }

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Assign the Student role
                    await _userManager.AddToRoleAsync(user, "Student");

                    // Log activity
                    var activity = new StudentActivity
                    {
                        UserId = user.Id,
                        UserName = $"{user.FirstName} {user.LastName}",
                        ActivityType = "Registration",
                        Description = "Student registered in the system and awaiting approval",
                        Timestamp = DateTime.Now
                    };
                    _context.StudentActivities.Add(activity);
                    
                    // Create approval notification for admin and dean
                    if (typeof(ApplicationDbContext).GetProperty("Notifications") != null)
                    {
                        var notification = new Notification
                        {
                            Title = "New Student Registration",
                            Message = $"New student {user.FirstName} {user.LastName} has registered and is awaiting approval.",
                            Link = $"/Student/Review/{user.Id}",
                            CreatedAt = DateTime.Now,
                            IsRead = false,
                            Type = NotificationType.StudentApproval
                        };
                        _context.Notifications.Add(notification);
                    }
                    
                    await _context.SaveChangesAsync();

                    // If an admin is registering a student, don't sign in the new user and set as approved
                    if (User.Identity?.IsAuthenticated == true &&
                        (User.IsInRole("Admin") || User.IsInRole("Dean") || User.IsInRole("Warden")))
                    {
                        // Auto-approve the student since an admin is registering them
                        user.IsApproved = true;
                        await _userManager.UpdateAsync(user);
                        
                        TempData["SuccessMessage"] = "Student registered successfully.";
                        return RedirectToAction("Details", "Student", new { id = user.Id });
                    }
                    
                    // Send confirmation email
                    string subject = "Bugema University Hostel - Registration Confirmation";
                    string message = $@"
                        <html>
                        <body>
                            <h2>Bugema University Hostel Management System</h2>
                            <p>Dear {user.FirstName} {user.LastName},</p>
                            <p>Thank you for registering with the Bugema University Hostel Management System.</p>
                            <p>Your registration is currently pending approval by an administrator. You will receive another email once your account has been approved.</p>
                            <p>Student ID: {user.StudentId}</p>
                            <p>Email: {user.Email}</p>
                            <p>Best regards,<br>Hostel Management Team<br>Bugema University</p>
                        </body>
                        </html>";

                    await _emailService.SendEmailAsync(user.Email, subject, message);
                    
                    // Otherwise show registration confirmation page (don't sign in yet)
                    TempData["SuccessMessage"] = "Your registration has been submitted successfully. Your account will be reviewed by an administrator. You will be able to log in once your account is approved.";
                    return RedirectToAction("RegistrationConfirmation");
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public IActionResult RegistrationConfirmation()
        {
            return View();
        }

        // Generate random password for admin-created accounts
        private string GenerateRandomPassword()
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            var random = new Random();
            var chars = new char[12];
            
            // Ensure at least one uppercase, one lowercase, one digit, and one special character
            chars[0] = validChars[random.Next(0, 26)]; // uppercase
            chars[1] = validChars[random.Next(26, 52)]; // lowercase
            chars[2] = validChars[random.Next(52, 62)]; // digit
            chars[3] = validChars[random.Next(62, validChars.Length)]; // special char
            
            // Fill the rest randomly
            for (int i = 4; i < chars.Length; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            
            // Shuffle the array
            for (int i = 0; i < chars.Length; i++)
            {
                int swapIndex = random.Next(0, chars.Length);
                char temp = chars[i];
                chars[i] = chars[swapIndex];
                chars[swapIndex] = temp;
            }
            
            return new string(chars);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Dean,Warden")]
        public async Task<IActionResult> ApproveStudent(string id)
        {
            var student = await _userManager.FindByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            student.IsApproved = true;
            student.ApprovalDate = DateTime.Now;
            student.ApprovedBy = User.Identity?.Name;
            
            await _userManager.UpdateAsync(student);
            
            // Log this activity
            var activity = new StudentActivity
            {
                UserId = student.Id,
                UserName = $"{student.FirstName} {student.LastName}",
                ActivityType = "Account Approval",
                Description = $"Account approved by {User.Identity?.Name}",
                Timestamp = DateTime.Now
            };
            _context.StudentActivities.Add(activity);
            
            // Send approval email
            string subject = "Bugema University Hostel - Account Approved";
            string message = $@"
                <html>
                <body>
                    <h2>Bugema University Hostel Management System</h2>
                    <p>Dear {student.FirstName} {student.LastName},</p>
                    <p>Your account has been approved. You can now log in to the Hostel Management System.</p>
                    <p>Student ID: {student.StudentId}</p>
                    <p>Email: {student.Email}</p>
                    <p>Please use your registered email and password to log in.</p>
                    <p><a href='https://hostelms.bugema.ac.ug/Account/Login?userType=student'>Click here to login</a></p>
                    <p>Best regards,<br>Hostel Management Team<br>Bugema University</p>
                </body>
                </html>";

            await _emailService.SendEmailAsync(student.Email, subject, message);
            
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = $"Student {student.FirstName} {student.LastName} has been approved successfully and notification has been sent.";
            return RedirectToAction("PendingApprovals", "Student");
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Dean,Warden")]
        public async Task<IActionResult> RejectStudent(string id)
        {
            var student = await _userManager.FindByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            
            // Log this activity before deletion
            var activity = new StudentActivity
            {
                UserId = "System",
                UserName = "System",
                ActivityType = "Account Rejection",
                Description = $"Student account for {student.FirstName} {student.LastName} ({student.Email}) was rejected by {User.Identity?.Name}",
                Timestamp = DateTime.Now
            };
            _context.StudentActivities.Add(activity);
            
            // Send rejection email
            string subject = "Bugema University Hostel - Registration Status";
            string message = $@"
                <html>
                <body>
                    <h2>Bugema University Hostel Management System</h2>
                    <p>Dear {student.FirstName} {student.LastName},</p>
                    <p>We regret to inform you that your registration request has not been approved at this time.</p>
                    <p>Please contact the Dean of Students' office for more information or to discuss your application.</p>
                    <p>Best regards,<br>Hostel Management Team<br>Bugema University</p>
                </body>
                </html>";

            await _emailService.SendEmailAsync(student.Email, subject, message);
            
            // Delete profile picture if exists
            if (!string.IsNullOrEmpty(student.ProfilePicture))
            {
                var picturePath = Path.Combine(_webHostEnvironment.WebRootPath, student.ProfilePicture.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (System.IO.File.Exists(picturePath))
                {
                    System.IO.File.Delete(picturePath);
                }
            }
            
            // Delete the user
            await _userManager.DeleteAsync(student);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Student registration has been rejected and notification has been sent.";
            return RedirectToAction("PendingApprovals", "Student");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}