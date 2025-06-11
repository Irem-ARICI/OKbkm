# 1. Base (çalışma zamanı) imajı
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 2. Build ortamı
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY . .

WORKDIR /src/OKbkm
RUN dotnet restore "OKbkm.csproj" --verbosity minimal
RUN dotnet build "OKbkm.csproj" -c $BUILD_CO	NFIGURATION -o /app/build

# 3. Publish aşaması
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OKbkm.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#COPY OKbkm/wwwroot /app/publish/wwwroot	//dockerda image olusturamadik diye bunu commentledik
#RUN ls -la /app/publish/wwwroot		#  Yayınlanan içeriği kontrol et

# 4. Final imaj
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "OKbkm.dll"]
