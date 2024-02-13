# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
RUN <<EOF
apt-get update
apt-get install -y clang zlib1g-dev
EOF
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.csproj .
COPY *.config .
RUN dotnet restore -r linux-$TARGETARCH

# copy and publish app and libraries
COPY . .
RUN dotnet publish --no-restore -o /app SRS.csproj
RUN rm /app/*.dbg /app/*.Development.json


# final stage/image
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-jammy-chiseled
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENTRYPOINT ["./SRS"]