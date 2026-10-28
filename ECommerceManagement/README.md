# E-Commerce Management System (Extended Version 1)

A modular backend console application built with **C#**, **.NET 10**, and direct **ADO.NET** (`Microsoft.Data.SqlClient`) connecting to **Microsoft SQL Server LocalDB / Express**.

---

## 1. Project Overview

This project implements a complete relational management system without an Object-Relational Mapper (ORM). It focuses on fundamental database interactions, safe query design, and enterprise data integrity practices.

- **Platform:** C# Console Application
- **Runtime:** .NET 10
- **Database Engine:** Microsoft SQL Server (`(localdb)\mssqllocaldb` or `.\SQLEXPRESS`)
- **Data Access:** ADO.NET (`Microsoft.Data.SqlClient`)
- **Architecture Pattern:** Multi-layered separation (Models $\rightarrow$ Repositories $\rightarrow$ Services $\rightarrow$ Presentation)

### Key Educational Objectives
- **Parameterized SQL Queries:** Preventing SQL injection across all input paths.
- **Relational Integrity:** Foreign keys, cascades, unique constraints, and check conditions.
- **Resource Discipline:** Proper async disposal using `await using` for connections, commands, readers, and transactions.
- **Atomic Transactions:** Coordinating order persistence, stock deduction, and audit logging under `SqlTransaction`.
- **Advanced Querying:** `INNER JOIN`, `LEFT JOIN`, `GROUP BY`, `COALESCE`, and `NOT EXISTS` subqueries.

---

## 2. Layered Architecture

```text
ECommerceManagement/
├── Helpers/
│   └── DatabaseHelper.cs        # Reusable SqlConnection factory
├── Models/
│   ├── Category.cs              # Category domain model
│   ├── Customer.cs              # Customer domain model
│   ├── Product.cs               # Product domain model
│   ├── Order.cs                 # Order header model
│   ├── OrderItem.cs             # Order line item model
│   └── AuditLog.cs              # Audit record model
├── Repositories/
│   ├── CategoryRepository.cs    # Category CRUD & name uniqueness queries
│   ├── CustomerRepository.cs    # Customer registration & email uniqueness
│   ├── ProductRepository.cs     # Catalog queries, JOINs, search & low-stock filters
│   ├── OrderRepository.cs       # Transactional multi-table order placement
│   ├── ReportRepository.cs      # Analytical aggregations and subqueries
│   └── AuditLogRepository.cs    # Audit record retrieval
├── Services/
│   ├── CategoryService.cs       # Category business rules and duplicate checks
│   ├── CustomerService.cs       # Customer validations (email, phone, duplicates)
│   ├── ProductService.cs        # Catalog rules (positive price, non-negative stock)
│   └── OrderService.cs          # Pre-order validation (stock sufficiency, active checks)
└── Program.cs                   # Interactive console UI and menu routing