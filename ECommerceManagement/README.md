# E-Commerce Management System

A console application built with C#, .NET 10, and ADO.NET connecting to SQL Server LocalDB.

## Features
- Pure ADO.NET using `Microsoft.Data.SqlClient`
- Category, Product, and Customer Management
- Atomic multi-table transactions (Orders + OrderItems + Stock deduction + Audit logs)
- Sales and customer reporting queries

## Setup
1. Create `ECommerceDB` in SQL Server (`(localdb)\mssqllocaldb`).
2. Run database schema scripts.
3. Update connection string in `DatabaseHelper.cs` if needed.
4. Press F5 in Visual Studio to build and run.