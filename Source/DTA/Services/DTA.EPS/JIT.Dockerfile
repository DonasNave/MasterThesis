FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Services/DTA.EPS/DTA.EPS.csproj", "Services/DTA.EPS/"]
COPY ["Common/", "Common/"]
RUN dotnet restore "Services/DTA.EPS/DTA.EPS.csproj"

COPY . .
WORKDIR "/src/Services/DTA.EPS"
RUN dotnet build "DTA.EPS.csproj" -c Release-JIT -o /app/build

FROM build AS publish
RUN dotnet publish "DTA.EPS.csproj" -c Release-JIT -o /app/publish \
    -p:DebugType=None \
    -p:ExcludeFilesFromPublish=**/appsettings.Development.json

# Clean up
RUN find /app/publish -name '*.pdb' -exec rm -f {} + \
    && rm -f /app/publish/appsettings.Development.json

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EPS.dll"]