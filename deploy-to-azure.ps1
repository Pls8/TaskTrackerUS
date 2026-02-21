# Azure Deployment Script
# Prerequisite: Install Azure CLI: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli
# Run 'az login' before running this script.

$RESOURCE_GROUP = "TaskTrackerRG"
$LOCATION = "eastus"
$DB_SERVER_NAME = "tasktracker-db-" + (Get-Random)
$DB_USER = "tasktrackeradmin"
$DB_PASS = "TaskTracker!2026"
$APP_SERVICE_PLAN = "TaskTrackerPlan"
$WEB_APP_NAME = "tasktracker-web-" + (Get-Random)
$DOCKER_IMAGE = Read-Host "Enter your Docker Hub image (e.g., username/tasktracker:latest)"

# 1. Create Resource Group
Write-Host "Creating Resource Group..."
az group create --name $RESOURCE_GROUP --location $LOCATION

# 2. Create PostgreSQL Database
Write-Host "Creating PostgreSQL Database (This may take a few minutes)..."
az postgres flexible-server create `
    --resource-group $RESOURCE_GROUP `
    --name $DB_SERVER_NAME `
    --location $LOCATION `
    --admin-user $DB_USER `
    --admin-password $DB_PASS `
    --sku-name Standard_B1ms `
    --tier Burstable `
    --public-access 0.0.0.0 `
    --storage-size 32 `
    --version 13

# Construct Connection String
$CONN_STR = "Host=$DB_SERVER_NAME.postgres.database.azure.com;Database=postgres;Username=$DB_USER;Password=$DB_PASS;SSL Mode=Require;Trust Server Certificate=true"

# 3. Create App Service Plan (Linux)
Write-Host "Creating App Service Plan..."
az appservice plan create `
    --name $APP_SERVICE_PLAN `
    --resource-group $RESOURCE_GROUP `
    --sku B1 `
    --is-linux

# 4. Create Web App for Containers
Write-Host "Creating Web App..."
az webapp create `
    --resource-group $RESOURCE_GROUP `
    --plan $APP_SERVICE_PLAN `
    --name $WEB_APP_NAME `
    --deployment-container-image-name $DOCKER_IMAGE

# 5. Configure App Settings (Connection String & Port)
Write-Host "Configuring App Settings..."
az webapp config appsettings set `
    --resource-group $RESOURCE_GROUP `
    --name $WEB_APP_NAME `
    --settings "ConnectionStrings__DefaultConnection=$CONN_STR" "WEBSITES_PORT=8080"

Write-Host "Deployment Complete!"
Write-Host "Your app URL: https://$WEB_APP_NAME.azurewebsites.net"
