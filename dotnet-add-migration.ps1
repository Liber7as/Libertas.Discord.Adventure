param ($migrationName, $context = "AdventureContext", $startupProject = "./Libertas.Discord.Adventure/")

# needs global ef tools installed - dotnet tool install --global dotnet-ef
dotnet ef migrations add $migrationName --startup-project $startupProject --context $context
