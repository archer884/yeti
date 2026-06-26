# One-shot EF Core migration runner. Applies pending migrations (incl. seed data) using the
# CONNECTION_STRING environment variable (see WriterContextFactory).
FROM mcr.microsoft.com/dotnet/sdk:10.0
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-ef
WORKDIR /src
COPY Yeti.Db/ Yeti.Db/
RUN dotnet restore Yeti.Db/Yeti.Db.csproj
ENTRYPOINT ["dotnet", "ef", "database", "update", "--project", "Yeti.Db"]
