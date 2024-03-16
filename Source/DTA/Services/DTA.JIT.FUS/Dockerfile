FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["DTA.JIT.FUS/DTA.JIT.FUS.csproj", "DTA.JIT.FUS/"]
COPY ["DTA.JIT.Extensions/DTA.JIT.Extensions.csproj", "DTA.JIT.Extensions/"]
COPY ["DTA.Shared.Models/DTA.Shared.Models.csproj", "DTA.Shared.Models/"]
RUN dotnet restore "DTA.JIT.FUS/DTA.JIT.FUS.csproj"

COPY . .
WORKDIR "/src/DTA.JIT.FUS"
RUN dotnet build "DTA.JIT.FUS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DTA.JIT.FUS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DTA.JIT.FUS.dll"]
