# Bestiary Arena Cracker

This repository contains a distributed application used to automate and track progress for the Bestiary Arena game. It consists of several .NET services and a Next.js web interface.

## Prerequisites

- [.NET SDK 9.0](https://dotnet.microsoft.com/download) or newer
- [Node.js 18+](https://nodejs.org/) if you want to run the UI locally

## Building the solution

Restore dependencies and build all projects using the solution file:

```bash
dotnet build BestiaryArenaCracker.sln
```

To prepare the UI, install its npm packages:

```bash
cd src/BestiaryArenaCracker.UI
npm install
```

## Running the application

Start the distributed application host which launches the API, database containers and the UI:

```bash
dotnet run --project src/BestiaryArenaCracker.AppHost
```

You can also run the API or UI individually.

```bash
# Run the API only
 dotnet run --project src/BestiaryArenaCracker.Api

# Run the UI locally with hot reload
 cd src/BestiaryArenaCracker.UI
 npm run dev
```

The UI will be available by default at `http://localhost:3000`.

## Monitoring

When the application host is started it now also launches Node Exporter and cAdvisor.
Prometheus scrapes these exporters and Grafana provides a `Node Metrics` dashboard
showing CPU, memory and disk usage for the running containers.

## Running tests

Execute the unit tests for the solution with:

```bash
dotnet test
```

