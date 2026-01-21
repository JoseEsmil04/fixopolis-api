# ====== Build ======
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src


COPY Fixopolis.sln ./
COPY src/Fixopolis.WebApi/Fixopolis.WebApi.csproj src/Fixopolis.WebApi/
COPY src/Fixopolis.Domain/Fixopolis.Domain.csproj src/Fixopolis.Domain/
COPY src/Fixopolis.Persistence/Fixopolis.Persistence.csproj src/Fixopolis.Persistence/
COPY src/Fixopolis.Application/Fixopolis.Application.csproj src/Fixopolis.Application/


RUN dotnet restore ./Fixopolis.sln

COPY . .

RUN dotnet publish ./src/Fixopolis.WebApi/Fixopolis.WebApi.csproj -c Release -o /app/publish

# ====== Runtime ======
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV PORT=8080
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080
ENTRYPOINT ["dotnet", "Fixopolis.WebApi.dll"]
