FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Services/DTA.FUS/DTA.FUS.csproj", "Services/DTA.FUS/"]
COPY ["Common/", "Common/"]
RUN dotnet restore "Services/DTA.FUS/DTA.FUS.csproj"

COPY . .
WORKDIR "/src/Services/DTA.FUS"
RUN dotnet build "DTA.FUS.csproj" -c Release-JIT -o /app/build

FROM build AS publish
RUN dotnet publish "DTA.FUS.csproj" -c Release-JIT -o /app/publish \
    -p:DebugType=None \
    -p:ExcludeFilesFromPublish=**/appsettings.Development.json

# Clean up
RUN find /app/publish -name '*.pdb' -exec rm -f {} + \
    && rm -f /app/publish/appsettings.Development.json

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FUS.dll"]