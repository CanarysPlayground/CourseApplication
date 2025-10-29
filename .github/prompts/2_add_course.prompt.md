---
mode: agent
---
Define the task to achieve, including specific requirements, constraints, and success criteria.
# Add Course Feature - Backend Implementation

## Task
Implement a complete "Add Course" feature in the .NET backend application that allows authorized users to create new courses in the system.

## Specific Requirements

### 1. Data Model
- Create a Course entity with properties: Id, Title, Description, Duration, InstructorId, Category, CreatedDate, UpdatedDate
- Implement proper validation attributes and constraints
- Ensure Course entity follows EF Core conventions

### 2. API Endpoint
- Create POST /api/courses endpoint
- Accept Course creation data via request body
- Return created course with 201 status code
- Implement proper error handling with appropriate HTTP status codes

### 3. Business Logic
- Validate course data (required fields, business rules)
- Check for duplicate course titles
- Verify instructor exists before associating with course
- Implement authorization to ensure only authorized users can add courses

### 4. Data Persistence
- Configure Entity Framework DbContext for Course entity
- Implement repository pattern or direct DbContext usage
- Ensure proper database constraints and relationships

## Constraints
- Follow clean architecture principles
- Use async/await patterns throughout
- Implement proper error handling and logging
- Validate all inputs using model validation
- Ensure thread-safe operations
- Follow established naming conventions and code standards

## Success Criteria
- ✅ Course can be successfully created via API endpoint
- ✅ Proper validation prevents invalid data submission
- ✅ Database constraints are enforced
- ✅ API returns appropriate HTTP status codes and response bodies
- ✅ Comprehensive error handling covers edge cases
- ✅ Code follows project coding standards and patterns
- ✅ Unit tests cover business logic with minimum 80% coverage
- ✅ Integration tests verify end-to-end functionality

## Technical Implementation Notes
- Use `dotnet watch run` for development with hot reload
- Implement proper dependency injection
- Add appropriate logging for debugging and monitoring
- Consider implementing CQRS pattern if applicable to project architecture