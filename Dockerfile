# Use the official .NET 8 runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["CQCDMS.csproj", "./"]
RUN dotnet restore "CQCDMS.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src"
RUN dotnet build "CQCDMS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CQCDMS.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create uploads directory for file storage
RUN mkdir -p /app/wwwroot/uploads/faxes

# Set environment variables for production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Create a non-root user for security
RUN addgroup --system --gid 1001 dotnet
RUN adduser --system --uid 1001 --gid 1001 dotnet
RUN chown -R dotnet:dotnet /app
USER dotnet

ENTRYPOINT ["dotnet", "CQCDMS.dll"]