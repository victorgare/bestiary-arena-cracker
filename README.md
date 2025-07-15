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

The application host also starts a monitoring stack consisting of Prometheus, Grafana,
and Loki for logs. A new Node Exporter container collects CPU, memory and disk
usage metrics from the host. These metrics are scraped by Prometheus and visualised
in Grafana using the `nodeexporter` dashboard.

You can also run the API or UI individually.

```bash
# Run the API only
 dotnet run --project src/BestiaryArenaCracker.Api

# Run the UI locally with hot reload
 cd src/BestiaryArenaCracker.UI
 npm run dev
```

The UI will be available by default at `http://localhost:3000`.

## Running tests

Execute the unit tests for the solution with:

```bash
dotnet test
```

