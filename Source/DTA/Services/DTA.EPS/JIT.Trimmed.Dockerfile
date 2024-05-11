FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-jammy-chiseled as base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src

COPY ["Services/DTA.EPS/DTA.EPS.csproj", "Services/DTA.EPS/"]
COPY ["Common/", "Common/"]
RUN dotnet restore "Services/DTA.EPS/DTA.EPS.csproj"

COPY . .
WORKDIR "/src/Services/DTA.EPS"

FROM build AS publish
RUN dotnet publish "DTA.EPS.csproj" -c Release-JIT -o /app/publish -r linux-$TARGETARCH --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./EPS"]
