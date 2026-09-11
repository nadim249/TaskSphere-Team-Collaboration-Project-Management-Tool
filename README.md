# TaskSphere

**Team Collaboration & Project Management Tool**

TaskSphere is a web-based project management platform built with ASP.NET Core MVC. It helps teams manage projects, assign tasks, track progress, and collaborate in one place.

## Features

- **Authentication & Role Management** — Admin, Project Manager, and Team Member roles
- **Project Management** — Create projects, manage members, set deadlines and priorities
- **Task Management** — Assign tasks, track status and priority, search and filter
- **Team Collaboration** — Comments and file attachments on tasks
- **Dashboard & Analytics** — Task statistics, top performers, and upcoming deadlines
- **User API** — CRUD operations for user management

## Tech Stack

ASP.NET Core MVC, C#, SQL Server, Entity Framework Core, HTML, CSS, JavaScript, Bootstrap, Web API

## Team

| Member | Contribution |
|---|---|
| Khalid Mahmud Nadim | Authentication, Task Management, Team Member Task Management |
| Md Rakibul Islam Ayon | Dashboard, Project Management, Team Member Dashboard & Projects |
| Arnab Hasan Kabbo | User Management, Profile Management, Team Management |

## Task Workflow

Pending → In Progress → Completed

## Running Locally

```bash
git clone <repository-url>
cd TaskSphere
dotnet restore
dotnet ef database update
dotnet run
```

Make sure to configure the SQL Server connection string in `appsettings.json` before running.

---

Advance Programming with .NET · Spring 2025–2026 · Section D · Group 03

*TaskSphere — Manage projects. Track tasks. Collaborate better.*
