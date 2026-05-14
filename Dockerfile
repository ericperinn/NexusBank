FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY NexusBank.slnx ./
COPY NexusBank.Domain/NexusBank.Domain.csproj NexusBank.Domain/
COPY NexusBank.Application/NexusBank.Application.csproj NexusBank.Application/
COPY NexusBank.Infrastructure/NexusBank.Infrastructure.csproj NexusBank.Infrastructure/
COPY NexusBank.Api/NexusBank.Api.csproj NexusBank.Api/
COPY NexusBank/NexusBank.csproj NexusBank/

RUN dotnet restore NexusBank.Api/NexusBank.Api.csproj

COPY . .
RUN dotnet publish NexusBank.Api/NexusBank.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "NexusBank.Api.dll"]
