# Context: .
# Command: docker build -f server/src/FilterLists.Agent/Dockerfile .

# init base debug (for VS)
FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

# init base-prod
FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.1-alpine as base-prod
ENTRYPOINT ["./FilterLists.Agent"]
WORKDIR /app

# init build
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build

# restore
WORKDIR /app/src/FilterLists.Agent
COPY server/src/FilterLists.Agent/FilterLists.Agent.csproj .
RUN dotnet restore

# build
COPY server/src/FilterLists.Agent/. .
RUN dotnet build -c Release -r linux-musl-x64 --no-restore

# init Agent.Tests
FROM build AS test-agent
ENTRYPOINT ["dotnet", "test", "--logger:trx"]

# restore Agent.Tests
WORKDIR /app/tests/FilterLists.Agent.Tests
COPY server/tests/FilterLists.Agent.Tests/FilterLists.Agent.Tests.csproj .
RUN dotnet restore

# build Agent.Tests
COPY server/tests/FilterLists.Agent.Tests/. .
RUN dotnet build -c Release --no-restore

# publish
FROM build AS publish
RUN dotnet publish -c Release -r linux-musl-x64 -o out --no-restore --no-build

# final
FROM base-prod as final
COPY --from=publish /app/src/FilterLists.Agent/out .