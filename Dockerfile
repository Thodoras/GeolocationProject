FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

COPY *.sin .
COPY src/GeolocationAPI/*.csproj ./src/GeolocationAPI/
COPY Tests/*.csproj ./Tests/



