# 1. Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 8080

# 2. Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .

WORKDIR /src
RUN dotnet restore "OKbkm.csproj" --verbosity minimal
RUN dotnet build "OKbkm.csproj" -c Release -o /app/build
RUN dotnet publish "OKbkm.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 3. Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OKbkm.dll"]
