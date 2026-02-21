\# Clean Architecture .NET MVC Task Management System - Implementation Prompt



\## \*\*System Overview\*\*

Create a .NET 8 MVC application following Clean Architecture principles for a construction/project task tracking system with PowerBI-style dashboard visualization.



\## \*\*Project Structure (Clean Architecture Layers)\*\*



\### \*\*Layer 1: Domain Layer (Core)\*\*

\*\*Project Name:\*\* `TaskTracker.Domain`

```

TaskTracker.Domain/

├── Entities/

│   ├── ProjectTask.cs

│   ├── ProjectSite.cs

│   ├── Milestone.cs

│   └── User.cs

├── Enums/

│   ├── TaskStatus.cs

│   ├── PriorityLevel.cs

│   └── SiteStatus.cs

├── Exceptions/

│   └── DomainExceptions.cs

├── Interfaces/

│   └── IAuditableEntity.cs

└── ValueObjects/

&nbsp;   └── Percentage.cs

```



\*\*Key Entities:\*\*

```csharp

// ProjectTask.cs

public class ProjectTask : IAuditableEntity

{

&nbsp;   public Guid Id { get; set; }

&nbsp;   public string Title { get; set; }

&nbsp;   public string Description { get; set; }

&nbsp;   public int PercentComplete { get; set; } // 0-100%

&nbsp;   public DateTime DueDate { get; set; }

&nbsp;   public DateTime? CompletedDate { get; set; }

&nbsp;   public PriorityLevel Priority { get; set; }

&nbsp;   public TaskStatus Status { get; set; }

&nbsp;   

&nbsp;   // Navigation

&nbsp;   public Guid SiteId { get; set; }

&nbsp;   public ProjectSite Site { get; set; }

&nbsp;   

&nbsp;   // Auditing

&nbsp;   public DateTime CreatedAt { get; set; }

&nbsp;   public string CreatedBy { get; set; }

}



// ProjectSite.cs

public class ProjectSite

{

&nbsp;   public Guid Id { get; set; }

&nbsp;   public string Name { get; set; }

&nbsp;   public string Location { get; set; }

&nbsp;   public SiteStatus Status { get; set; }

&nbsp;   public DateTime StartDate { get; set; }

&nbsp;   public DateTime PlannedEndDate { get; set; }

&nbsp;   public DateTime? ActualEndDate { get; set; }

&nbsp;   

&nbsp;   // Calculated properties

&nbsp;   public decimal OverallProgress => CalculateProgress();

&nbsp;   

&nbsp;   // Navigation

&nbsp;   public ICollection<ProjectTask> Tasks { get; set; }

}

```



\### \*\*Layer 2: Application Layer\*\*

\*\*Project Name:\*\* `TaskTracker.Application`

```

TaskTracker.Application/

├── Common/

│   ├── Interfaces/

│   │   ├── IApplicationDbContext.cs

│   │   └── ICurrentUserService.cs

│   ├── Behaviors/

│   │   └── ValidationBehavior.cs

│   └── Exceptions/

│       └── ValidationException.cs

├── Features/

│   ├── Dashboard/

│   │   ├── Queries/

│   │   │   ├── GetDashboardMetrics.cs

│   │   │   └── GetDashboardMetricsHandler.cs

│   │   └── DTOs/

│   │       └── DashboardMetricsDto.cs

│   ├── Tasks/

│   │   ├── Commands/

│   │   │   ├── CreateTask/

│   │   │   │   ├── CreateTaskCommand.cs

│   │   │   │   └── CreateTaskCommandHandler.cs

│   │   │   └── UpdateTaskProgress/

│   │   │       ├── UpdateTaskProgressCommand.cs

│   │   │       └── UpdateTaskProgressCommandHandler.cs

│   │   └── Queries/

│   │       └── GetTasksBySite/

│   │           ├── GetTasksBySiteQuery.cs

│   │           └── GetTasksBySiteQueryHandler.cs

│   └── Sites/

│       └── Queries/

│           └── GetSiteStatus/

│               ├── GetSiteStatusQuery.cs

│               └── GetSiteStatusQueryHandler.cs

├── DTOs/

│   ├── TaskDto.cs

│   └── SiteDto.cs

├── Mappings/

│   └── MappingProfile.cs

└── Application.csproj

```



\*\*Key DTOs:\*\*

```csharp

// DashboardMetricsDto.cs

public class DashboardMetricsDto

{

&nbsp;   public int TotalSites { get; set; }

&nbsp;   public decimal OverallCompletionPercentage { get; set; }

&nbsp;   public int SitesUnderConstruction { get; set; }

&nbsp;   public int CompletedSites { get; set; }

&nbsp;   public int DelayedSites { get; set; }

&nbsp;   public int TotalRemainingDays { get; set; }

&nbsp;   

&nbsp;   // Chart data

&nbsp;   public List<SiteProgressDto> SiteProgressList { get; set; }

&nbsp;   public List<TaskStatusDistributionDto> TaskDistribution { get; set; }

}

```



\### \*\*Layer 3: Infrastructure Layer\*\*

\*\*Project Name:\*\* `TaskTracker.Infrastructure`

```

TaskTracker.Infrastructure/

├── Persistence/

│   ├── ApplicationDbContext.cs

│   ├── Configurations/

│   │   ├── ProjectTaskConfiguration.cs

│   │   └── ProjectSiteConfiguration.cs

│   ├── Migrations/

│   └── Seed/

│       └── DatabaseSeeder.cs

├── Services/

│   ├── DateTimeService.cs

│   └── DashboardDataService.cs

├── Repositories/

│   └── GenericRepository.cs

└── Infrastructure.csproj

```



\### \*\*Layer 4: Presentation Layer\*\*

\*\*Project Name:\*\* `TaskTracker.Web` (MVC)

```

TaskTracker.Web/

├── Controllers/

│   ├── HomeController.cs

│   ├── DashboardController.cs

│   ├── TasksController.cs

│   └── SitesController.cs

├── Views/

│   ├── Shared/

│   │   ├── \_Layout.cshtml

│   │   ├── \_Navbar.cshtml

│   │   └── \_PowerBIStyleDashboard.cshtml

│   ├── Home/

│   │   └── Index.cshtml (Dashboard)

│   ├── Dashboard/

│   │   ├── Index.cshtml

│   │   └── \_MetricsCards.cshtml

│   ├── Tasks/

│   │   ├── Index.cshtml

│   │   ├── Create.cshtml

│   │   └── \_TaskList.cshtml

│   └── Sites/

│       └── Index.cshtml

├── ViewModels/

│   ├── DashboardViewModel.cs

│   ├── CreateTaskViewModel.cs

│   └── SiteStatusViewModel.cs

├── wwwroot/

│   ├── css/

│   │   ├── powerbi-styles.css

│   │   └── site.css

│   ├── js/

│   │   ├── dashboard-charts.js

│   │   └── task-management.js

│   └── lib/

│       └── chart.js/

└── Program.cs

```



\## \*\*Implementation Requirements\*\*



\### \*\*1. Database Setup (Entity Framework Core)\*\*

\- Use SQL Server with Entity Framework Core 8

\- Implement Identity for authentication

\- Configure relationships between Sites and Tasks

\- Add seed data for demonstration



\### \*\*2. Dashboard Requirements (PowerBI-Style)\*\*

\*\*Metrics to Display:\*\*

1\. \*\*Total Sites\*\* - Count of all project sites

2\. \*\*Overall Completion %\*\* - Weighted average across all sites

3\. \*\*Sites Under Construction\*\* - Sites with status "InProgress"

4\. \*\*Completed Sites\*\* - Sites with status "Completed"

5\. \*\*Delayed Sites\*\* - Sites past PlannedEndDate but not completed

6\. \*\*Remaining Days\*\* - Sum of days remaining across all active sites



\*\*Visualizations:\*\*

\- Progress bar for each site

\- Pie chart for task status distribution

\- Bar chart for site completion comparison

\- Timeline view for project milestones



\### \*\*3. Page Structure\*\*

\*\*Navigation Menu Items:\*\*

1\. \*\*Dashboard\*\* (Home page) - Main PowerBI-style dashboard

2\. \*\*Task Status\*\* - List view of all tasks with filtering

3\. \*\*Add Task\*\* - Form to create new tasks

4\. \*\*Site Overview\*\* - List of all project sites

5\. \*\*Reports\*\* - Future expansion for detailed reports



\### \*\*4. Task Management Features\*\*

\- Create/Edit/Delete tasks

\- Update progress percentage (0-100%)

\- Mark tasks as complete

\- Set priority levels

\- Assign to specific sites

\- Filter by status, priority, site



\### \*\*5. Technical Requirements\*\*

\- Use CQRS pattern for data access

\- Implement AutoMapper for object mapping

\- Use MediatR for command/query handling

\- Implement FluentValidation for input validation

\- Add global exception handling

\- Use Bootstrap 5 for responsive design

\- Implement Chart.js for visualizations



\### \*\*6. Styling Requirements\*\*

\- PowerBI-inspired color scheme (blues, grays, clean layout)

\- Card-based dashboard widgets

\- Interactive charts with hover effects

\- Mobile-responsive design

\- Dark/Light mode toggle (future enhancement)



\## \*\*Database Schema\*\*

```sql

CREATE TABLE ProjectSites (

&nbsp;   Id UNIQUEIDENTIFIER PRIMARY KEY,

&nbsp;   Name NVARCHAR(200) NOT NULL,

&nbsp;   Location NVARCHAR(500),

&nbsp;   Status INT NOT NULL,

&nbsp;   StartDate DATETIME2 NOT NULL,

&nbsp;   PlannedEndDate DATETIME2 NOT NULL,

&nbsp;   ActualEndDate DATETIME2 NULL

);



CREATE TABLE ProjectTasks (

&nbsp;   Id UNIQUEIDENTIFIER PRIMARY KEY,

&nbsp;   Title NVARCHAR(200) NOT NULL,

&nbsp;   Description NVARCHAR(MAX),

&nbsp;   PercentComplete INT DEFAULT 0,

&nbsp;   DueDate DATETIME2 NOT NULL,

&nbsp;   CompletedDate DATETIME2 NULL,

&nbsp;   Priority INT NOT NULL,

&nbsp;   Status INT NOT NULL,

&nbsp;   SiteId UNIQUEIDENTIFIER NOT NULL,

&nbsp;   FOREIGN KEY (SiteId) REFERENCES ProjectSites(Id)

);

```



\## \*\*Implementation Steps Request\*\*



Please generate the complete solution with the following steps:



1\. \*\*Create solution structure\*\* with the 4 projects following Clean Architecture

2\. \*\*Set up project references\*\* according to dependency flow

3\. \*\*Implement Domain entities\*\* with proper relationships

4\. \*\*Configure Application layer\*\* with CQRS handlers

5\. \*\*Set up Infrastructure\*\* with EF Core and repositories

6\. \*\*Create MVC Presentation layer\*\* with controllers and views

7\. \*\*Implement PowerBI-style dashboard\*\* with Chart.js visualizations

8\. \*\*Add task management pages\*\* (list, create, edit)

9\. \*\*Implement navigation\*\* with Bootstrap navbar

10\. \*\*Add sample seed data\*\* for demonstration

11\. \*\*Configure authentication\*\* (if needed for future)

12\. \*\*Add responsive styling\*\* matching PowerBI aesthetic



\## \*\*Additional Notes\*\*

\- Use .NET 8 LTS version

\- Follow async/await pattern throughout

\- Add XML documentation for public methods

\- Implement proper error handling

\- Add client-side validation where appropriate

\- Use partial views for reusable components

\- Implement view models for complex views



The application should be production-ready with clean code, proper separation of concerns, and maintainable architecture. Focus on delivering a functional MVP with the dashboard and basic task management first.s

