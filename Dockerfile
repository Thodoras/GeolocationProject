FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln .
COPY src/GeolocationAPI/*.csproj ./src/GeolocationAPI/
COPY tests/GeolocationAPI.Tests/*.csproj ./tests/GeolocationAPI.Tests/

RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_HTTPS_PORT=0

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "GeolocationAPI.dll"]


