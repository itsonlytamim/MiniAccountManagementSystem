# MiniAccountManagementSystem

A simple web-based account management system built with ASP.NET Core (Razor Pages) and MS SQL Server, using only stored procedures for database access.

---

## What This Project Does

- **User & Role Management:**  
  - Three roles: Admin, Accountant, Viewer  
  - Admins can manage users, roles, and assign module access (all via stored procedures)

- **Chart of Accounts:**  
  - Create, update, and delete accounts (with parent/child hierarchy)  
  - Uses stored procedure: `sp_ManageChartOfAccounts`

- **Voucher Entry:**  
  - Supports Journal, Payment, and Receipt vouchers  
  - Multi-line debit/credit entries with account selection  
  - Uses stored procedure: `sp_SaveVoucher`

- **Excel Export:**  
  - Export reports to Excel

---
## For Screenshots Check each of the **Screenshots** folder category wise operation

## How to Use

1. **Clone This Repo**

2. **Set Up The Database**
    - The SQL script is in:  
      `MiniAccountManagementSystemSln/Infrastructure/Scripts/1_App_Schema.sql`  
      (If you don’t see it, check the repo’s `Infrastructure/Scripts` folder.)
    - Run this script in your SQL Server to create all required tables, stored procedures, and initial data.
    - Stored Proceedure is in : 
      `MiniAccountManagementSystemSln/Infrastructure/Scripts/2_App_StoredProcedures.sql` 

3. **Configure the Application**
    - Edit `appsettings.json` and set your database connection string.

4. **Run the App**
    - Open the project in Visual Studio or use `dotnet run`.

5. **Login**
    - The initial admin user is created by the SQL script and using the seeder (see `MiniAccountManagementSystemSln\Web\Data` for username/password).

---

## What Makes This Different

- No LINQ or direct SQL in the code:  
  All database operations use stored procedures only.
- Simple UI, focused on clarity and core features.
- Authentication and authorization are managed via ASP.NET Identity, with custom role support.

---

If you have questions please mail shamiul.tamim@gmail.com
