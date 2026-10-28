# E-Commerce Management System

A console-based application built using C#, .NET, and ADO.NET with SQL Server LocalDB.

## Features
- Pure ADO.NET data access (SqlConnection, SqlCommand, SqlDataReader, SqlTransaction)
- Layered architecture: Models, Repositories, Services, and Helpers
- Atomic transactions: Order creation, stock deduction, and audit logging commit together
- Parameterized queries for SQL injection prevention

## Database Setup
1. Open SQL Server Management Studio (SSMS).
2. Connect to `(localdb)\mssqllocaldb`.
3. Create database `ECommerceDB` and run table setup scripts.

## How to Run
1. Open the project in Visual Studio.
2. Ensure connection string points to `(localdb)\mssqllocaldb`.
3. Press `F5` to build and run.