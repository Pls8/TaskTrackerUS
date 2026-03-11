![image alt](https://github.com/Pls8/TaskTrackerUS/blob/master/RepoTaskTraker_00.jpg?raw=true)
# TaskTracker

TaskTracker is a web application designed to help users manage tasks and track project progress efficiently. It allows for organizing tasks into sections, assigning weights to tasks, and visualizing overall progress through a dashboard.

## Features

- **Dashboard**: View high-level metrics including total tasks, progress percentages, and upcoming deadlines.
- **Task Management**: Create, edit, and delete tasks. Set priorities, deadlines, and completion status.
- **Sections**: Group tasks into logical sections. The system automatically calculates section progress based on the tasks within it.
- **Weighted Progress**: Assign weights to tasks to reflect their importance. The application calculates weighted completion percentages.
- **Sites**: Manage different project sites or locations associated with tasks.

## Technology Stack

- **Framework**: .NET 9 (ASP.NET Core MVC)
- **Database**: PostgreSQL (Entity Framework Core)
- **Containerization**: Docker & Docker Compose
- **Frontend**: Razor Views with Bootstrap 5

## Getting Started

### Prerequisites

- Docker Desktop installed
- .NET 9 SDK (optional, for local development without Docker)

### Running with Docker

1. Open a terminal in the project root directory.
2. Run the following command to build and start the application and database:
   
   docker-compose up --build

3. Access the application in your browser at: http://localhost:5000

### Running Locally (.NET CLI)

1. Ensure you have a running PostgreSQL instance.
2. Update the connection string in appsettings.json to point to your local database.
3. Run the application:

   dotnet run --project TaskTracker.Web

## Deployment

For instructions on deploying this application to Render.com with NeonDB, please refer to the DEPLOYMENT.md file included in this repository.
