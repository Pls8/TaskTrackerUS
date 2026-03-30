\# Deployment Plan: The "No Credit Card" Stack



We will switch to \*\*Render.com\*\* for hosting the application and \*\*Neon.tech\*\* for the database. Both providers are developer-friendly, offer generous free tiers, and typically allow sign-up via GitHub or Google \*\*without requiring a credit card\*\*.



\## 1. Database: Neon (Serverless PostgreSQL)

Neon offers a free tier for PostgreSQL that is perfect for this project.

\- \*\*Action\*\*: Sign up at \[neon.tech](https://neon.tech) (use GitHub/Google).

\- \*\*Action\*\*: Create a new project named `tasktracker`.

\- \*\*Outcome\*\*: You will get a \*\*Connection String\*\* (e.g., `postgres://user:pass@ep-xyz.aws.neon.tech/neondb...`).



\## 2. Application: Render (Web Service)

Render allows you to deploy Docker images directly from Docker Hub.

\- \*\*Action\*\*: Sign up at \[render.com](https://render.com) (use GitHub/Google).

\- \*\*Action\*\*: Create a new \*\*Web Service\*\*.

\- \*\*Source\*\*: Select "Deploy an existing image from a registry" and enter your Docker Hub image path (e.g., `yourusername/tasktracker:latest`).

\- \*\*Plan\*\*: Select the \*\*Free\*\* instance type.



\## 3. Configuration

We need to connect the two services.

\- \*\*Environment Variables\*\*: In the Render dashboard, we will add the `ConnectionStrings\_\_DefaultConnection` variable and paste the Connection String from Neon.

\- \*\*Note\*\*: We will need to append `Pooling=true;` to the Neon connection string for best performance with .NET.



\## 4. Verification

\- Render provides a generic `.onrender.com` URL.

\- We will access this URL to verify the application is running and connected to the database.



\*Note: The free tier on Render spins down after 15 minutes of inactivity (taking ~30s to wake up on the next visit). This is normal for free hosting and acceptable for portfolios/demos.\*



