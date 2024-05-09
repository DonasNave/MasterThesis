FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-jammy-chiseled as base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src

COPY ["Services/DTA.BPS/DTA.BPS.csproj", "Services/DTA.BPS/"]
COPY ["Common/", "Common/"]
RUN dotnet restore "Services/DTA.BPS/DTA.BPS.csproj"

COPY . .
WORKDIR "/src/Services/DTA.BPS"

FROM build AS publish
RUN dotnet publish "DTA.BPS.csproj" -c Release-JIT -o /app/publish -r linux-$TARGETARCH --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./BPS"]
