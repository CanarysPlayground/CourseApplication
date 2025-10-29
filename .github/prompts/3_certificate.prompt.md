---
mode: agent
---

# Certificate Generation and Display UI

## Task Objective
Create a comprehensive certificate generation and display system for the Course Registration System that allows users to search for and view digital certificates for completed courses.

## Requirements

### 1. Homepage Integration
- Add a prominent "Certificate" button on the homepage
- Button should be clearly visible and professionally styled
- On click, display a search interface for certificate lookup

### 2. Certificate Search Functionality
- Implement search by student name (first name, last name, or full name)
- Support partial name matching for better user experience
- Display helpful search suggestions or demo names for testing
- Handle cases where no certificates are found with appropriate messaging

### 3. Certificate Display
- Generate professional-looking digital certificates
- Include the following mandatory elements:
  - Certificate title ("Certificate of Completion")
  - Student's full name (prominently displayed)
  - Course name and description
  - Instructor name
  - Final grade with descriptive text (A=Excellent, B=Good, etc.)
  - Issue date in readable format
  - Unique certificate number
  - Digital signature/verification code
  - Professional styling with borders and formatting

### 4. Backend API Requirements
- Create RESTful API endpoints for certificate operations:
  - `GET /api/certificates/student-name/{name}` - Search by student name
  - `GET /api/certificates/{id}` - Get specific certificate by ID
  - `GET /api/certificates/student/{studentId}` - Get all certificates for a student
- Implement proper error handling and validation
- Return structured JSON data for certificates

### 5. Data Model
- Create Certificate entity with relationships to Student and Course
- Include fields for certificate number, issue date, grade, remarks
- Ensure proper navigation properties for data retrieval

### 6. User Experience
- Responsive design that works on desktop and mobile
- Loading states while searching for certificates
- Error messages for failed searches or network issues
- Print-friendly certificate layout
- Professional visual design with proper typography and colors

### 7. Sample Data
- Provide sample students and courses for demonstration
- Include pre-generated certificates for testing
- Cover different grades (A, B, C) and course types

## Technical Constraints
- Use .NET 9.0 Web API for backend
- Implement clean architecture patterns
- Follow C# coding conventions and best practices
- Use Entity Framework for data modeling (in-memory for demo)
- Create HTML/CSS/JavaScript frontend served from the API
- Ensure API endpoints follow RESTful conventions

## Success Criteria
1. ✅ Homepage displays a "Certificate" button
2. ✅ Clicking the button reveals a search interface
3. ✅ Users can search for certificates by entering a student name
4. ✅ Search returns relevant certificates with proper error handling
5. ✅ Certificates display with all required information in a professional format
6. ✅ The system handles edge cases (no results, network errors, etc.)
7. ✅ The UI is responsive and user-friendly
8. ✅ API endpoints are properly documented and functional
9. ✅ Sample data allows for immediate testing and demonstration
10. ✅ Certificate design is print-ready and professional

## Implementation Notes
- Start with the Certificate domain entity and DTOs
- Implement the service layer with sample data for quick testing
- Create API controllers with proper routing and error handling
- Build the frontend with progressive enhancement
- Ensure the certificate design is both web and print friendly
- Add proper validation and error messages throughout the system