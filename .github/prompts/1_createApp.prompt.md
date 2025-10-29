---
mode: agent
---

# Course Registration .NET API Application

## Task Overview
Create a comprehensive .NET 8 Web API application for course registration using the repository pattern with Entity Framework Core and in-memory database. The application should provide a complete course management and student registration system with proper separation of concerns and clean architecture principles.

## Specific Requirements

### 1. Architecture & Design Patterns
- **Repository Pattern**: Implement generic repository pattern with unit of work
- **Clean Architecture**: Separate concerns into layers (API, Application, Domain, Infrastructure)
- **Dependency Injection**: Use built-in .NET DI container
- **DTOs**: Implement request/response DTOs with AutoMapper
- **Validation**: Use FluentValidation for input validation

### 2. Domain Models
Create the following core entities:

#### Student Entity
- StudentId (Primary Key, Guid)
- FirstName (Required, max 50 characters)
- LastName (Required, max 50 characters)
- Email (Required, unique, valid email format)
- PhoneNumber (Optional, valid phone format)
- CreatedAt (DateTime)
- UpdatedAt (DateTime)

#### Course Entity
- CourseId (Primary Key, Guid)
- CourseName (Required, max 100 characters)
- Description (Optional, max 500 characters)
- CurrentEnrollment (Calculated property)
- InstructorName (Required, max 100 characters)
- StartDate (Required, future date)
- EndDate (Required, after start date)
- Schedule (Required, e.g., "MWF 10:00-11:00")
- IsActive (Boolean, default true)

#### Registration Entity
- RegistrationId (Primary Key, Guid)
- StudentId (Foreign Key to Student)
- CourseId (Foreign Key to Course)
- RegistrationDate (DateTime, auto-set to current time)
- Status (Enum: Pending, Confirmed, Cancelled, Completed)
- Grade (Optional, enum: A, B, C, D, F)
- Notes (Optional, max 200 characters)

### 3. API Endpoints

#### Student Management
- `GET /api/students` - Get all students with pagination
- `GET /api/students/{id}` - Get student by ID
- `POST /api/students` - Create new student
- `PUT /api/students/{id}` - Update student
- `DELETE /api/students/{id}` - Soft delete student
- `GET /api/students/{id}/registrations` - Get student's registrations

#### Course Management
- `GET /api/courses` - Get all active courses with pagination and filtering
- `GET /api/courses/{id}` - Get course by ID with enrollment details
- `POST /api/courses` - Create new course
- `PUT /api/courses/{id}` - Update course
- `DELETE /api/courses/{id}` - Soft delete course
- `GET /api/courses/search` - Search courses by name, code, or instructor
- `GET /api/courses/{id}/registrations` - Get course registrations

#### Registration Management
- `POST /api/registrations` - Register student for course
- `GET /api/registrations/{id}` - Get registration details
- `PUT /api/registrations/{id}/status` - Update registration status
- `DELETE /api/registrations/{id}` - Cancel registration
- `GET /api/registrations` - Get all registrations with filtering

### 4. Technical Constraints

#### Database Configuration
- Use Entity Framework Core with In-Memory Database
- Implement database seeding with sample data
- Configure relationships with proper foreign keys
- Implement soft delete functionality
- Add audit fields (CreatedAt, UpdatedAt) to all entities

#### Validation Rules
- Email addresses must be unique across students
- Course codes must be unique and follow pattern validation
- Students cannot register for the same course twice
- Courses cannot exceed maximum capacity
- Registration dates cannot be in the future beyond course start date
- Students must be at least 16 years old
- Course end date must be after start date

#### Error Handling
- Implement global exception handling middleware
- Return appropriate HTTP status codes
- Provide detailed error messages in development
- Log all exceptions with correlation IDs
- Handle concurrent access scenarios

#### Performance Requirements
- Implement pagination for list endpoints (default 10, max 100 per page)
- Use async/await patterns throughout
- Implement caching for frequently accessed data
- Optimize database queries with proper includes

### 5. Project Structure
```
CourseRegistration.API/
├── Controllers/
├── Middleware/
├── Program.cs
├── appsettings.json

CourseRegistration.Application/
├── DTOs/
├── Services/
├── Interfaces/
├── Validators/
├── Mappings/

CourseRegistration.Domain/
├── Entities/
├── Enums/
├── Interfaces/

CourseRegistration.Infrastructure/
├── Data/
├── Repositories/
├── Services/
```

### 6. Dependencies
- Microsoft.EntityFrameworkCore.InMemory
- AutoMapper.Extensions.Microsoft.DependencyInjection
- FluentValidation.AspNetCore
- Swashbuckle.AspNetCore (Swagger)
- Serilog.AspNetCore (Logging)

## Success Criteria

### Functional Requirements
1. ✅ All CRUD operations work correctly for Students, Courses, and Registrations
2. ✅ Business rules are enforced (capacity limits, unique constraints, date validations)
3. ✅ Search and filtering functionality works as expected
4. ✅ Pagination is implemented and functional
5. ✅ Data relationships are properly maintained

### Technical Requirements
1. ✅ Repository pattern is correctly implemented with dependency injection
2. ✅ All endpoints return appropriate HTTP status codes
3. ✅ Request/Response DTOs are used (no direct entity exposure)
4. ✅ Input validation works with clear error messages
5. ✅ Exception handling provides meaningful responses
6. ✅ Swagger documentation is complete and accurate
7. ✅ Logging is implemented throughout the application

### Quality Requirements
1. ✅ Code follows .NET coding conventions and clean architecture principles
2. ✅ Proper separation of concerns between layers
3. ✅ Database context is properly configured with relationships
4. ✅ Sample data is seeded for testing
5. ✅ Application starts without errors and serves requests successfully

## Testing Guidelines
- Create sample requests for all endpoints
- Test business rule validations
- Verify error handling scenarios
- Test pagination and filtering
- Validate data relationships and constraints

## Deliverables
1. Complete .NET 8 Web API project with clean architecture
2. Swagger documentation accessible at `/swagger`
3. Sample data seeded in in-memory database
4. README.md with setup and usage instructions
5. Postman collection or equivalent for API testing