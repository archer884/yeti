# Build the author SPA (yeti-svelte) into Yeti.Web/wwwroot/author.
FROM node:24-alpine AS spa
WORKDIR /yeti-svelte
COPY yeti-svelte/package*.json ./
RUN npm ci
COPY yeti-svelte/ ./
RUN mkdir -p /Yeti.Web/wwwroot/author && npm run build

# Build the .NET reader site, layered with the SPA output.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Yeti.Db/Yeti.Db.csproj Yeti.Db/
COPY Yeti.Core/Yeti.Core.csproj Yeti.Core/
COPY Yeti.Web/Yeti.Web.csproj Yeti.Web/
RUN dotnet restore Yeti.Web/Yeti.Web.csproj

COPY Yeti.Db/ Yeti.Db/
COPY Yeti.Core/ Yeti.Core/
COPY Yeti.Web/ Yeti.Web/
COPY --from=spa /Yeti.Web/wwwroot/author ./Yeti.Web/wwwroot/author
RUN dotnet publish Yeti.Web/Yeti.Web.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "Yeti.Web.dll"]
