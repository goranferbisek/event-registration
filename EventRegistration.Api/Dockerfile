FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY EventRegistration.Api/EventRegistration.Api.csproj EventRegistration.Api/
RUN dotnet restore EventRegistration.Api/EventRegistration.Api.csproj
COPY EventRegistration.Api/ EventRegistration.Api/
RUN dotnet publish EventRegistration.Api/EventRegistration.Api.csproj -c Release -o /app/publish --no-restore

FROM build AS bundle
RUN dotnet tool install --tool-path /tools dotnet-ef --version 10.0.15
RUN /tools/dotnet-ef migrations bundle \
        --project EventRegistration.Api/EventRegistration.Api.csproj \
        --startup-project EventRegistration.Api/EventRegistration.Api.csproj \
        --configuration Release -o /app/efbundle --force

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS migrator
WORKDIR /app
COPY --from=bundle /app/efbundle ./efbundle
ENTRYPOINT ["./efbundle"]

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS api
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EventRegistration.Api.dll"]

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS test
WORKDIR /src
COPY EventRegistration.Api/EventRegistration.Api.csproj EventRegistration.Api/
COPY EventRegistration.Api.Tests/EventRegistration.Api.Tests.csproj EventRegistration.Api.Tests/
RUN dotnet restore EventRegistration.Api.Tests/EventRegistration.Api.Tests.csproj
COPY . .
RUN dotnet build EventRegistration.Api.Tests/EventRegistration.Api.Tests.csproj --no-restore
ENTRYPOINT ["dotnet", "test", "EventRegistration.Api.Tests/EventRegistration.Api.Tests.csproj", "--no-build"]
