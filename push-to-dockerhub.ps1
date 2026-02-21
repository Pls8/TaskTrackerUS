# Docker Hub Deployment Script
# Prerequisite: You must have a Docker Hub account. Sign up at https://hub.docker.com/

# 1. Login to Docker Hub (will prompt for username/password)
docker login

# 2. Set your Docker Hub username variable
$DOCKER_USERNAME = Read-Host "Enter your Docker Hub username"

# 3. Tag your local image
# Assuming your local image is named 'test4ds-web' from the docker-compose build
# We'll tag it as 'tasktracker:latest'
docker tag test4ds-web "$DOCKER_USERNAME/tasktracker:latest"

# 4. Push the image to Docker Hub
docker push "$DOCKER_USERNAME/tasktracker:latest"

Write-Host "Image successfully pushed to Docker Hub: $DOCKER_USERNAME/tasktracker:latest"
