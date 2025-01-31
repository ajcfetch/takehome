# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set working directory inside container
WORKDIR /app

# Copy project files and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy remaining project files and build the application
COPY . ./
RUN dotnet publish -c Release -o /out

# Use a smaller runtime image for final execution
FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app
COPY --from=build /out .

# Expose the necessary port (change based on your app)
EXPOSE 5074

# Set the entry point to start the application
ENTRYPOINT ["dotnet", "FetchReceiptProcessor.dll"]
