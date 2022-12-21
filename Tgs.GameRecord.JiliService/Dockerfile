#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.JiliService/Tgs.GameRecord.JiliService.csproj", "Tgs.GameRecord.JiliService/"]
COPY ["Tgs.GameRecord.JiliService.Core/Tgs.GameRecord.JiliService.Core.csproj", "Tgs.GameRecord.JiliService.Core/"]
RUN dotnet restore "Tgs.GameRecord.JiliService/Tgs.GameRecord.JiliService.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.JiliService"
RUN dotnet build "Tgs.GameRecord.JiliService.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameRecord.JiliService.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.JiliService.dll"]