#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Robots.API/Robots.API.csproj", "src/Robots.API/"]
COPY ["src/Robots.Core/Robots.Core.csproj", "src/Robots.Core/"]
COPY ["src/Robots.Data/Robots.Data.csproj", "src/Robots.Data/"]
COPY ["src/Robots.Domain/Robots.Domain.csproj", "src/Robots.Domain/"]
RUN dotnet restore "src/Robots.API/Robots.API.csproj"
COPY . .
WORKDIR "/src/src/Robots.API"
RUN dotnet build "Robots.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Robots.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Robots.API.dll"]
