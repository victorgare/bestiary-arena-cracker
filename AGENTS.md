# Repository Instructions

## Programmatic checks
- Run `dotnet test` from the repository root after making changes.

## Development
- Build the solution with `dotnet build BestiaryArenaCracker.sln`.
- To work on the UI, first run `npm install` in `src/BestiaryArenaCracker.UI`.
- Start the infrastructure using
  `dotnet run --project src/BestiaryArenaCracker.AppHost`.
  This launches the API, SQL Server, Redis and the monitoring stack
  (Prometheus, Grafana and the OpenTelemetry Collector). When the
  application starts, the console output lists the URLs for each
  component.
  The monitoring configuration files live under `prometheus/`,
  `grafana/` and `otelcollector/`.

## Pull request guidelines
- Summarize your changes and reference files in the PR description.
- Mention the test results in the PR message.
