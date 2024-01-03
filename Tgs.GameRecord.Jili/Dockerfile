#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.Jili/Tgs.GameRecord.Jili.csproj", "Tgs.GameRecord.Jili/"]
COPY ["Tgs.GameRecord.Jili.Core/Tgs.GameRecord.Jili.Core.csproj", "Tgs.GameRecord.Jili.Core/"]
COPY ["Tgs.GameRecord.Jili.Common/Tgs.GameRecord.Jili.Common.csproj", "Tgs.GameRecord.Jili.Common/"]
RUN dotnet restore "./Tgs.GameRecord.Jili/./Tgs.GameRecord.Jili.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.Jili"
RUN dotnet build "./Tgs.GameRecord.Jili.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameRecord.Jili.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.Jili.dll"]