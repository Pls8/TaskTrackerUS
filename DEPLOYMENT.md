# Deployment Guide: TaskTracker on Azure

This guide walks you through deploying your Dockerized application to Azure using the provided scripts.

## Prerequisites
1.  **Docker Hub Account**: [Sign up here](https://hub.docker.com/).
2.  **Azure Account**: [Sign up here](https://azure.microsoft.com/free/).
3.  **Azure CLI**: Install it from [here](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli).

## Step 1: Push Your Image to Docker Hub
We need to upload your local Docker image to a registry so Azure can download it.

1.  Open PowerShell in the project root.
2.  Run the push script:
    ```powershell
    ./push-to-dockerhub.ps1
    ```
3.  Enter your Docker Hub username when prompted.
4.  Wait for the upload to complete.

## Step 2: Deploy to Azure
This script creates a Resource Group, a PostgreSQL database, and an App Service for Containers.

1.  Log in to Azure:
    ```powershell
    az login
    ```
2.  Run the deployment script:
    ```powershell
    ./deploy-to-azure.ps1
    ```
3.  Enter the full image name you pushed in Step 1 (e.g., `yourusername/tasktracker:latest`).

## Step 3: Verification
The script will output your application URL (e.g., `https://tasktracker-web-12345.azurewebsites.net`).

1.  Open the URL in your browser.
2.  The first load might take a minute as the container starts up.
3.  Verify that the database is working by creating a new Section or Task.

## Troubleshooting
- **Application Error**: Check the logs in the Azure Portal -> App Service -> Log Stream.
- **Database Connection**: Ensure the firewall rules on the PostgreSQL server allow access (the script sets `0.0.0.0` for simplicity, but you may want to restrict this in production).
