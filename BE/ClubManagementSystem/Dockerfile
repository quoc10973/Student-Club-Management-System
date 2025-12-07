# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY ClubManagementSystem.sln ./

# Copy csproj của từng project
COPY ClubManagementSystem.Repository/ClubManagementSystem.Repository.csproj ClubManagementSystem.Repository/
COPY ClubManagementSystem.Service/ClubManagementSystem.Service.csproj ClubManagementSystem.Service/
COPY ClubManagementSystem.API/ClubManagementSystem.API.csproj ClubManagementSystem.API/

# Restore nuget packages
RUN dotnet restore ClubManagementSystem.API/ClubManagementSystem.API.csproj

# Copy toàn bộ source code
COPY . .

# Set working dir sang project API
WORKDIR /src/ClubManagementSystem.API

# Build & publish
RUN dotnet publish -c Release -o /app

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
EXPOSE 8081
ENTRYPOINT ["dotnet", "ClubManagementSystem.API.dll"]
