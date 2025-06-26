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

## Grafana dashboards

Grafana starts automatically when running the distributed host. You can access
it at `http://localhost:3000`. A new dashboard named **Endpoint Node Graph**
visualizes each API route as a node with the request rate and average response
time. It is provisioned from `grafana/dashboards/endpoint-nodegraph.json`.

## Running tests

Execute the unit tests for the solution with:

```bash
dotnet test
```

