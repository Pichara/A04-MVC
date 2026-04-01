# Dockerfile for A04-MVC (ASP.NET Core MVC)
# References:
# - https://docs.docker.com/engine/reference/builder/
# - https://learn.microsoft.com/dotnet/core/docker/

# Use official .NET 8.0 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["A04-MVC.csproj", "./"]
RUN dotnet restore "./A04-MVC.csproj"

# Copy all source files
COPY . .

# Build application
WORKDIR "/src"
RUN dotnet build "A04-MVC.csproj" -c Release -o /app/build

# Publish application
FROM build AS publish
RUN dotnet publish "A04-MVC.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use runtime image for final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "A04-MVC.dll"]
