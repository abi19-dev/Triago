# Triago (Email Triage Agent) ✉️🛡️

Triago is an intelligent inbox assistant that scores incoming emails, routes them into the right state, and learns from human reviews. Messages that are uncertain are queued for review, and feedback is used to retrain the agent over time.

## What it does ✅
- Scores each incoming email for spam risk
- Routes messages to Allowed, Blocked, or Pending Review
- Collects human labels to improve future decisions
- Runs in short agent ticks (Sense → Think → Act → Learn)

## Project structure 🧭
- `src/AiAgents.Core` – generic agent abstractions
- `src/EmailTriageAgent` – domain + application + infrastructure logic
- `src/EmailTriageAgent.Web` – API host + background workers
- `webapp` – Triago UI

## Prerequisites 🧰
- .NET 8 SDK
- Node.js 18+
- SQL Server (LocalDB is fine)

## Configure database 🗄️
Update the connection string in:
- `src/EmailTriageAgent.Web/appsettings.json`

Example LocalDB:
```
Server=(localdb)\MSSQLLocalDB;Database=EmailTriageAgent;Trusted_Connection=True;TrustServerCertificate=True
```

## Run the API 🚀
1. Apply migrations:
```
Add-Migration InitialCreate -Project EmailTriageAgent -StartupProject EmailTriageAgent.Web
Update-Database -Project EmailTriageAgent -StartupProject EmailTriageAgent.Web
```
2. Run the API:
```
dotnet run --project src/EmailTriageAgent.Web
```

Swagger will be available at:
```
https://localhost:<port>/swagger
```

## Run the UI 🎨
```
cd webapp
npm install
npm run dev
```

## Basic usage 🧪
- Open the UI and go to Compose to send a new email to the agent.
- Check Inbox / Pending Review / Blocked / Completed tabs to see routing.
- Mark Pending Review items as Spam or Not Spam to provide feedback.
- When enough reviews are collected, the retrain agent runs automatically.
