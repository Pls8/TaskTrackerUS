# Use the SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["TaskTracker.sln", "./"]
COPY ["TaskTracker.Web/TaskTracker.Web.csproj", "TaskTracker.Web/"]
COPY ["TaskTracker.Application/TaskTracker.Application.csproj", "TaskTracker.Application/"]
COPY ["TaskTracker.Domain/TaskTracker.Domain.csproj", "TaskTracker.Domain/"]
COPY ["TaskTracker.Infrastructure/TaskTracker.Infrastructure.csproj", "TaskTracker.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "TaskTracker.Web/TaskTracker.Web.csproj"

# Copy the rest of the source code
COPY . .

# Build the project
WORKDIR "/src/TaskTracker.Web"
RUN dotnet build "TaskTracker.Web.csproj" -c Release -o /app/build

# Publish the project
FROM build AS publish
RUN dotnet publish "TaskTracker.Web.csproj" -c Release -o /app/publish

# Use the runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskTracker.Web.dll"]
