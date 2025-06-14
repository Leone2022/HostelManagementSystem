USE [master]
GO
/****** Object:  Database [BugemahostelMS]    Script Date: 5/31/2025 9:20:47 PM ******/
CREATE DATABASE [BugemahostelMS]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'BugemahostelMS', FILENAME = N'C:\Users\HP ELITEBOOK 840 G5\BugemahostelMS.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'BugemahostelMS_log', FILENAME = N'C:\Users\HP ELITEBOOK 840 G5\BugemahostelMS_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [BugemahostelMS] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BugemahostelMS].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [BugemahostelMS] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [BugemahostelMS] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [BugemahostelMS] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [BugemahostelMS] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [BugemahostelMS] SET ARITHABORT OFF 
GO
ALTER DATABASE [BugemahostelMS] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [BugemahostelMS] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [BugemahostelMS] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [BugemahostelMS] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [BugemahostelMS] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [BugemahostelMS] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [BugemahostelMS] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [BugemahostelMS] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [BugemahostelMS] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [BugemahostelMS] SET  ENABLE_BROKER 
GO
ALTER DATABASE [BugemahostelMS] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [BugemahostelMS] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [BugemahostelMS] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [BugemahostelMS] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [BugemahostelMS] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [BugemahostelMS] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [BugemahostelMS] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [BugemahostelMS] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [BugemahostelMS] SET  MULTI_USER 
GO
ALTER DATABASE [BugemahostelMS] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [BugemahostelMS] SET DB_CHAINING OFF 
GO
ALTER DATABASE [BugemahostelMS] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [BugemahostelMS] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [BugemahostelMS] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [BugemahostelMS] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [BugemahostelMS] SET QUERY_STORE = OFF
GO
USE [BugemahostelMS]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 5/31/2025 9:20:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Amenities]    Script Date: 5/31/2025 9:20:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Amenities](
	[AmenityId] [int] IDENTITY(1,1) NOT NULL,
	[HostelId] [int] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[IconClass] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Amenities] PRIMARY KEY CLUSTERED 
(
	[AmenityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Announcements]    Script Date: 5/31/2025 9:20:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Announcements](
	[AnnouncementId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[PostedDate] [datetime2](7) NOT NULL,
	[PostedBy] [nvarchar](max) NOT NULL,
	[ExpiryDate] [datetime2](7) NULL,
	[HostelId] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsUrgent] [bit] NOT NULL,
 CONSTRAINT [PK_Announcements] PRIMARY KEY CLUSTERED 
(
	[AnnouncementId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 5/31/2025 9:20:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 5/31/2025 9:20:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 5/31/2025 9:20:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 5/31/2025 9:20:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 5/31/2025 9:20:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 5/31/2025 9:20:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[FirstName] [nvarchar](max) NOT NULL,
	[LastName] [nvarchar](max) NOT NULL,
	[UserRole] [nvarchar](max) NULL,
	[StudentId] [nvarchar](max) NULL,
	[Course] [nvarchar](max) NULL,
	[Year] [nvarchar](max) NULL,
	[ParentName] [nvarchar](max) NULL,
	[ParentContact] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[Nationality] [nvarchar](max) NULL,
	[EmergencyContactName] [nvarchar](max) NULL,
	[EmergencyContactPhone] [nvarchar](max) NULL,
	[ProfilePicture] [nvarchar](max) NULL,
	[IdentificationType] [nvarchar](max) NULL,
	[IdentificationNumber] [nvarchar](max) NULL,
	[CurrentRoomNumber] [nvarchar](max) NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[AssignmentDate] [datetime2](7) NULL,
	[Country] [nvarchar](max) NULL,
	[CurrentHostelId] [int] NULL,
	[IsBoarding] [bit] NOT NULL,
	[IsCurrentlyInHostel] [bit] NOT NULL,
	[IsTemporaryAssignment] [bit] NOT NULL,
	[IsVerified] [bit] NOT NULL,
	[LastCheckInTime] [datetime2](7) NULL,
	[LastCheckOutTime] [datetime2](7) NULL,
	[ProbationEndDate] [datetime2](7) NULL,
	[RoomId] [int] NULL,
	[VerificationDate] [datetime2](7) NULL,
	[ApprovalDate] [datetime2](7) NULL,
	[ApprovedBy] [nvarchar](max) NULL,
	[IsApproved] [bit] NOT NULL,
	[RegistrationDate] [datetime2](7) NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 5/31/2025 9:20:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Bookings]    Script Date: 5/31/2025 9:20:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bookings](
	[BookingId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[RoomId] [int] NOT NULL,
	[BookingDate] [datetime2](7) NOT NULL,
	[CheckInDate] [datetime2](7) NOT NULL,
	[CheckOutDate] [datetime2](7) NOT NULL,
	[TotalAmount] [decimal](18, 2) NOT NULL,
	[Status] [int] NOT NULL,
	[Comments] [nvarchar](max) NULL,
	[ApprovedBy] [nvarchar](max) NULL,
	[ApprovalDate] [datetime2](7) NULL,
	[RejectionReason] [nvarchar](max) NULL,
	[Course] [nvarchar](100) NULL,
 CONSTRAINT [PK_Bookings] PRIMARY KEY CLUSTERED 
(
	[BookingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Hostels]    Script Date: 5/31/2025 9:20:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Hostels](
	[HostelId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Location] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[Gender] [int] NOT NULL,
	[Capacity] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[WardenName] [nvarchar](max) NULL,
	[WardenContact] [nvarchar](max) NULL,
	[ImageUrl] [nvarchar](max) NULL,
	[DistanceFromCampus] [decimal](18, 2) NOT NULL,
	[ManagementType] [int] NOT NULL,
	[HostelCode] [nvarchar](max) NULL,
	[YouTubeVideoId] [nvarchar](max) NULL,
	[LandlordId] [nvarchar](max) NULL,
	[WardenId] [nvarchar](max) NULL,
	[MinPrice] [decimal](18, 2) NULL,
	[MaxPrice] [decimal](18, 2) NULL,
	[AvailableRoomTypes] [nvarchar](max) NULL,
 CONSTRAINT [PK_Hostels] PRIMARY KEY CLUSTERED 
(
	[HostelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MaintenanceRequests]    Script Date: 5/31/2025 9:20:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MaintenanceRequests](
	[RequestId] [int] IDENTITY(1,1) NOT NULL,
	[RoomId] [int] NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[Status] [int] NOT NULL,
	[ResolvedById] [nvarchar](max) NULL,
	[ResolvedAt] [datetime2](7) NULL,
	[StaffNotes] [nvarchar](max) NULL,
	[IsUrgent] [bit] NOT NULL,
	[ReportedById] [nvarchar](450) NULL,
	[Resolution] [nvarchar](max) NULL,
 CONSTRAINT [PK_MaintenanceRequests] PRIMARY KEY CLUSTERED 
(
	[RequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notifications]    Script Date: 5/31/2025 9:20:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notifications](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Message] [nvarchar](500) NOT NULL,
	[Link] [nvarchar](255) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[ReadAt] [datetime2](7) NULL,
	[IsRead] [bit] NOT NULL,
	[Type] [int] NOT NULL,
	[TargetUserId] [nvarchar](max) NULL,
	[SenderUserId] [nvarchar](max) NULL,
	[SenderName] [nvarchar](max) NULL,
	[RecipientId] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Notifications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payments]    Script Date: 5/31/2025 9:20:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payments](
	[PaymentId] [int] IDENTITY(1,1) NOT NULL,
	[BookingId] [int] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[PaymentDate] [datetime2](7) NOT NULL,
	[Method] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[TransactionReference] [nvarchar](max) NULL,
	[ReceiptNumber] [nvarchar](max) NULL,
	[Notes] [nvarchar](max) NULL,
	[ProofOfPaymentUrl] [nvarchar](max) NULL,
 CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED 
(
	[PaymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Rooms]    Script Date: 5/31/2025 9:20:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Rooms](
	[RoomId] [int] IDENTITY(1,1) NOT NULL,
	[RoomNumber] [nvarchar](max) NOT NULL,
	[HostelId] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[Capacity] [int] NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[PricePerSemester] [decimal](18, 2) NOT NULL,
	[Status] [int] NOT NULL,
	[CurrentOccupancy] [int] NOT NULL,
	[NeedsAttention] [bit] NOT NULL,
 CONSTRAINT [PK_Rooms] PRIMARY KEY CLUSTERED 
(
	[RoomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StudentActivities]    Script Date: 5/31/2025 9:20:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudentActivities](
	[ActivityId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](max) NOT NULL,
	[UserName] [nvarchar](max) NOT NULL,
	[ActivityType] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[Timestamp] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_StudentActivities] PRIMARY KEY CLUSTERED 
(
	[ActivityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250327071417_InitialCreate', N'9.0.3')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250401124525_UpdateModels', N'9.0.3')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250413184351_AddNotificationsAndApproval', N'9.0.3')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250419203250_AddHostelManagementType', N'9.0.3')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250420121545_AddManagementTypeToHostel', N'9.0.3')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250420162028_AddHostelCodeField', N'9.0.3')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250420231656_MakeHostelCodeNullable', N'9.0.3')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250421091702_AddYouTubeVideoField', N'9.0.3')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250422212218_AddProofOfPaymentUrl', N'9.0.3')
GO
SET IDENTITY_INSERT [dbo].[Amenities] ON 

INSERT [dbo].[Amenities] ([AmenityId], [HostelId], [Name], [Description], [IconClass], [IsActive]) VALUES (47, 14, N'wifi', N'Free high-speed Wi-Fi available throughout the hostel.', N'fas fa-wifi', 1)
INSERT [dbo].[Amenities] ([AmenityId], [HostelId], [Name], [Description], [IconClass], [IsActive]) VALUES (48, 14, N'cleaning', N'Daily cleaning service for common areas and rooms.', N'fas fa-broom', 1)
INSERT [dbo].[Amenities] ([AmenityId], [HostelId], [Name], [Description], [IconClass], [IsActive]) VALUES (49, 14, N'security', N'24/7 security personnel and CCTV surveillance.', N'fas fa-shield-alt', 1)
INSERT [dbo].[Amenities] ([AmenityId], [HostelId], [Name], [Description], [IconClass], [IsActive]) VALUES (50, 15, N'wifi', N'Free high-speed Wi-Fi available throughout the hostel.', N'fas fa-wifi', 1)
INSERT [dbo].[Amenities] ([AmenityId], [HostelId], [Name], [Description], [IconClass], [IsActive]) VALUES (51, 15, N'cleaning', N'Daily cleaning service for common areas and rooms.', N'fas fa-broom', 1)
INSERT [dbo].[Amenities] ([AmenityId], [HostelId], [Name], [Description], [IconClass], [IsActive]) VALUES (52, 15, N'security', N'24/7 security personnel and CCTV surveillance.', N'fas fa-shield-alt', 1)
INSERT [dbo].[Amenities] ([AmenityId], [HostelId], [Name], [Description], [IconClass], [IsActive]) VALUES (53, 16, N'wifi', N'Free high-speed Wi-Fi available throughout the hostel.', N'fas fa-wifi', 1)
INSERT [dbo].[Amenities] ([AmenityId], [HostelId], [Name], [Description], [IconClass], [IsActive]) VALUES (54, 16, N'cleaning', N'Daily cleaning service for common areas and rooms.', N'fas fa-broom', 1)
INSERT [dbo].[Amenities] ([AmenityId], [HostelId], [Name], [Description], [IconClass], [IsActive]) VALUES (55, 16, N'security', N'24/7 security personnel and CCTV surveillance.', N'fas fa-shield-alt', 1)
INSERT [dbo].[Amenities] ([AmenityId], [HostelId], [Name], [Description], [IconClass], [IsActive]) VALUES (56, 16, N'kitchen', N'Fully equipped communal kitchen for guest use.', N'fas fa-utensils', 1)
INSERT [dbo].[Amenities] ([AmenityId], [HostelId], [Name], [Description], [IconClass], [IsActive]) VALUES (58, 18, N'wifi', N'Free high-speed Wi-Fi available throughout the hostel.', N'fas fa-wifi', 1)
INSERT [dbo].[Amenities] ([AmenityId], [HostelId], [Name], [Description], [IconClass], [IsActive]) VALUES (59, 18, N'cleaning', N'Daily cleaning service for common areas and rooms.', N'fas fa-broom', 1)
INSERT [dbo].[Amenities] ([AmenityId], [HostelId], [Name], [Description], [IconClass], [IsActive]) VALUES (60, 18, N'security', N'24/7 security personnel and CCTV surveillance.', N'fas fa-shield-alt', 1)
INSERT [dbo].[Amenities] ([AmenityId], [HostelId], [Name], [Description], [IconClass], [IsActive]) VALUES (61, 18, N'kitchen', N'Fully equipped communal kitchen for guest use.', N'fas fa-utensils', 1)
SET IDENTITY_INSERT [dbo].[Amenities] OFF
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'00df7608-3cad-42e4-9c68-b545d7e2e83e', N'Student', N'STUDENT', NULL)
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'17294a7a-de88-4050-8aa5-5a3ffdce2011', N'Dean', N'DEAN', NULL)
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'721c968f-aef8-4c41-b790-5231aed85818', N'Warden', N'WARDEN', NULL)
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'b7525f4d-c7cf-47a5-9ed1-dfcfcc8bd9ce', N'Admin', N'ADMIN', NULL)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'39f76439-4199-4c87-a718-7c3930b8f99f', N'00df7608-3cad-42e4-9c68-b545d7e2e83e')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'4bbdbce3-0c64-48a1-b48d-9631039b657e', N'00df7608-3cad-42e4-9c68-b545d7e2e83e')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'86db4821-ac85-47c0-bedf-57951bc2758f', N'00df7608-3cad-42e4-9c68-b545d7e2e83e')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd00bef25-78a1-46a0-b9e6-347c050a7f27', N'00df7608-3cad-42e4-9c68-b545d7e2e83e')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e77139c7-1241-4c53-a1d4-cad4cd55278b', N'00df7608-3cad-42e4-9c68-b545d7e2e83e')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'ea74ae1f-f3fb-4be9-ba02-cb191c53cef3', N'00df7608-3cad-42e4-9c68-b545d7e2e83e')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'ebe0f1d2-1c3f-4eb9-9bc7-5e005c085c37', N'00df7608-3cad-42e4-9c68-b545d7e2e83e')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'f17a2caf-62c1-4a81-bc34-f62a57947a49', N'00df7608-3cad-42e4-9c68-b545d7e2e83e')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'74a4a167-e100-48fe-9737-77ce22c3cf26', N'17294a7a-de88-4050-8aa5-5a3ffdce2011')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'9d246352-5be3-42e5-9380-973b7d90d6af', N'721c968f-aef8-4c41-b790-5231aed85818')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'2519927c-8503-4435-b824-c5de26219f45', N'b7525f4d-c7cf-47a5-9ed1-dfcfcc8bd9ce')
GO
INSERT [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [UserRole], [StudentId], [Course], [Year], [ParentName], [ParentContact], [Address], [Nationality], [EmergencyContactName], [EmergencyContactPhone], [ProfilePicture], [IdentificationType], [IdentificationNumber], [CurrentRoomNumber], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [AssignmentDate], [Country], [CurrentHostelId], [IsBoarding], [IsCurrentlyInHostel], [IsTemporaryAssignment], [IsVerified], [LastCheckInTime], [LastCheckOutTime], [ProbationEndDate], [RoomId], [VerificationDate], [ApprovalDate], [ApprovedBy], [IsApproved], [RegistrationDate]) VALUES (N'2519927c-8503-4435-b824-c5de26219f45', N'System', N'Administrator', N'Admin', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'admin@bugema.ac.ug', N'ADMIN@BUGEMA.AC.UG', N'admin@bugema.ac.ug', N'ADMIN@BUGEMA.AC.UG', 1, N'AQAAAAIAAYagAAAAEBlP+bgKj0JGW4RZwVr3d4PEzW/d8sjFmt0jj2MA0hgec9VkSAg7bkh/IwQltF5KFg==', N'IPOWAXFNE65MEJINWP4XTFLMJM6LO5GI', N'5bf0d6b2-d180-4ed8-a06e-2575b0291efb', NULL, 0, 0, NULL, 1, 0, NULL, NULL, NULL, 1, 0, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL)
INSERT [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [UserRole], [StudentId], [Course], [Year], [ParentName], [ParentContact], [Address], [Nationality], [EmergencyContactName], [EmergencyContactPhone], [ProfilePicture], [IdentificationType], [IdentificationNumber], [CurrentRoomNumber], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [AssignmentDate], [Country], [CurrentHostelId], [IsBoarding], [IsCurrentlyInHostel], [IsTemporaryAssignment], [IsVerified], [LastCheckInTime], [LastCheckOutTime], [ProbationEndDate], [RoomId], [VerificationDate], [ApprovalDate], [ApprovedBy], [IsApproved], [RegistrationDate]) VALUES (N'39f76439-4199-4c87-a718-7c3930b8f99f', N'Thandekile', N'Mabuza', N'Student', N'22/BWS/BU/R/0014', N'Social work and administration', N'First Year', N'Walvaton Mabuza', N'0771691101', N'Bulawayo', N'ZIMBABWEAN', N'Leone Chirodza', N'0781657234', N'/images/students/6c26429f-753c-4057-83e1-123b54a9ceff_IMG-20240502-WA0060-removebg-preview.png', NULL, NULL, NULL, N'thandekilencube@gmail.com', N'THANDEKILENCUBE@GMAIL.COM', N'thandekilencube@gmail.com', N'THANDEKILENCUBE@GMAIL.COM', 0, N'AQAAAAIAAYagAAAAEBSDaVM/rTunvnaxEthvZYaQcA3xLymhWEmopqrqTKsB83r9wjnH8T1CwJ1Gg+u6yw==', N'OFWCTTNBM4NONULEAVST65AOH7X4D7JA', N'02aa96a1-c79b-42ff-b223-4cc714ebb30f', NULL, 0, 0, NULL, 1, 0, NULL, N'ZIMBABWE', NULL, 1, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, CAST(N'2025-05-27T08:17:09.3826144' AS DateTime2), N'admin@bugema.ac.ug', 1, CAST(N'2025-04-22T05:33:22.0602577' AS DateTime2))
INSERT [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [UserRole], [StudentId], [Course], [Year], [ParentName], [ParentContact], [Address], [Nationality], [EmergencyContactName], [EmergencyContactPhone], [ProfilePicture], [IdentificationType], [IdentificationNumber], [CurrentRoomNumber], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [AssignmentDate], [Country], [CurrentHostelId], [IsBoarding], [IsCurrentlyInHostel], [IsTemporaryAssignment], [IsVerified], [LastCheckInTime], [LastCheckOutTime], [ProbationEndDate], [RoomId], [VerificationDate], [ApprovalDate], [ApprovedBy], [IsApproved], [RegistrationDate]) VALUES (N'4bbdbce3-0c64-48a1-b48d-9631039b657e', N'Peter', N'Sithole', N'Student', N'22/BWS/BU/R/0407', N'Theology', N'Fourth Year', N'Alex', N'07257682445', N'Pretoria 2354, Mandela Street', N'South Africa', N'alex', N'0876854467', N'/images/students/324e6275-39df-4a57-9de2-2924c2cd259f_20240216_190331.jpg', NULL, NULL, NULL, N'peter@gmail.com', N'PETER@GMAIL.COM', N'peter@gmail.com', N'PETER@GMAIL.COM', 0, N'AQAAAAIAAYagAAAAELjCAlkaXkg2cSPZtlt++TSMCeSKfm/qzS+0/70qaUO9I/tNbpfXN3ukrULtvB6abA==', N'JVHO6BJZRJDI33YEFLEDDKLKAFNCJ3X7', N'460eea05-2d05-4e8d-8951-c09e86659e45', NULL, 0, 0, NULL, 1, 0, NULL, N'South Africa', NULL, 1, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, CAST(N'2025-05-04T17:33:30.2309039' AS DateTime2), N'admin@bugema.ac.ug', 1, CAST(N'2025-05-04T17:21:02.0102212' AS DateTime2))
INSERT [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [UserRole], [StudentId], [Course], [Year], [ParentName], [ParentContact], [Address], [Nationality], [EmergencyContactName], [EmergencyContactPhone], [ProfilePicture], [IdentificationType], [IdentificationNumber], [CurrentRoomNumber], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [AssignmentDate], [Country], [CurrentHostelId], [IsBoarding], [IsCurrentlyInHostel], [IsTemporaryAssignment], [IsVerified], [LastCheckInTime], [LastCheckOutTime], [ProbationEndDate], [RoomId], [VerificationDate], [ApprovalDate], [ApprovedBy], [IsApproved], [RegistrationDate]) VALUES (N'74a4a167-e100-48fe-9737-77ce22c3cf26', N'Dean', N'Students', N'Dean', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'dean@bugema.ac.ug', N'DEAN@BUGEMA.AC.UG', N'dean@bugema.ac.ug', N'DEAN@BUGEMA.AC.UG', 1, N'AQAAAAIAAYagAAAAELo0ldvhni8ZUZynU7s3Y72z/pXFGRBtQM3fDS0ArpEx8IYdYxKFWwhssjCQzKnPaQ==', N'DKAL2MQQJGB4LMFPBEKKWXAQGS3VZTT3', N'1b6d8209-8282-4838-92f7-079129ec495e', NULL, 0, 0, NULL, 1, 0, NULL, NULL, NULL, 1, 0, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL)
INSERT [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [UserRole], [StudentId], [Course], [Year], [ParentName], [ParentContact], [Address], [Nationality], [EmergencyContactName], [EmergencyContactPhone], [ProfilePicture], [IdentificationType], [IdentificationNumber], [CurrentRoomNumber], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [AssignmentDate], [Country], [CurrentHostelId], [IsBoarding], [IsCurrentlyInHostel], [IsTemporaryAssignment], [IsVerified], [LastCheckInTime], [LastCheckOutTime], [ProbationEndDate], [RoomId], [VerificationDate], [ApprovalDate], [ApprovedBy], [IsApproved], [RegistrationDate]) VALUES (N'86db4821-ac85-47c0-bedf-57951bc2758f', N'Getrude', N'Mukyala', N'Student', N'22/BSE/BU/R/0008', N'Jonalism', N'Second Year', N'AUGUSTINE CHIRODZA', N'0771691101', N'First', N'Uganda', N'THANDEKILE MABUZA', N'0766167856', N'/images/students/f04acadb-2e66-40db-9680-c349595a28a0_HEADTEACHER.png', NULL, NULL, NULL, N'getrude@bugema.ac.ug', N'GETRUDE@BUGEMA.AC.UG', N'getrude@bugema.ac.ug', N'GETRUDE@BUGEMA.AC.UG', 0, N'AQAAAAIAAYagAAAAEMEQwYEwqLavQk9y79yjqzM436kgUmkSilPVq4wpMPrX1ush6yySpYFvvDeNbyK5Yg==', N'7BQBRAFTY4DOCD2LG4QX32TKRY5QKKJD', N'19d27ed8-29de-41a6-b72f-7213e1334388', NULL, 0, 0, NULL, 1, 0, NULL, N'Uganda', NULL, 1, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, CAST(N'2025-05-27T08:17:34.9472797' AS DateTime2), N'admin@bugema.ac.ug', 1, CAST(N'2025-04-22T13:16:06.2544944' AS DateTime2))
INSERT [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [UserRole], [StudentId], [Course], [Year], [ParentName], [ParentContact], [Address], [Nationality], [EmergencyContactName], [EmergencyContactPhone], [ProfilePicture], [IdentificationType], [IdentificationNumber], [CurrentRoomNumber], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [AssignmentDate], [Country], [CurrentHostelId], [IsBoarding], [IsCurrentlyInHostel], [IsTemporaryAssignment], [IsVerified], [LastCheckInTime], [LastCheckOutTime], [ProbationEndDate], [RoomId], [VerificationDate], [ApprovalDate], [ApprovedBy], [IsApproved], [RegistrationDate]) VALUES (N'9d246352-5be3-42e5-9380-973b7d90d6af', N'Hostel', N'Warden', N'Warden', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'warden@bugema.ac.ug', N'WARDEN@BUGEMA.AC.UG', N'warden@bugema.ac.ug', N'WARDEN@BUGEMA.AC.UG', 1, N'AQAAAAIAAYagAAAAEOriJPAvflP+32yneZDzIRH7g6bopANeuo8P1ZYyvryAezjCDtMB0ArAThQRVB3cwg==', N'VRDM3R4R6QJIPWPL7A4O5SEWFHQY53MD', N'a60753d1-ae86-46af-bbf3-4680a01abe14', NULL, 0, 0, NULL, 1, 0, NULL, NULL, NULL, 1, 0, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL)
INSERT [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [UserRole], [StudentId], [Course], [Year], [ParentName], [ParentContact], [Address], [Nationality], [EmergencyContactName], [EmergencyContactPhone], [ProfilePicture], [IdentificationType], [IdentificationNumber], [CurrentRoomNumber], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [AssignmentDate], [Country], [CurrentHostelId], [IsBoarding], [IsCurrentlyInHostel], [IsTemporaryAssignment], [IsVerified], [LastCheckInTime], [LastCheckOutTime], [ProbationEndDate], [RoomId], [VerificationDate], [ApprovalDate], [ApprovedBy], [IsApproved], [RegistrationDate]) VALUES (N'd00bef25-78a1-46a0-b9e6-347c050a7f27', N'LEONE', N'CHIRODZA', N'Student', N'22/BSE/BU/R/0007', N'SOFTWARE ENGINEERING', N'First Year', N'AUGUSTINE CHIRODZA', N'+27 72 868 9669', N'1933 Molebogeng street South Africa', N'ZIMBABWEAN', N'THANDEKILE MABUZA', N'0781657234', N'/images/students/ef152c6d-4570-4b3a-b56e-28158546bdc2_IMG-20240315-WA0022.jpg', NULL, NULL, NULL, N'leonechirodza@gmail.com', N'LEONECHIRODZA@GMAIL.COM', N'leonechirodza@gmail.com', N'LEONECHIRODZA@GMAIL.COM', 0, N'AQAAAAIAAYagAAAAEMTBgewOxqBgY3tVP2H/+lY4CJx6+WxkWl0EN8OnOfrD2L1qT9spK6kFE2X7dnRjpA==', N'YLKO6AGQ4JY6ESRAVQVDZM7LJTNFRTK7', N'230af5e3-752a-4730-a753-3d61a6f0756b', NULL, 0, 0, NULL, 1, 0, NULL, N'ZIMBABWE', NULL, 1, 0, 1, 0, NULL, NULL, NULL, NULL, NULL, CAST(N'2025-05-27T08:16:55.0297788' AS DateTime2), N'admin@bugema.ac.ug', 1, CAST(N'2025-04-22T05:30:52.1497258' AS DateTime2))
INSERT [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [UserRole], [StudentId], [Course], [Year], [ParentName], [ParentContact], [Address], [Nationality], [EmergencyContactName], [EmergencyContactPhone], [ProfilePicture], [IdentificationType], [IdentificationNumber], [CurrentRoomNumber], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [AssignmentDate], [Country], [CurrentHostelId], [IsBoarding], [IsCurrentlyInHostel], [IsTemporaryAssignment], [IsVerified], [LastCheckInTime], [LastCheckOutTime], [ProbationEndDate], [RoomId], [VerificationDate], [ApprovalDate], [ApprovedBy], [IsApproved], [RegistrationDate]) VALUES (N'e77139c7-1241-4c53-a1d4-cad4cd55278b', N'Thembi', N'Antony', N'Student', N'25/BWS/BU/R/0045', N'Social work and administration', N'Second Year', N'AUGUSTINE CHIRODZA', N'0771695351', N'Bugema University', N'ZIMBABWEAN', N'Leone Chirodza', N'0766167856', N'/images/students/fb481942-6a8d-4a54-969f-1b78935d4712_Untitl1.png', NULL, NULL, NULL, N'thembi@gmail.com', N'THEMBI@GMAIL.COM', N'thembi@gmail.com', N'THEMBI@GMAIL.COM', 0, N'AQAAAAIAAYagAAAAEKpBzgzgHanqeuWpJh4EvvrgVSUnWY13FL7rB31RSOW4mGym/AgU+l0ZnmW/UBSg2g==', N'ZPWBBCAVBDYRIP2QRHJKSZUKVBYVRRXO', N'348ee798-b869-462c-bdc6-9f76a6b88421', NULL, 0, 0, NULL, 1, 0, NULL, N'Uganda', NULL, 1, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, CAST(N'2025-05-07T10:49:35.0234121' AS DateTime2))
INSERT [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [UserRole], [StudentId], [Course], [Year], [ParentName], [ParentContact], [Address], [Nationality], [EmergencyContactName], [EmergencyContactPhone], [ProfilePicture], [IdentificationType], [IdentificationNumber], [CurrentRoomNumber], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [AssignmentDate], [Country], [CurrentHostelId], [IsBoarding], [IsCurrentlyInHostel], [IsTemporaryAssignment], [IsVerified], [LastCheckInTime], [LastCheckOutTime], [ProbationEndDate], [RoomId], [VerificationDate], [ApprovalDate], [ApprovedBy], [IsApproved], [RegistrationDate]) VALUES (N'ea74ae1f-f3fb-4be9-ba02-cb191c53cef3', N'Francis', N'Lubwanja', N'Student', N'22/BSE/BU/R/0045', N'SOFTWARE ENGINEERING', N'Third Year', N'Donald', N'0771695351', N'Bugema University', N'ZIMBABWEAN', N'Leone - Chirodza', N'0766167856', N'/images/students/dc12bf2d-4e8a-4e69-8490-540e763b89ac_20241204_144948.jpg', NULL, NULL, NULL, N'francis@gmail.com', N'FRANCIS@GMAIL.COM', N'francis@gmail.com', N'FRANCIS@GMAIL.COM', 0, N'AQAAAAIAAYagAAAAEF3eS6j8KevAFXKWhtkMg9+hVxkDI7HVKLdJj64WT3vlECb1YovourBs2Yd/+hZX9A==', N'LLR4XQ6E7V4STEMRLUTGXEBIW2CWQ4IY', N'29a959db-bdfa-4c55-ac3d-086f67b44489', NULL, 0, 0, NULL, 1, 0, NULL, N'Uganda', NULL, 1, 0, 1, 0, NULL, NULL, NULL, NULL, NULL, CAST(N'2025-05-27T08:18:03.7879443' AS DateTime2), N'admin@bugema.ac.ug', 1, CAST(N'2025-05-04T17:17:57.2472640' AS DateTime2))
INSERT [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [UserRole], [StudentId], [Course], [Year], [ParentName], [ParentContact], [Address], [Nationality], [EmergencyContactName], [EmergencyContactPhone], [ProfilePicture], [IdentificationType], [IdentificationNumber], [CurrentRoomNumber], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [AssignmentDate], [Country], [CurrentHostelId], [IsBoarding], [IsCurrentlyInHostel], [IsTemporaryAssignment], [IsVerified], [LastCheckInTime], [LastCheckOutTime], [ProbationEndDate], [RoomId], [VerificationDate], [ApprovalDate], [ApprovedBy], [IsApproved], [RegistrationDate]) VALUES (N'ebe0f1d2-1c3f-4eb9-9bc7-5e005c085c37', N'Leroy ', N'Chirodza', N'Student', N'24\DIT\BU\R\0008', N'Information Technology', N'First Year', N'AUGUSTINE CHIRODZA', N'+27 72 868 9669', N'1933 Molebogeng Street Mothotlung', N'ZIMBABWEAN', N'Leone Chirodza', N'0766167856', N'/images/students/0623dc17-b622-4183-81c2-29091cc26e23_WhatsApp Image 2025-04-22 at 5.34.52 AM.jpeg', NULL, NULL, NULL, N'leroychirodza@gmail.com', N'LEROYCHIRODZA@GMAIL.COM', N'leroychirodza@gmail.com', N'LEROYCHIRODZA@GMAIL.COM', 0, N'AQAAAAIAAYagAAAAEOUoU5bZlbXOImJ1ygpf3B+1wUJhHPQ2Bwz3TIFlzWGSAdcRaUOOIGH72g5GUT6NZw==', N'GTQQ44A5LRXLYSR3BKSUDWT5QCMMXLAX', N'047d3b38-38e7-4219-b197-ae5783a6b18e', NULL, 0, 0, NULL, 1, 0, NULL, N'ZIMBABWE', NULL, 1, 0, 1, 0, NULL, NULL, NULL, NULL, NULL, CAST(N'2025-05-27T08:17:17.1614658' AS DateTime2), N'admin@bugema.ac.ug', 1, CAST(N'2025-04-22T05:37:50.1421130' AS DateTime2))
INSERT [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [UserRole], [StudentId], [Course], [Year], [ParentName], [ParentContact], [Address], [Nationality], [EmergencyContactName], [EmergencyContactPhone], [ProfilePicture], [IdentificationType], [IdentificationNumber], [CurrentRoomNumber], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [AssignmentDate], [Country], [CurrentHostelId], [IsBoarding], [IsCurrentlyInHostel], [IsTemporaryAssignment], [IsVerified], [LastCheckInTime], [LastCheckOutTime], [ProbationEndDate], [RoomId], [VerificationDate], [ApprovalDate], [ApprovedBy], [IsApproved], [RegistrationDate]) VALUES (N'f17a2caf-62c1-4a81-bc34-f62a57947a49', N'John', N'Chirodza', N'Student', N'dom@bugema.ac.ug', N'SOFTWARE ENGINEERING', N'Third Year', N'AUGUSTINE CHIRODZA', N'0771691103', N'Bugema University, P.O. Box 6529 Kampala, Uganda', N'ZIMBABWEAN', N'Leone Chirodza', N'0766167856', N'/images/students/6ce58bda-94c5-467e-a48b-70f719e4991e_Screenshot 2025-04-03 04153.jpg', NULL, NULL, NULL, N'getrue@bugema.ac.ug', N'GETRUE@BUGEMA.AC.UG', N'getrue@bugema.ac.ug', N'GETRUE@BUGEMA.AC.UG', 0, N'AQAAAAIAAYagAAAAEEgXA3lskoJ8A/QQI23BkUj/TpiJ711R01WM+/uCgJcZqMLK+gMZYxbfuYnFyh0E3w==', N'7IYBTQTBHNWFGFPXRDOPQBQEAHSI2Y5N', N'99fe02ae-e6a7-4d0e-9d4f-a75ba16082c0', NULL, 0, 0, NULL, 1, 0, NULL, N'Uganda', NULL, 1, 0, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, CAST(N'2025-05-23T17:25:41.5341105' AS DateTime2))
GO
SET IDENTITY_INSERT [dbo].[Bookings] ON 

INSERT [dbo].[Bookings] ([BookingId], [UserId], [RoomId], [BookingDate], [CheckInDate], [CheckOutDate], [TotalAmount], [Status], [Comments], [ApprovedBy], [ApprovalDate], [RejectionReason], [Course]) VALUES (8, N'd00bef25-78a1-46a0-b9e6-347c050a7f27', 11, CAST(N'2025-05-27T09:43:12.5349720' AS DateTime2), CAST(N'2025-05-27T00:00:00.0000000' AS DateTime2), CAST(N'2025-09-27T00:00:00.0000000' AS DateTime2), CAST(800000.00 AS Decimal(18, 2)), 0, N'Course: Bachelor of Science in Software Engineering and Application Development
NICE', NULL, NULL, NULL, N'BSc_Software')
SET IDENTITY_INSERT [dbo].[Bookings] OFF
GO
SET IDENTITY_INSERT [dbo].[Hostels] ON 

INSERT [dbo].[Hostels] ([HostelId], [Name], [Location], [Description], [Gender], [Capacity], [IsActive], [WardenName], [WardenContact], [ImageUrl], [DistanceFromCampus], [ManagementType], [HostelCode], [YouTubeVideoId], [LandlordId], [WardenId], [MinPrice], [MaxPrice], [AvailableRoomTypes]) VALUES (14, N'NEW GENERATION', N'KIZIRI ROAD, BUGEMA UNIVERISTY( 1KM AWARE FROM MAIN GATE)', N'FFFFFFFFFFFF', 0, 2, 1, N'MRS LINDA', N'+256 788 947 4382', N'/images/hostels/5af5c99d-4a03-4750-ad8d-05d15784f6fb_ea574b7c-bb1c-4d9d-87be-ff04b84ca707_NEW GENERATION.jpg', CAST(0.80 AS Decimal(18, 2)), 1, N'BUHA006', N'R6rJo-PhbiU', NULL, NULL, CAST(500000.00 AS Decimal(18, 2)), CAST(800000.00 AS Decimal(18, 2)), N'Single,Double')
INSERT [dbo].[Hostels] ([HostelId], [Name], [Location], [Description], [Gender], [Capacity], [IsActive], [WardenName], [WardenContact], [ImageUrl], [DistanceFromCampus], [ManagementType], [HostelCode], [YouTubeVideoId], [LandlordId], [WardenId], [MinPrice], [MaxPrice], [AvailableRoomTypes]) VALUES (15, N'ANNEX MALE -', N'BUGEMA UNIVERSITY-BUSIKA ROAD', N'GGGGGGGGG', 0, 2, 1, N'MRS LINDA', N'+256 77 168 2299', N'/images/hostels/a9b246c0-44eb-41c5-9972-e34a43e749cf_annex.jpg', CAST(0.00 AS Decimal(18, 2)), 0, N'BUH006', N'R6rJo-PhbiU', NULL, NULL, CAST(500000.00 AS Decimal(18, 2)), CAST(800000.00 AS Decimal(18, 2)), N'Single,Double')
INSERT [dbo].[Hostels] ([HostelId], [Name], [Location], [Description], [Gender], [Capacity], [IsActive], [WardenName], [WardenContact], [ImageUrl], [DistanceFromCampus], [ManagementType], [HostelCode], [YouTubeVideoId], [LandlordId], [WardenId], [MinPrice], [MaxPrice], [AvailableRoomTypes]) VALUES (16, N'CLIFFORD', N'ACROSS THE ROAD, LESS THEN 100METER FROM MAIN GATE', N'GOOD PLACE', 1, 3, 1, N'SSSSSSSSS', N'SSSSSSSSSSS', N'/images/hostels/9a47adfb-5d91-495e-98d6-b505546d3f9f_f02bad49-74b5-40e5-9704-f0350b24c3a3_clifford.png', CAST(0.30 AS Decimal(18, 2)), 1, N'BUHA007', N'R6rJo-PhbiU', NULL, NULL, CAST(273000.00 AS Decimal(18, 2)), CAST(275000.00 AS Decimal(18, 2)), N'Single,Double')
INSERT [dbo].[Hostels] ([HostelId], [Name], [Location], [Description], [Gender], [Capacity], [IsActive], [WardenName], [WardenContact], [ImageUrl], [DistanceFromCampus], [ManagementType], [HostelCode], [YouTubeVideoId], [LandlordId], [WardenId], [MinPrice], [MaxPrice], [AvailableRoomTypes]) VALUES (17, N'BENSDOF', N'INSIDE CAMPUS', N'FFFFFFFFFF', 0, 6, 1, N'MRS LINDA', N'+256 700 123 456', N'/images/hostels/87e7a8fd-ce64-4a15-8cf5-8535b5f6da69_7049eab7-be11-4803-90d4-b398eafe11ba_BENSDOF.jpeg', CAST(0.10 AS Decimal(18, 2)), 0, N'BUH007', N'R6rJo-PhbiU', NULL, NULL, CAST(275000.00 AS Decimal(18, 2)), CAST(275000.00 AS Decimal(18, 2)), N'Single,Double')
INSERT [dbo].[Hostels] ([HostelId], [Name], [Location], [Description], [Gender], [Capacity], [IsActive], [WardenName], [WardenContact], [ImageUrl], [DistanceFromCampus], [ManagementType], [HostelCode], [YouTubeVideoId], [LandlordId], [WardenId], [MinPrice], [MaxPrice], [AvailableRoomTypes]) VALUES (18, N'CLIFFORD', N'ACROSS THE ROAD, LESS THEN 100METER FROM MAIN GATE', N'llllllllllllllllllllllll', 1, 6, 1, N'Mrs Betty', N'+256 77 168 2299', N'/images/hostels/6eef3be0-7d9a-4714-851b-40cc11308b98_9a47adfb-5d91-495e-98d6-b505546d3f9f_f02bad49-74b5-40e5-9704-f0350b24c3a3_clifford.png', CAST(0.50 AS Decimal(18, 2)), 0, N'BUH008', N'-n_l1-JNyjE', NULL, NULL, CAST(275000.00 AS Decimal(18, 2)), CAST(275000.00 AS Decimal(18, 2)), N'Single,Double')
SET IDENTITY_INSERT [dbo].[Hostels] OFF
GO
SET IDENTITY_INSERT [dbo].[Notifications] ON 

INSERT [dbo].[Notifications] ([Id], [Title], [Message], [Link], [CreatedAt], [ReadAt], [IsRead], [Type], [TargetUserId], [SenderUserId], [SenderName], [RecipientId]) VALUES (1, N'New Student Registration', N'New student LEONE CHIRODZA has registered and is awaiting approval.', N'/Student/Review/d00bef25-78a1-46a0-b9e6-347c050a7f27', CAST(N'2025-04-22T05:30:53.4027722' AS DateTime2), NULL, 0, 1, NULL, NULL, NULL, N'')
INSERT [dbo].[Notifications] ([Id], [Title], [Message], [Link], [CreatedAt], [ReadAt], [IsRead], [Type], [TargetUserId], [SenderUserId], [SenderName], [RecipientId]) VALUES (2, N'New Student Registration', N'New student Thandekile Mabuza has registered and is awaiting approval.', N'/Student/Review/39f76439-4199-4c87-a718-7c3930b8f99f', CAST(N'2025-04-22T05:33:22.6873410' AS DateTime2), NULL, 0, 1, NULL, NULL, NULL, N'')
INSERT [dbo].[Notifications] ([Id], [Title], [Message], [Link], [CreatedAt], [ReadAt], [IsRead], [Type], [TargetUserId], [SenderUserId], [SenderName], [RecipientId]) VALUES (3, N'New Student Registration', N'New student Leroy  Chirodza has registered and is awaiting approval.', N'/Student/Review/ebe0f1d2-1c3f-4eb9-9bc7-5e005c085c37', CAST(N'2025-04-22T05:37:50.6651962' AS DateTime2), NULL, 0, 1, NULL, NULL, NULL, N'')
INSERT [dbo].[Notifications] ([Id], [Title], [Message], [Link], [CreatedAt], [ReadAt], [IsRead], [Type], [TargetUserId], [SenderUserId], [SenderName], [RecipientId]) VALUES (4, N'New Student Registration', N'New student Getrude Mukyala has registered and is awaiting approval.', N'/Student/Review/86db4821-ac85-47c0-bedf-57951bc2758f', CAST(N'2025-04-22T13:16:07.4721330' AS DateTime2), NULL, 0, 1, NULL, NULL, NULL, N'')
INSERT [dbo].[Notifications] ([Id], [Title], [Message], [Link], [CreatedAt], [ReadAt], [IsRead], [Type], [TargetUserId], [SenderUserId], [SenderName], [RecipientId]) VALUES (5, N'New Student Registration', N'New student Francis Lubwanja has registered and is awaiting approval.', N'/Student/Review/ea74ae1f-f3fb-4be9-ba02-cb191c53cef3', CAST(N'2025-05-04T17:17:57.6558035' AS DateTime2), NULL, 0, 1, NULL, NULL, NULL, N'')
INSERT [dbo].[Notifications] ([Id], [Title], [Message], [Link], [CreatedAt], [ReadAt], [IsRead], [Type], [TargetUserId], [SenderUserId], [SenderName], [RecipientId]) VALUES (6, N'New Student Registration', N'New student Peter Sithole has registered and is awaiting approval.', N'/Student/Review/4bbdbce3-0c64-48a1-b48d-9631039b657e', CAST(N'2025-05-04T17:21:02.1423603' AS DateTime2), NULL, 0, 1, NULL, NULL, NULL, N'')
INSERT [dbo].[Notifications] ([Id], [Title], [Message], [Link], [CreatedAt], [ReadAt], [IsRead], [Type], [TargetUserId], [SenderUserId], [SenderName], [RecipientId]) VALUES (7, N'New Student Registration', N'New student Thembi Antony has registered and is awaiting approval.', N'/Student/Review/e77139c7-1241-4c53-a1d4-cad4cd55278b', CAST(N'2025-05-07T10:49:36.1201009' AS DateTime2), NULL, 0, 1, NULL, NULL, NULL, N'')
INSERT [dbo].[Notifications] ([Id], [Title], [Message], [Link], [CreatedAt], [ReadAt], [IsRead], [Type], [TargetUserId], [SenderUserId], [SenderName], [RecipientId]) VALUES (8, N'New Student Registration', N'New student John Chirodza has registered and is awaiting approval.', N'/Student/Review/f17a2caf-62c1-4a81-bc34-f62a57947a49', CAST(N'2025-05-23T17:25:42.0173325' AS DateTime2), NULL, 0, 1, NULL, NULL, NULL, N'')
SET IDENTITY_INSERT [dbo].[Notifications] OFF
GO
SET IDENTITY_INSERT [dbo].[Payments] ON 

INSERT [dbo].[Payments] ([PaymentId], [BookingId], [Amount], [PaymentDate], [Method], [Status], [TransactionReference], [ReceiptNumber], [Notes], [ProofOfPaymentUrl]) VALUES (8, 8, CAST(800000.00 AS Decimal(18, 2)), CAST(N'2025-05-27T09:43:12.7379664' AS DateTime2), 0, 0, N'fgr4444444567vb', NULL, N'Payment for NEW GENERATION, Room 1
Course: Bachelor of Science in Software Engineering and Application Development', N'/uploads/payments/36744230-991b-4365-a0cc-8df3e860b45a_9a47adfb-5d91-495e-98d6-b505546d3f9f_f02bad49-74b5-40e5-9704-f0350b24c3a3_clifford.png')
SET IDENTITY_INSERT [dbo].[Payments] OFF
GO
SET IDENTITY_INSERT [dbo].[Rooms] ON 

INSERT [dbo].[Rooms] ([RoomId], [RoomNumber], [HostelId], [Type], [Capacity], [Description], [PricePerSemester], [Status], [CurrentOccupancy], [NeedsAttention]) VALUES (11, N'1', 14, 0, 1, N'NICE', CAST(800000.00 AS Decimal(18, 2)), 0, 0, 0)
INSERT [dbo].[Rooms] ([RoomId], [RoomNumber], [HostelId], [Type], [Capacity], [Description], [PricePerSemester], [Status], [CurrentOccupancy], [NeedsAttention]) VALUES (12, N'2', 14, 1, 2, N'NICE', CAST(500000.00 AS Decimal(18, 2)), 0, 0, 0)
INSERT [dbo].[Rooms] ([RoomId], [RoomNumber], [HostelId], [Type], [Capacity], [Description], [PricePerSemester], [Status], [CurrentOccupancy], [NeedsAttention]) VALUES (13, N'3', 14, 0, 1, N'NICE', CAST(800000.00 AS Decimal(18, 2)), 0, 0, 0)
INSERT [dbo].[Rooms] ([RoomId], [RoomNumber], [HostelId], [Type], [Capacity], [Description], [PricePerSemester], [Status], [CurrentOccupancy], [NeedsAttention]) VALUES (14, N'A01', 16, 0, 1, N'NICE', CAST(800000.00 AS Decimal(18, 2)), 0, 0, 0)
INSERT [dbo].[Rooms] ([RoomId], [RoomNumber], [HostelId], [Type], [Capacity], [Description], [PricePerSemester], [Status], [CurrentOccupancy], [NeedsAttention]) VALUES (15, N'A02', 16, 0, 1, N'NICE', CAST(800000.00 AS Decimal(18, 2)), 0, 0, 0)
INSERT [dbo].[Rooms] ([RoomId], [RoomNumber], [HostelId], [Type], [Capacity], [Description], [PricePerSemester], [Status], [CurrentOccupancy], [NeedsAttention]) VALUES (16, N'1', 17, 0, 1, N'Leone', CAST(275000.00 AS Decimal(18, 2)), 0, 0, 0)
INSERT [dbo].[Rooms] ([RoomId], [RoomNumber], [HostelId], [Type], [Capacity], [Description], [PricePerSemester], [Status], [CurrentOccupancy], [NeedsAttention]) VALUES (17, N'B1', 17, 0, 1, N'Leone', CAST(275000.00 AS Decimal(18, 2)), 0, 0, 0)
INSERT [dbo].[Rooms] ([RoomId], [RoomNumber], [HostelId], [Type], [Capacity], [Description], [PricePerSemester], [Status], [CurrentOccupancy], [NeedsAttention]) VALUES (18, N'B2', 17, 1, 2, N'lgood', CAST(275000.00 AS Decimal(18, 2)), 0, 0, 0)
INSERT [dbo].[Rooms] ([RoomId], [RoomNumber], [HostelId], [Type], [Capacity], [Description], [PricePerSemester], [Status], [CurrentOccupancy], [NeedsAttention]) VALUES (19, N'C1', 18, 0, 1, N'Leone', CAST(275000.00 AS Decimal(18, 2)), 0, 0, 0)
INSERT [dbo].[Rooms] ([RoomId], [RoomNumber], [HostelId], [Type], [Capacity], [Description], [PricePerSemester], [Status], [CurrentOccupancy], [NeedsAttention]) VALUES (20, N'C2', 18, 1, 2, N'Leone', CAST(275000.00 AS Decimal(18, 2)), 0, 0, 0)
INSERT [dbo].[Rooms] ([RoomId], [RoomNumber], [HostelId], [Type], [Capacity], [Description], [PricePerSemester], [Status], [CurrentOccupancy], [NeedsAttention]) VALUES (21, N'A1', 15, 0, 1, N'ddddddddddddd', CAST(800000.00 AS Decimal(18, 2)), 0, 0, 0)
INSERT [dbo].[Rooms] ([RoomId], [RoomNumber], [HostelId], [Type], [Capacity], [Description], [PricePerSemester], [Status], [CurrentOccupancy], [NeedsAttention]) VALUES (22, N'A2', 15, 1, 2, N'kkkkkkkkkkkkkkkkkkkkkkk', CAST(500000.00 AS Decimal(18, 2)), 0, 0, 0)
SET IDENTITY_INSERT [dbo].[Rooms] OFF
GO
SET IDENTITY_INSERT [dbo].[StudentActivities] ON 

INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (1, N'24b15807-1861-4957-ae98-026bab3acb39', N'Sample Student', N'Account Approval', N'Account approved by admin@bugema.ac.ug', CAST(N'2025-04-21T12:19:22.2183755' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (2, N'd00bef25-78a1-46a0-b9e6-347c050a7f27', N'LEONE CHIRODZA', N'Registration', N'Student registered in the system and awaiting approval', CAST(N'2025-04-22T05:30:53.3473734' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (3, N'39f76439-4199-4c87-a718-7c3930b8f99f', N'Thandekile Mabuza', N'Registration', N'Student registered in the system and awaiting approval', CAST(N'2025-04-22T05:33:22.6854513' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (4, N'ebe0f1d2-1c3f-4eb9-9bc7-5e005c085c37', N'Leroy  Chirodza', N'Registration', N'Student registered in the system and awaiting approval', CAST(N'2025-04-22T05:37:50.6646260' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (5, N'ebe0f1d2-1c3f-4eb9-9bc7-5e005c085c37', N'Leroy  Chirodza', N'Account Approval', N'Account approved by dean@bugema.ac.ug', CAST(N'2025-04-22T05:39:02.3166435' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (6, N'86db4821-ac85-47c0-bedf-57951bc2758f', N'Getrude Mukyala', N'Registration', N'Student registered in the system and awaiting approval', CAST(N'2025-04-22T13:16:07.4349285' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (7, N'86db4821-ac85-47c0-bedf-57951bc2758f', N'Getrude Mukyala', N'Room Assignment', N'Assigned to C1 in Unknown Hostel', CAST(N'2025-04-22T13:17:56.4883260' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (8, N'ea74ae1f-f3fb-4be9-ba02-cb191c53cef3', N'Francis Lubwanja', N'Registration', N'Student registered in the system and awaiting approval', CAST(N'2025-05-04T17:17:57.6393689' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (9, N'4bbdbce3-0c64-48a1-b48d-9631039b657e', N'Peter Sithole', N'Registration', N'Student registered in the system and awaiting approval', CAST(N'2025-05-04T17:21:02.1415933' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (10, N'4bbdbce3-0c64-48a1-b48d-9631039b657e', N'Peter Sithole', N'Account Approval', N'Account approved by admin@bugema.ac.ug', CAST(N'2025-05-04T17:33:30.2420431' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (11, N'39f76439-4199-4c87-a718-7c3930b8f99f', N'Thandekile Mabuza', N'Room Assignment', N'Assigned to C1 in Unknown Hostel', CAST(N'2025-05-04T18:17:06.5058896' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (12, N'e77139c7-1241-4c53-a1d4-cad4cd55278b', N'Thembi Antony', N'Registration', N'Student registered in the system and awaiting approval', CAST(N'2025-05-07T10:49:36.0690150' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (13, N'ea74ae1f-f3fb-4be9-ba02-cb191c53cef3', N'Francis Lubwanja', N'Account Approval', N'Account approved by admin@bugema.ac.ug', CAST(N'2025-05-20T09:22:53.1940899' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (14, N'e77139c7-1241-4c53-a1d4-cad4cd55278b', N'Thembi Antony', N'Room Assignment', N'Assigned to C2 in Unknown Hostel', CAST(N'2025-05-20T09:25:06.8368038' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (15, N'f17a2caf-62c1-4a81-bc34-f62a57947a49', N'John Chirodza', N'Registration', N'Student registered in the system and awaiting approval', CAST(N'2025-05-23T17:25:41.9877501' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (16, N'4bbdbce3-0c64-48a1-b48d-9631039b657e', N'Peter Sithole', N'Room Assignment', N'Assigned to C2 in Unknown Hostel', CAST(N'2025-05-23T17:30:54.0489584' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (17, N'39f76439-4199-4c87-a718-7c3930b8f99f', N'Thandekile Mabuza', N'Room Unassignment', N'Unassigned from C1 in Unknown Hostel - Reason: Student Request', CAST(N'2025-05-26T13:38:00.6940513' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (18, N'86db4821-ac85-47c0-bedf-57951bc2758f', N'Getrude Mukyala', N'Room Unassignment', N'Unassigned from C1 in Unknown Hostel - Reason: Disciplinary Action', CAST(N'2025-05-26T13:38:11.1478686' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (19, N'4bbdbce3-0c64-48a1-b48d-9631039b657e', N'Peter Sithole', N'Room Unassignment', N'Unassigned from C2 in Unknown Hostel - Reason: Disciplinary Action', CAST(N'2025-05-26T13:38:40.2772734' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (20, N'e77139c7-1241-4c53-a1d4-cad4cd55278b', N'Thembi Antony', N'Room Unassignment', N'Unassigned from C2 in Unknown Hostel - Reason: Room Transfer', CAST(N'2025-05-26T13:38:46.1325552' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (21, N'd00bef25-78a1-46a0-b9e6-347c050a7f27', N'LEONE CHIRODZA', N'Account Approval', N'Account approved by admin@bugema.ac.ug', CAST(N'2025-05-27T08:16:55.6447934' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (22, N'39f76439-4199-4c87-a718-7c3930b8f99f', N'Thandekile Mabuza', N'Account Approval', N'Account approved by admin@bugema.ac.ug', CAST(N'2025-05-27T08:17:09.3913761' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (23, N'ebe0f1d2-1c3f-4eb9-9bc7-5e005c085c37', N'Leroy  Chirodza', N'Account Approval', N'Account approved by admin@bugema.ac.ug', CAST(N'2025-05-27T08:17:17.1701402' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (24, N'86db4821-ac85-47c0-bedf-57951bc2758f', N'Getrude Mukyala', N'Account Approval', N'Account approved by admin@bugema.ac.ug', CAST(N'2025-05-27T08:17:34.9532634' AS DateTime2))
INSERT [dbo].[StudentActivities] ([ActivityId], [UserId], [UserName], [ActivityType], [Description], [Timestamp]) VALUES (25, N'ea74ae1f-f3fb-4be9-ba02-cb191c53cef3', N'Francis Lubwanja', N'Account Approval', N'Account approved by admin@bugema.ac.ug', CAST(N'2025-05-27T08:18:03.7927070' AS DateTime2))
SET IDENTITY_INSERT [dbo].[StudentActivities] OFF
GO
/****** Object:  Index [IX_Amenities_HostelId]    Script Date: 5/31/2025 9:20:47 PM ******/
CREATE NONCLUSTERED INDEX [IX_Amenities_HostelId] ON [dbo].[Amenities]
(
	[HostelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Announcements_HostelId]    Script Date: 5/31/2025 9:20:47 PM ******/
CREATE NONCLUSTERED INDEX [IX_Announcements_HostelId] ON [dbo].[Announcements]
(
	[HostelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetRoleClaims_RoleId]    Script Date: 5/31/2025 9:20:47 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 5/31/2025 9:20:47 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserClaims_UserId]    Script Date: 5/31/2025 9:20:47 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserLogins_UserId]    Script Date: 5/31/2025 9:20:47 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserRoles_RoleId]    Script Date: 5/31/2025 9:20:47 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [EmailIndex]    Script Date: 5/31/2025 9:20:47 PM ******/
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AspNetUsers_CurrentHostelId]    Script Date: 5/31/2025 9:20:47 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUsers_CurrentHostelId] ON [dbo].[AspNetUsers]
(
	[CurrentHostelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AspNetUsers_RoomId]    Script Date: 5/31/2025 9:20:47 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUsers_RoomId] ON [dbo].[AspNetUsers]
(
	[RoomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 5/31/2025 9:20:47 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Bookings_RoomId]    Script Date: 5/31/2025 9:20:47 PM ******/
CREATE NONCLUSTERED INDEX [IX_Bookings_RoomId] ON [dbo].[Bookings]
(
	[RoomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Bookings_UserId]    Script Date: 5/31/2025 9:20:47 PM ******/
CREATE NONCLUSTERED INDEX [IX_Bookings_UserId] ON [dbo].[Bookings]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_MaintenanceRequests_ReportedById]    Script Date: 5/31/2025 9:20:47 PM ******/
CREATE NONCLUSTERED INDEX [IX_MaintenanceRequests_ReportedById] ON [dbo].[MaintenanceRequests]
(
	[ReportedById] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_MaintenanceRequests_RoomId]    Script Date: 5/31/2025 9:20:47 PM ******/
CREATE NONCLUSTERED INDEX [IX_MaintenanceRequests_RoomId] ON [dbo].[MaintenanceRequests]
(
	[RoomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Payments_BookingId]    Script Date: 5/31/2025 9:20:47 PM ******/
CREATE NONCLUSTERED INDEX [IX_Payments_BookingId] ON [dbo].[Payments]
(
	[BookingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Rooms_HostelId]    Script Date: 5/31/2025 9:20:47 PM ******/
CREATE NONCLUSTERED INDEX [IX_Rooms_HostelId] ON [dbo].[Rooms]
(
	[HostelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Announcements] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsUrgent]
GO
ALTER TABLE [dbo].[AspNetUsers] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsBoarding]
GO
ALTER TABLE [dbo].[AspNetUsers] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsCurrentlyInHostel]
GO
ALTER TABLE [dbo].[AspNetUsers] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsTemporaryAssignment]
GO
ALTER TABLE [dbo].[AspNetUsers] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsVerified]
GO
ALTER TABLE [dbo].[AspNetUsers] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsApproved]
GO
ALTER TABLE [dbo].[Hostels] ADD  DEFAULT ((0)) FOR [ManagementType]
GO
ALTER TABLE [dbo].[MaintenanceRequests] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsUrgent]
GO
ALTER TABLE [dbo].[Notifications] ADD  DEFAULT (N'') FOR [RecipientId]
GO
ALTER TABLE [dbo].[Rooms] ADD  DEFAULT ((0)) FOR [CurrentOccupancy]
GO
ALTER TABLE [dbo].[Rooms] ADD  DEFAULT (CONVERT([bit],(0))) FOR [NeedsAttention]
GO
ALTER TABLE [dbo].[Amenities]  WITH CHECK ADD  CONSTRAINT [FK_Amenities_Hostels_HostelId] FOREIGN KEY([HostelId])
REFERENCES [dbo].[Hostels] ([HostelId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Amenities] CHECK CONSTRAINT [FK_Amenities_Hostels_HostelId]
GO
ALTER TABLE [dbo].[Announcements]  WITH CHECK ADD  CONSTRAINT [FK_Announcements_Hostels_HostelId] FOREIGN KEY([HostelId])
REFERENCES [dbo].[Hostels] ([HostelId])
GO
ALTER TABLE [dbo].[Announcements] CHECK CONSTRAINT [FK_Announcements_Hostels_HostelId]
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUsers_Hostels_CurrentHostelId] FOREIGN KEY([CurrentHostelId])
REFERENCES [dbo].[Hostels] ([HostelId])
GO
ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_AspNetUsers_Hostels_CurrentHostelId]
GO
ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUsers_Rooms_RoomId] FOREIGN KEY([RoomId])
REFERENCES [dbo].[Rooms] ([RoomId])
GO
ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_AspNetUsers_Rooms_RoomId]
GO
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK_Bookings_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK_Bookings_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK_Bookings_Rooms_RoomId] FOREIGN KEY([RoomId])
REFERENCES [dbo].[Rooms] ([RoomId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK_Bookings_Rooms_RoomId]
GO
ALTER TABLE [dbo].[MaintenanceRequests]  WITH CHECK ADD  CONSTRAINT [FK_MaintenanceRequests_AspNetUsers_ReportedById] FOREIGN KEY([ReportedById])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[MaintenanceRequests] CHECK CONSTRAINT [FK_MaintenanceRequests_AspNetUsers_ReportedById]
GO
ALTER TABLE [dbo].[MaintenanceRequests]  WITH CHECK ADD  CONSTRAINT [FK_MaintenanceRequests_Rooms_RoomId] FOREIGN KEY([RoomId])
REFERENCES [dbo].[Rooms] ([RoomId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MaintenanceRequests] CHECK CONSTRAINT [FK_MaintenanceRequests_Rooms_RoomId]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_Bookings_BookingId] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Bookings] ([BookingId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_Bookings_BookingId]
GO
ALTER TABLE [dbo].[Rooms]  WITH CHECK ADD  CONSTRAINT [FK_Rooms_Hostels_HostelId] FOREIGN KEY([HostelId])
REFERENCES [dbo].[Hostels] ([HostelId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Rooms] CHECK CONSTRAINT [FK_Rooms_Hostels_HostelId]
GO
USE [master]
GO
ALTER DATABASE [BugemahostelMS] SET  READ_WRITE 
GO
