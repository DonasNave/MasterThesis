FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-jammy-chiseled as base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
RUN apt-get update && apt-get install -y clang zlib1g-dev
WORKDIR /src

COPY ["Services/DTA.EPS/DTA.EPS.csproj", "Services/DTA.EPS/"]
COPY ["Common/", "Common/"]
RUN dotnet restore "Services/DTA.EPS/DTA.EPS.csproj" -r linux-$TARGETARCH

# copy and publish app and libraries
COPY . .
WORKDIR "/src/Services/DTA.EPS"

FROM build AS publish
RUN dotnet publish "DTA.EPS.csproj" -c Release-AOT -o /app/publish \
    -p:DebugType=None \
    -p:ExcludeFilesFromPublish=**/appsettings.Development.json

# Clean up
RUN find /app/publish -name '*.pdb' -exec rm -f {} + \
    && rm -f /app/publish/appsettings.Development.json

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#USER $APP_UID
ENTRYPOINT ["./EPS"]