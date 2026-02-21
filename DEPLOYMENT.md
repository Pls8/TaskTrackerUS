# Deployment Guide

This guide explains how to deploy the **TaskTracker** application to **Render.com** (Web Service) using **GitHub** for source control and **NeonDB** for the PostgreSQL database.

---

## 1. Prerequisites

Before starting, ensure you have the following accounts:
*   [**GitHub**](https://github.com/): To host your source code.
*   [**Render**](https://render.com/): To host the web application (Free Tier available).
*   [**NeonDB**](https://neon.tech/): To host the PostgreSQL database (Free Tier available).

---

## 2. Database Setup (NeonDB)

1.  **Create a Project**: Log in to NeonDB and create a new project.
2.  **Get Connection String**:
    *   Go to the **Dashboard**.
    *   Find the **Connection Details** section.
    *   Select the **.NET / Entity Framework** tab (or "ADO.NET").
    *   Copy the connection string. It should look like this:
        ```
        Host=ep-xyz.us-east-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=**********;Ssl Mode=Require;Trust Server Certificate=true
        ```
    *   **Note**: Using the `.NET` format is safer than the `postgres://` URI format because it avoids manual parsing errors.

---

## 3. Web Service Setup (Render)

1.  **Create New Web Service**:
    *   Log in to Render.
    *   Click **New +** -> **Web Service**.
2.  **Connect GitHub**:
    *   Select "Build and deploy from a Git repository".
    *   Connect your GitHub account and select the **TaskTracker** repository.
3.  **Configure Service**:
    *   **Name**: `task-tracker-web` (or any name).
    *   **Region**: Choose a region close to your database (e.g., US East).
    *   **Branch**: `main` (or your default branch).
    *   **Runtime**: **Docker**.
    *   **Instance Type**: Free (if applicable).
4.  **Environment Variables**:
    *   Scroll down to the **Environment Variables** section.
    *   Add a key named `CONNETION_STRING` (or `CONNECTION_STRING` or `DATABASE_URL`).
    *   Paste the **NeonDB Connection String** you copied in Step 2.
    *   *(Optional)* Add `ASPNETCORE_ENVIRONMENT` = `Production`.

5.  **Deploy**: Click **Create Web Service**.

---

## 4. Application Configuration Logic

The application (`TaskTracker.Infrastructure/DependencyInjection.cs`) is configured to be robust in finding the database connection. It checks for variables in the following priority order:

1.  **`CONNETION_STRING`**: Checks for this specific variable name first (handling the known typo).
2.  **`CONNECTION_STRING`**: Checks for the correct spelling.
3.  **`DATABASE_URL`**: Checks for the standard Render variable name.
4.  **`DefaultConnection`**: Falls back to `appsettings.json` (Localhost) if no environment variables are found.

### Connection String Formats Supported
*   **ADO.NET (Recommended)**: `Host=...;Database=...;Username=...;Password=...`
    *   This is the standard .NET format.
*   **URI**: `postgres://user:password@host/dbname`
    *   The application includes logic to parse this format and convert it to ADO.NET format automatically.
    *   If the port is missing or invalid (e.g., `-1`), it defaults to `5432`.

---

## 5. Troubleshooting

### "Connection Refused 127.0.0.1:5432"
*   **Cause**: The application cannot find any of the environment variables mentioned above. It is falling back to `localhost` (DefaultConnection).
*   **Fix**: Verify that you have added the correct Environment Variable Key (e.g., `CONNETION_STRING`) in the Render Dashboard settings. Ensure there are no extra spaces.


### Logs
*   On application startup, check the Render logs. You should see a message:
    ```
    [Startup] Using Connection String: Present (Password Hidden)
    ```
    If it says `NULL`, the environment variable is not being read.

---

## 6. Local Development

To run the application locally:

1.  **Docker**:
    ```bash
    docker-compose up --build
    ```
    This spins up a local PostgreSQL container and the Web App.

2.  **Manual (.NET CLI)**:
    *   Update `appsettings.json` with your local PostgreSQL credentials.
    *   Run:
        ```bash
        dotnet run --project TaskTracker.Web
        ```
