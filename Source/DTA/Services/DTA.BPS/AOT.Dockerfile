FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-jammy-chiseled as base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
RUN apt-get update && apt-get install -y clang zlib1g-dev
WORKDIR /src

COPY ["Services/DTA.BPS/DTA.BPS.csproj", "Services/DTA.BPS/"]
COPY ["Common/", "Common/"]
RUN dotnet restore "Services/DTA.BPS/DTA.BPS.csproj" -r linux-$TARGETARCH

# copy and publish app and libraries
COPY . .
WORKDIR "/src/Services/DTA.BPS"

FROM build AS publish
RUN dotnet publish "DTA.BPS.csproj" -r linux-$TARGETARCH -c Release-AOT -o /app/publish \
    -p:DebugType=None \
    -p:ExcludeFilesFromPublish=**/appsettings.Development.json

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#USER $APP_UID
ENTRYPOINT ["./BPS"]