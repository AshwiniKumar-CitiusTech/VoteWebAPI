FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ContestantService/ContestantService.csproj", "ContestantService/"]
RUN dotnet restore "ContestantService/ContestantService.csproj"
COPY . .
WORKDIR "/src/ContestantService"
RUN dotnet build "ContestantService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContestantService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContestantService.dll"]
