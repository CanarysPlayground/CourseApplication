---
applyTo: '**'
---
# .NET Development Environment Setup Instructions

## Prerequisites
- Install .NET 8.0 SDK or later from [Microsoft's official site](https://dotnet.microsoft.com/download)
- Install Visual Studio 2022 or Visual Studio Code with C# extension
- Install Git for version control

## Project Structure Standards
- Follow standard .NET project structure with `src/`, `tests/`, and `docs/` folders
- Use meaningful project and solution names
- Implement proper namespace conventions following company domain structure

## Coding Standards
- Follow Microsoft C# Coding Conventions
- Use PascalCase for public members, camelCase for private fields
- Implement XML documentation for public APIs
- Use nullable reference types and enable warnings as errors
- Follow SOLID principles and clean architecture patterns

## Dependencies Management
- Use PackageReference format in project files
- Pin package versions for production dependencies
- Regularly update packages and check for security vulnerabilities
- Use central package management for multi-project solutions

## Configuration Management
- Use appsettings.json for configuration with environment-specific overrides
- Implement the Options pattern for strongly-typed configuration
- Never commit secrets - use Azure Key Vault or user secrets for development
- Use environment variables for deployment-specific settings

## Testing Requirements
- Maintain minimum 80% code coverage
- Write unit tests using xUnit framework
- Implement integration tests for API endpoints
- Use TestContainers for database integration tests
- Follow AAA (Arrange, Act, Assert) pattern

## Code Quality Tools
- Enable static code analysis with .editorconfig
- Use SonarCloud or similar tools for code quality analysis
- Implement pre-commit hooks with Husky.NET
- Configure StyleCop analyzers for consistent code style

## Security Guidelines
- Enable security analyzers in project files
- Validate all user inputs and implement proper sanitization
- Use HTTPS everywhere and implement proper authentication
- Follow OWASP security guidelines
- Regular security scanning of dependencies

## Performance Standards
- Profile applications using Application Insights or similar tools
- Implement async/await patterns properly
- Use appropriate data structures and algorithms
- Monitor memory usage and implement proper disposal patterns
- Cache frequently accessed data appropriately

## Logging and Monitoring
- Use structured logging with Serilog or Microsoft.Extensions.Logging
- Implement correlation IDs for request tracking
- Set up health checks for all services
- Monitor application metrics and performance counters

## CI/CD Integration
- Configure GitHub Actions workflows for build, test, and deployment
- Implement automated code quality gates
- Use semantic versioning for releases
- Automate deployment to staging and production environments
- Maintain rollback strategies for deployments