FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first (cached layer) using just the project files.
COPY Yeti.Db/Yeti.Db.csproj Yeti.Db/
COPY Yeti.Core/Yeti.Core.csproj Yeti.Core/
COPY Yeti.Api/Yeti.Api.csproj Yeti.Api/
RUN dotnet restore Yeti.Api/Yeti.Api.csproj

COPY Yeti.Db/ Yeti.Db/
COPY Yeti.Core/ Yeti.Core/
COPY Yeti.Api/ Yeti.Api/
RUN dotnet publish Yeti.Api/Yeti.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "Yeti.Api.dll"]
