language="$1"
if [ -z "$language" ]; then language="English"; fi
dotnet run --project src/ident.csproj --configuration release -- "$language"
