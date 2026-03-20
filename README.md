# Course Registration Application

A comprehensive **Course Registration Management System** built with ASP.NET Core 8.0 and a vanilla JavaScript frontend. The system demonstrates clean/layered architecture, RESTful APIs, certificate issuance, and role-based access control.

---

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Setup & Installation](#setup--installation)
- [Running Locally](#running-locally)
- [Running Tests](#running-tests)
- [Configuration](#configuration)
- [API Reference](#api-reference)
- [Frontend](#frontend)
- [Deployment](#deployment)
- [Troubleshooting](#troubleshooting)

---

## Features

| Feature | Description |
|---------|-------------|
| **Course Management** | Create, update, search, and paginate courses |
| **Student Management** | Register and manage student profiles |
| **Course Enrollment** | Enroll students with status tracking (Pending, Confirmed, Cancelled, Completed) |
| **Grading** | Assign letter grades (A–F) to completed registrations |
| **Certificate Issuance** | Issue verifiable certificates with digital signatures |
| **Role-Based Access** | Student, Instructor, Admin, and SuperAdmin roles |
| **Search & Filtering** | Filter courses by name, description, or instructor |
| **Pagination** | Efficient page-based data retrieval across all list endpoints |
| **Health Monitoring** | Built-in `/health` endpoint |
| **Swagger UI** | Interactive API documentation served at `/` |
| **Structured Logging** | Serilog with console + daily rolling file output |
| **Global Error Handling** | Consistent error responses via middleware |

---

## Architecture

The application follows **Clean Architecture** (also known as Layered/Onion Architecture):

```
┌─────────────────────────────────────────────────────┐
│   CourseRegistration.API          (Presentation)    │
│   Controllers · Middleware · Attributes · Program   │
└────────────────┬────────────────────────────────────┘
                 ↓
┌────────────────────────────────────────────────────┐
│   CourseRegistration.Application  (Business Logic)  │
│   Services · DTOs · Validators · AutoMapper         │
└────────────────┬───────────────────────────────────┘
                 ↓
┌───────────────────────────────────────────────────┐
│   CourseRegistration.Domain       (Core Rules)     │
│   Entities · Interfaces · Enums                   │
└───────────────┬───────────────────────────────────┘
                ↓
┌──────────────────────────────────────────────────┐
│   CourseRegistration.Infrastructure  (Data)       │
│   Repositories · Unit of Work · DbContext         │
└──────────────────────────────────────────────────┘
```

### Project Map

| Project | Role |
|---------|------|
| `CourseRegistration.API` | HTTP layer – controllers, middleware, DI wiring |
| `CourseRegistration.Application` | Business logic – services, DTOs, validators, mappings |
| `CourseRegistration.Domain` | Core domain – entities, interfaces, enums |
| `CourseRegistration.Infrastructure` | Data access – EF Core repositories, Unit of Work |
| `CourseRegistration.Tests` | xUnit unit tests |

### Key Design Patterns

- **Repository Pattern** – abstracts data access behind `ICourseRepository`, `IStudentRepository`, `IRegistrationRepository`
- **Unit of Work** – `IUnitOfWork` coordinates cross-repository transactions
- **Dependency Injection** – all dependencies wired in `Program.cs` via .NET's built-in DI
- **DTO / AutoMapper** – API contracts are separate from domain entities; mapped via AutoMapper profiles
- **FluentValidation** – declarative validation rules per DTO
- **Global Exception Middleware** – `ExceptionHandlingMiddleware` converts unhandled exceptions to consistent `ApiResponseDto` error responses

### Data Flow (example: create course)

```
HTTP POST /api/courses
    → CoursesController.CreateCourse()
        → ICourseService.CreateCourseAsync()
            → FluentValidation (CreateCourseDto)
            → IUnitOfWork → ICourseRepository.AddAsync()
            → AutoMapper → CourseDto
        ← ApiResponseDto<CourseDto> (201 Created)
```

---

## Prerequisites

| Requirement | Version | Notes |
|-------------|---------|-------|
| [.NET SDK](https://dotnet.microsoft.com/download) | 8.0.x | `global.json` pins `8.0.416` with `latestMinor` roll-forward |
| A modern web browser | — | For the frontend and Swagger UI |
| Git | — | To clone the repository |

> **Optional:** Visual Studio 2022 or VS Code with the C# Dev Kit extension for IDE support.

---

## Setup & Installation

```bash
# 1. Clone the repository
git clone https://github.com/CanarysPlayground/CourseApplication.git
cd CourseApplication

# 2. Restore NuGet packages
dotnet restore api/CourseRegistration.sln
```

No database server is required. The application uses an **EF Core In-Memory database** that is seeded automatically on startup with sample students, courses, and registrations.

---

## Running Locally

### Backend API

```bash
# Development mode (with hot-reload)
dotnet watch run --project api/CourseRegistration.API/CourseRegistration.API.csproj

# Or without hot-reload
dotnet run --project api/CourseRegistration.API/CourseRegistration.API.csproj
```

The API starts on:
- HTTP:  `http://localhost:5210`
- HTTPS: `https://localhost:7037`

Once running, open **`http://localhost:5210`** in your browser to access the **Swagger UI**.

### Frontend

The frontend is a static site (no build step required). Open `frontend/index.html` directly in a browser, or serve it with any static file server:

```bash
# Using Python's built-in server (Python 3)
cd frontend
python3 -m http.server 3000
# Then open http://localhost:3000
```

> **Note:** The frontend is configured to call the API at `http://localhost:5210/api`. Ensure the backend is running before using the frontend.

---

## Running Tests

### Backend unit tests

```bash
dotnet test api/CourseRegistration.Tests/CourseRegistration.Tests.csproj --configuration Release
```

To view detailed output:

```bash
dotnet test api/CourseRegistration.Tests/CourseRegistration.Tests.csproj \
  --configuration Release \
  --logger "console;verbosity=detailed"
```

Tests follow the **Arrange / Act / Assert** pattern and are located in `api/CourseRegistration.Tests/Services/`.

---

## Configuration

### `appsettings.json` (API)

Located at `api/CourseRegistration.API/appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Environment-specific overrides go in `appsettings.Development.json` (already present) or `appsettings.Production.json`.

### Environment Variables

| Variable | Purpose | Example |
|----------|---------|---------|
| `ASPNETCORE_ENVIRONMENT` | Sets the runtime environment | `Development` / `Production` |
| `ASPNETCORE_URLS` | Override the listening URL(s) | `http://+:8080` |

Set via shell, `.env` file, or the hosting platform's configuration panel.

### Secrets Handling

> ⚠️ **Never commit real secrets to source control.**

- Use [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for local development:
  ```bash
  dotnet user-secrets init --project api/CourseRegistration.API
  dotnet user-secrets set "SomeService:ApiKey" "your-value" --project api/CourseRegistration.API
  ```
- In production (Azure), store secrets in **Azure Key Vault** or the App Service **Application Settings** panel — they appear as environment variables at runtime.
- The `.env` file at the repository root is intentionally empty. Do not populate it with real credentials.

### Logging

Serilog writes to:
- **Console** (all environments)
- **Rolling daily file**: `api/CourseRegistration.API/logs/course-registration-YYYYMMDD.txt`

---

## API Reference

All responses share the `ApiResponseDto<T>` envelope:

```json
{
  "success": true,
  "data": { },
  "message": "Description",
  "errors": []
}
```

### Courses — `/api/courses`

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/api/courses` | List courses (paginated) |
| `GET` | `/api/courses/{id}` | Get course by ID |
| `POST` | `/api/courses` | Create course |
| `PUT` | `/api/courses/{id}` | Update course |
| `DELETE` | `/api/courses/{id}` | Delete course |
| `GET` | `/api/courses/search` | Search by term or instructor |
| `GET` | `/api/courses/available` | List available (enrollable) courses |
| `GET` | `/api/courses/instructor/{name}` | Courses by instructor |
| `GET` | `/api/courses/{id}/registrations` | Registrations for a course |

**Create course request:**
```json
POST /api/courses
{
  "courseName": "Introduction to C#",
  "description": "Beginner-friendly C# programming course.",
  "instructorName": "Dr. Jane Smith",
  "startDate": "2026-04-01T00:00:00Z",
  "endDate": "2026-06-30T00:00:00Z",
  "schedule": "MWF 10:00-11:30 AM"
}
```

**Query parameters for `GET /api/courses`:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `page` | int | 1 | Page number |
| `pageSize` | int | 10 | Items per page (max 100) |
| `searchTerm` | string | — | Filter by name/description |
| `instructor` | string | — | Filter by instructor name |

---

### Students — `/api/students`

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/api/students` | List students (paginated) |
| `GET` | `/api/students/{id}` | Get student by ID |
| `POST` | `/api/students` | Create student |
| `PUT` | `/api/students/{id}` | Update student |
| `DELETE` | `/api/students/{id}` | Delete student |

**Create student request:**
```json
POST /api/students
{
  "firstName": "Alice",
  "lastName": "Johnson",
  "email": "alice.johnson@example.com",
  "phoneNumber": "+1-555-0100",
  "dateOfBirth": "2000-05-15T00:00:00Z"
}
```

---

### Registrations — `/api/registrations`

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/api/registrations` | List registrations (paginated, filterable) |
| `GET` | `/api/registrations/{id}` | Get registration by ID |
| `POST` | `/api/registrations` | Enroll student in course |
| `PUT` | `/api/registrations/{id}` | Update registration |
| `DELETE` | `/api/registrations/{id}` | Cancel registration |
| `PUT` | `/api/registrations/{id}/status` | Update status |
| `PUT` | `/api/registrations/{id}/grade` | Assign grade |

**Enroll student request:**
```json
POST /api/registrations
{
  "studentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "courseId": "7ba19f64-8831-4572-c3fc-4a159f77bfb7",
  "notes": "Student requested early enrollment."
}
```

**Registration status values:** `Pending` · `Confirmed` · `Cancelled` · `Completed`

**Grade values:** `A` · `B` · `C` · `D` · `F`

---

### Certificates — `/api/certificates`

| Method | Path | Description |
|--------|------|-------------|
| `POST` | `/api/certificates` | Issue a certificate |
| `GET` | `/api/certificates/{id}` | Get certificate by ID |
| `GET` | `/api/certificates/verify/{number}` | Verify by certificate number |

---

### Admin — `/api/admin`

Admin-only operations requiring elevated role (`Admin` or `SuperAdmin`).

---

### Health Check

```
GET /health
```

Returns `200 OK` with status `Healthy` when the application is operational.

---

### Interactive API Docs

Swagger UI is available at **`http://localhost:5210`** (root path) when running locally, and provides a full interactive reference for all endpoints.

---

## Frontend

The frontend is a self-contained static web app located in `frontend/`:

| File | Description |
|------|-------------|
| `index.html` | Main page with navigation and modals |
| `script.js` | API calls, form handling, UI logic |
| `styles.css` | CSS3 styling (gradients, flexbox layout) |
| `certificate.js` | Certificate request and display logic |

The frontend communicates with the API at `http://localhost:5210/api`. To point it at a different backend, update the `API_BASE_URL` constant at the top of `script.js`.

---

## Deployment

### CI/CD — GitHub Actions

The workflow at `.github/workflows/ci-cd.yml` runs on every push or pull request to `main`:

1. **Build & Test** (`build-test` job)
   - Restores packages, builds the solution in Release mode, and runs all xUnit tests.
2. **Deploy** (`deploy` job, runs after `build-test`)
   - Publishes the API project and deploys to **Azure App Service** using the `azure/webapps-deploy` action.

**Required GitHub secrets for deployment:**

| Secret | Purpose |
|--------|---------|
| `AZURE_WEBAPP_PUBLISH_PROFILE` | Azure App Service publish profile (XML) |

**Required workflow variable to update before deploying:**

Open `.github/workflows/ci-cd.yml` and replace the placeholder value:
```yaml
AZURE_WEBAPP_NAME: 'YOUR-APP-SERVICE-NAME'   # ← replace with your App Service name
```

### Manual Azure Deployment

```bash
# Publish the API
dotnet publish api/CourseRegistration.API/CourseRegistration.API.csproj \
  -c Release -o ./publish

# Deploy using Azure CLI
az webapp deploy \
  --resource-group <your-rg> \
  --name <your-app-service> \
  --src-path ./publish \
  --type zip
```

### Production Configuration Notes

- Set `ASPNETCORE_ENVIRONMENT=Production` in the App Service Application Settings.
- Switch from the in-memory database to a persistent database (SQL Server / PostgreSQL) by replacing `UseInMemoryDatabase` with the appropriate EF Core provider in `Program.cs` and adding a connection string to the Application Settings.
- Enable HTTPS-only and configure HSTS in the App Service TLS/SSL settings.

---

## Troubleshooting

### API does not start / port already in use

```bash
# Find the process using port 5210
lsof -i :5210        # macOS / Linux
netstat -ano | findstr :5210   # Windows

# Kill the process, then restart the API
```

### `dotnet: command not found`

- Verify .NET 8 SDK is installed: `dotnet --version`
- Add `~/.dotnet` (or the SDK install path) to your `PATH`.

### Frontend cannot reach the API (CORS / network error)

- Ensure the backend is running on `http://localhost:5210`.
- The API has a permissive CORS policy (`AllowAll`) enabled in development — this is intentional for local use.
- Open the browser developer console (F12) to view the exact error.

### Tests fail with "could not load assembly"

```bash
# Rebuild before running tests
dotnet build api/CourseRegistration.sln --configuration Release
dotnet test api/CourseRegistration.Tests/CourseRegistration.Tests.csproj --configuration Release --no-build
```

### Seed data not appearing after restart

The in-memory database is recreated on every application restart, so seed data is re-applied automatically. If data appears missing mid-session, check the console logs for seeding errors.

### GitHub Actions deployment fails

1. Confirm `AZURE_WEBAPP_PUBLISH_PROFILE` secret is set in the repository **Settings → Secrets and variables → Actions**.
2. Confirm `AZURE_WEBAPP_NAME` in `.github/workflows/ci-cd.yml` matches your App Service name exactly.
3. Review the workflow run logs in the **Actions** tab for the specific error.

### Viewing application logs

```bash
# Tail the current day's log file (Linux / macOS)
tail -f api/CourseRegistration.API/logs/course-registration-$(date +%Y%m%d).txt
```

---

## Additional Documentation

| File | Description |
|------|-------------|
| [`AI_DEVOPS_DEMO_PLAN.md`](AI_DEVOPS_DEMO_PLAN.md) | Comprehensive demo guide covering planning, security, CI/CD, IaC, Kubernetes, and monitoring |
| [`DEMO_QUICK_REFERENCE.md`](DEMO_QUICK_REFERENCE.md) | Quick-reference card with GitHub Copilot prompts organized by workflow stage |
