## Database Information
- **Database Type:** Microsoft SQL Server
- **Database Name:** hostel_management (or your actual database name)
- **Authentication:** Windows Authentication (or specify if SQL Server auth)

## Database Setup Instructions
1. Open SQL Server Management Studio
2. Create a new database called `hostel_management`
3. Right-click the database → Tasks → Execute SQL Script
4. Run the script: `database/hostel_management_complete.sql`
5. Verify tables and data are imported correctly

# BugemahostelMS Database Setup

## Database Information
- **Database Name:** BugemahostelMS
- **Database Type:** SQL Server LocalDB
- **Server Instance:** `(localdb)\\mssqllocaldb`
- **Authentication:** Windows Authentication (Trusted Connection)

## Prerequisites
- Visual Studio 2022 (includes SQL Server LocalDB) OR
- SQL Server LocalDB (standalone download)
- SQL Server Management Studio (SSMS) - optional but recommended

## Setup Instructions

### 1. Install SQL Server LocalDB
If you don't have LocalDB installed:
- **With Visual Studio:** LocalDB is included with Visual Studio 2019/2022
- **Standalone:** Download "SQL Server Express LocalDB" from Microsoft
- **Verify Installation:** Open Command Prompt and run `sqllocaldb info`

### 2. Create and Import Database

#### Option A: Using SQL Server Management Studio (Recommended)
1. Open SQL Server Management Studio
2. Connect to: `(localdb)\mssqllocaldb`
3. If connection fails, try: `(localdb)\ProjectV13` or `(localdb)\v11.0`
4. Right-click "Databases" → New Database
5. Name it: `BugemahostelMS`
6. Click "OK" to create the database
7. Right-click the new `BugemahostelMS` database → Tasks → Execute SQL Script
8. Browse and select: `BugemahostelMS_complete.sql`
9. Click "Execute" to run the script
10. Verify that tables and data have been imported successfully

#### Option B: Using Command Line (sqlcmd)
```cmd
# Create the database
sqlcmd -S "(localdb)\mssqllocaldb" -E -Q "CREATE DATABASE BugemahostelMS"

# Import the database script
sqlcmd -S "(localdb)\mssqllocaldb" -E -d BugemahostelMS -i "database/BugemahostelMS_complete.sql"
```

#### Option C: Using Visual Studio (Package Manager Console)
```powershell
# If using Entity Framework migrations
Update-Database
```

### 3. Verify Database Setup
After importing, verify these tables exist in your `BugemahostelMS` database:
- Users/Students table
- Hostels/Rooms table
- Payments table
- Bookings table
- (Add your actual table names here)

## Connection Settings for Development

### Default Connection String (from appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BugemahostelMS;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### Alternative Connection Strings
```csharp
// If using different LocalDB instance
"Server=(localdb)\\ProjectV13;Database=BugemahostelMS;Trusted_Connection=True;MultipleActiveResultSets=true"

// If using full SQL Server Express
"Server=.\\SQLEXPRESS;Database=BugemahostelMS;Trusted_Connection=True;MultipleActiveResultSets=true"
```

## Default Login Credentials
(Update this section with your actual admin credentials)
- **Admin Username:** admin
- **Admin Password:** [provide default password]

## Troubleshooting

### Common Issues:
1. **LocalDB Connection Failed:**
   - Try different instance names: `(localdb)\mssqllocaldb`, `(localdb)\ProjectV13`, `(localdb)\v11.0`
   - Check if LocalDB is installed: run `sqllocaldb info` in Command Prompt
   - Start LocalDB: run `sqllocaldb start mssqllocaldb`

2. **Database Not Found:**
   - Ensure database name is exactly `BugemahostelMS`
   - Check if the import script ran successfully
   - In SSMS, refresh the Databases folder

3. **Permission Denied:**
   - LocalDB runs under your Windows account, so permissions should work automatically
   - Try running Visual Studio or SSMS as Administrator

4. **Entity Framework Issues:**
   - Ensure the connection string matches exactly
   - Try running `Update-Database` in Package Manager Console

### SQL Server Configuration
If you can't connect, check SQL Server Configuration Manager:
1. SQL Server Services → Ensure SQL Server service is running
2. SQL Server Network Configuration → Enable TCP/IP
3. Restart SQL Server service after changes

## Database Schema Information
- **Created for:** Hostel Management System
- **Version:** 1.0
- **Last Updated:** [Date of export]
- **Compatible with:** .NET 9.0, Entity Framework Core
