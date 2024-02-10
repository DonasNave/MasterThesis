FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["JIT.APP.SRS/JIT.APP.SRS.csproj", "JIT.APP.SRS/"]
COPY ["JIT.APP.Extensions/JIT.APP.Extensions.csproj", "JIT.APP.Extensions/"]
COPY ["JIT.APP.Models/JIT.APP.Models.csproj", "JIT.APP.Models/"]
RUN dotnet restore "JIT.APP.SRS/JIT.APP.SRS.csproj"

COPY . .
WORKDIR "/src/JIT.APP.SRS"
RUN dotnet build "JIT.APP.SRS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JIT.APP.SRS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JIT.APP.SRS.dll"]
