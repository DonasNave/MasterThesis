# Alternative method
FROM alpine:3.18 AS prepare
WORKDIR /app
RUN adduser -u 1000 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine3.18 AS build
RUN apk update && apk upgrade
RUN apk add --no-cache clang build-base zlib-dev
WORKDIR /code
COPY ./AoT.APP.FUS.csproj .
ARG RUNTIME_ID=linux-musl-x64
RUN dotnet restore -r $RUNTIME_ID
COPY . .
RUN dotnet publish \
    -c Release \
    -r $RUNTIME_ID \
    -o /app \
    --no-restore

FROM prepare AS final
COPY --chown=appuser --from=build /app/AoT.APP.FUS ./AoT.APP.FUS
ENTRYPOINT ["./AoT.APP.FUS"]

#####
# Alternative method
# syntax=docker/dockerfile:1
# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# ARG TARGETARCH
# RUN <<EOF
# apt-get update
# apt-get install -y clang zlib1g-dev
# EOF
# WORKDIR /source

# # copy csproj and restore as distinct layers
# COPY *.csproj .
# COPY *.config .
# RUN dotnet restore -r linux-$TARGETARCH

# # copy and publish app and libraries
# COPY . .
# RUN dotnet publish --no-restore -o /app AoT.APP.FUS.csproj
# RUN rm /app/*.dbg /app/*.Development.json


# # final stage/image
# FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-jammy-chiseled
# WORKDIR /app
# COPY --from=build /app .
# USER $APP_UID
# ENTRYPOINT ["./AoT.APP.FUS"]
