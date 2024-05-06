FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-jammy-chiseled as base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
RUN apt-get update && apt-get install -y clang zlib1g-dev
WORKDIR /src

COPY ["Services/DTA.SRS/DTA.SRS.csproj", "Services/DTA.SRS/"]
COPY ["Common/", "Common/"]
RUN dotnet restore "Services/DTA.SRS/DTA.SRS.csproj" -r linux-$TARGETARCH

# copy and publish app and libraries
COPY . .
WORKDIR "/src/Services/DTA.SRS"

FROM build AS publish
RUN dotnet publish "DTA.SRS.csproj" -c Release-AOT -o /app/publish -r linux-$TARGETARCH \
    -p:DebugType=None \
    -p:ExcludeFilesFromPublish=**/appsettings.Development.json

# Clean up
RUN find /app/publish -name '*.pdb' -exec rm -f {} + \
    && rm -f /app/publish/appsettings.Development.json

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#USER $APP_UID
ENTRYPOINT ["./SRS"]