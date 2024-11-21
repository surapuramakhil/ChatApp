# Use the official .NET SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copy everything and restore as distinct layers
COPY . ./
RUN dotnet restore

# Build and publish a release version of the app
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose port 80 for HTTP traffic, port 443 for HTTPS traffic
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "ChatAppBackend.dll"]