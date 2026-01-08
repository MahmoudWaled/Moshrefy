# 🎓 Moshrefy - Educational Center Management System

**EduManager** is a robust, **Multi-Tenant** web application designed to manage educational centers, courses, students, and financial operations. Built with the latest **.NET 10** technologies and following **Clean Architecture** principles, it ensures scalability, security, and maintainability.

---

## Architecture & Design Patterns

The project follows a strict **Clean Architecture** approach with 4 distinct layers to ensure separation of concerns:

* **Domain Layer:** Enterprise logic and entities.
* **Application Layer:** Interfaces, DTOs, Mapping, and Business rules (CQRS/Services).
* **Infrastructure Layer:** Database context, Repositories, and external services.
* **Presentation Layer:** ASP.NET Core MVC / Razor Pages.

### Key Technical Concepts:
* ✅ **Multi-Tenancy:** Secure data isolation using `TenantContext`.
* ✅ **Repository & Unit of Work:** For transactional consistency.
* ✅ **Entity Framework Core:** With Code-First approach and seed data.
* ✅ **AutoMapper:** For clean Entity-to-DTO mapping.
* ✅ **FluentValidation:** For robust server-side validation.
* ✅ **Soft Delete:** Implemented globally for data recovery and auditing.

---

## Key Features (Implemented)

### 🔐 Security & Access Control
* **Identity System:** Full integration with ASP.NET Core Identity.
* **RBAC (Role-Based Access Control):** Pre-seeded roles (SuperAdmin, Admin, Manager, Employee).
* **Policy-Based Auth:** Granular permissions (e.g., `Permissions.Students.Create`).
* **Security Policies:** Strict password rules, account lockout, and cookie-based auth.

### Core Modules
* **User Management:** Complete CRUD for system users with role assignment.
* **Academic Structure:** Manage Academic Years, Courses, and Classrooms.
* **Student Affairs:** Student registration, profiles, and enrollments.
* **Staff Management:** Teacher profiles and course assignments.
* **Scheduling:** Session management and tracking.

### Operations & Exams
* **Attendance System:** Track daily attendance for students.
* **Examination:** Create exams, record results, and track grades.

### Financial Management
* **Invoicing:** Generate invoices for students.
* **Payments:** Record and track payments.
* **Billable Items:** Manage course fees and extra items.
* **Teacher Financials:** Track teacher-specific billable items.

### Multi-Tenancy (SuperAdmin)
* **Center Management:** Create and configure independent educational centers.
* **Tenant Administration:** Assign admins to specific centers.

---

## UI & UX
* **Interactive Tables:** Server-side **DataTables** integration for high performance with sorting/filtering.
* **Dynamic Dashboards:** Customized views based on user roles (Admin vs. Employee).
* **Icons:** Modern UI using Tabler Icons.

---

## Roadmap & Upcoming Features

The project is currently under active development. Below are the prioritized features for the next release:

* **Localization:** Full Arabic (RTL) support.
* **Reporting & Analytics:** Dashboard statistics, financial reports, and attendance insights.
* **Audit Logging:** Comprehensive tracking of "Who changed What and When".
* **File Management:** Upload capabilities for student photos and documents.
* **Bulk Operations:** Import Students/Teachers via Excel.

---

## Technology Stack

* **Backend:** .NET 10, C#, ASP.NET Core
* **Database:** SQL Server
* **ORM:** Entity Framework Core
* **Frontend:** MVC, JavaScript, jQuery
* **Tools:** Visual Studio 2026, Git

