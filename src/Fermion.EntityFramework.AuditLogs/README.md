# Fermion.EntityFramework.AuditLogs

Fermion.EntityFramework.AuditLogs is a comprehensive audit logging library for .NET applications that provides detailed tracking of entity changes, user activities, and audit trail analysis. It's built on top of Entity Framework Core and follows clean architecture principles.

## Features

- Entity change tracking with detailed property changes
- User activity monitoring and analysis
- Comprehensive audit trail analysis
- Time series data analysis
- User behavior analysis
- Entity change summaries
- Most modified entities tracking
- Entity changes trend analysis
- Clean Architecture implementation
- RESTful API endpoints for audit log management
- Configurable sensitive data masking
- Flexible entity and property filtering

## Installation

```bash
  dotnet add package Fermion.EntityFramework.AuditLogs
```

## Project Structure

The library follows Clean Architecture principles with the following layers:

### Core
- Base entities and interfaces
- Domain models
- Enums and constants

### Infrastructure
- Entity Framework Core configurations
- Database context implementations
- Repository implementations

### Application
- DTOs
- Interfaces
- Services
- Mappings

### Presentation
- Controllers
- API endpoints
- Request/Response models

### DependencyInjection
- Service registration extensions
- Configuration options

### Extensions
- Helper methods
- Utility functions

## Configuration

```csharp
// Configure audit logging in Startup.cs
builder.Services.AddAuditLogServices<ApplicationDbContext>(opt =>
{
    // Enable/Disable audit logging
    opt.Enabled = true;

    // Configure sensitive data masking
    opt.MaskPattern = "***MASKED***";
    opt.SensitiveProperties = [
        "Password", "Token", "Secret", "ApiKey", 
        "Key", "Credential", "Ssn", "Credit", 
        "Card", "SecurityCode", "Pin", "Authorization"
    ];

    // Configure entity-specific exclusions
    opt.ExcludedPropertiesByEntityType = new Dictionary<Type, HashSet<string>>
    {
        { typeof(Todo), ["AssignedTo"] }
    };

    // Configure entity type filtering
    opt.IncludedEntityTypes = [ 
        //typeof(Todo) 
    ];
    opt.ExcludedEntityTypes = [ 
        //typeof(Todo) 
    ];

    // Configure logging behavior
    opt.LogChangeDetails = true;
    opt.MaxValueLength = 5000;
    opt.LoggedStates = [
        States.Added,
        States.Modified,
        States.Deleted
    ];
});

// Register DbContext with audit logging
services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(connectionString)
           .UseAuditLog()
    );


// Configure DbContext
public class YourDbContext : DbContext 
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply audit log configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuditLogConfiguration).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EntityPropertyChangeConfiguration).Assembly);
    }
}
```

## API Endpoints

The library provides the following RESTful API endpoints when `EnableApiEndpoints` is set to true:

- `GET /api/auditlogs` - Get audit logs with filtering and pagination
- `GET /api/auditlogs/{id}` - Get specific audit log details
- `GET /api/auditlogs/user-activity` - Get user activity analysis
- `GET /api/auditlogs/user-behavior` - Get user behavior analysis
- `GET /api/auditlogs/most-modified` - Get most modified entities
- `GET /api/auditlogs/trends` - Get entity changes trends
- `GET /api/auditlogs/summary` - Get entity change summary

## DTOs

### Audit Logs
- `AuditLogResponseDto`: Detailed audit log information
- `GetListAuditLogRequestDto`: Filtering and pagination options
- `UserActivityDto`: User activity metrics
- `UserChangeBehaviorRequestDto`: User behavior analysis parameters
- `UserChangeBehaviorResponseDto`: User behavior analysis results

### Entity Property Changes
- `EntityPropertyChangeResponseDto`: Property change details

### Analysis
- `MostModifiedEntitiesRequestDto`: Most modified entities parameters
- `MostModifiedEntitiesResponseDto`: Most modified entities results
- `EntityChangesTrendRequestDto`: Trend analysis parameters
- `EntityChangesTrendResponseDto`: Trend analysis results
- `EntityChangeSummaryRequestDto`: Summary analysis parameters
- `EntityChangeSummaryResponseDto`: Summary analysis results

## Enums

- `States`: Audit log states (Added, Modified, Deleted)
- `TimeGrouping`: Time grouping options (Hourly, Daily, Weekly, Monthly)
