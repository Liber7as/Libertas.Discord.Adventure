param ($context = "AdventureContext", $startupProject = "./Libertas.Discord.Adventure/")

dotnet ef database update --startup-project $startupProject --context $context --verbose
