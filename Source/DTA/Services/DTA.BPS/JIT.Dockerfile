FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Services/DTA.BPS/DTA.BPS.csproj", "Services/DTA.BPS/"]
COPY ["Common/", "Common/"]
RUN dotnet restore "Services/DTA.BPS/DTA.BPS.csproj"

COPY . .
WORKDIR "/src/Services/DTA.BPS"
RUN dotnet build "DTA.BPS.csproj" -c Release-JIT -o /app/build

FROM build AS publish
RUN dotnet publish "DTA.BPS.csproj" -c Release-JIT -o /app/publish \
    -p:DebugType=None \
    -p:ExcludeFilesFromPublish=**/appsettings.Development.json

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BPS.dll"]