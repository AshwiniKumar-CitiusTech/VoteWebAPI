 # Use the official .NET SDK image to build the app
 FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
 WORKDIR /src

 # Copy csproj and restore as distinct layers
 COPY ContestantService.csproj ./
 RUN dotnet restore

 # Copy everything else and build
 COPY . ./
 RUN dotnet publish -c Release -o /app/out

 # Build runtime image
 FROM mcr.microsoft.com/dotnet/aspnet:8.0
 WORKDIR /app
 COPY --from=build-env /app/out .

 # Expose port
 EXPOSE 80

 # Set the entry point for the container
 ENTRYPOINT ["dotnet", "ContestantService.dll"]
 