$language = if ($args[0] -ne $null) { $args[0] } else { "English" }
dotnet run --project src/ident.csproj --configuration release -- $language
