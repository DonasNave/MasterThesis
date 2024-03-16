FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["DTA.JIT.SRS/DTA.JIT.SRS.csproj", "DTA.JIT.SRS/"]
COPY ["DTA.JIT.Extensions/DTA.JIT.Extensions.csproj", "DTA.JIT.Extensions/"]
COPY ["DTA.Shared.Models/DTA.Shared.Models.csproj", "DTA.Shared.Models/"]
RUN dotnet restore "DTA.JIT.SRS/DTA.JIT.SRS.csproj"

COPY . .
WORKDIR "/src/DTA.JIT.SRS"
RUN dotnet build "DTA.JIT.SRS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DTA.JIT.SRS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DTA.JIT.SRS.dll"]
