# ====== Build ======
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore ./Fixopolis.sln
RUN dotnet publish ./src/Fixopolis.WebApi/Fixopolis.WebApi.csproj -c Release -o /app

# ====== Runtime ======
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Puerto standard para free tiers y plataformas PaaS
ENV PORT=8080
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

# <--- IMPORTANTE: nombre del dll del proyecto WebApi --->
ENTRYPOINT ["dotnet", "Fixopolis.WebApi.dll"]
