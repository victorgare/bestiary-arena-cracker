# Migrations steps

## Create new migration step

change `<step name>` with the desired migration name

```powershell
dotnet ef migrations add <migration step> --project ".\src\BestiaryArenaCracker.Repository\BestiaryArenaCracker.Repository.csproj" --startup-project ".\src\BestiaryArenaCracker.Api\BestiaryArenaCracker.Api.csproj" -- --environment Development
```
