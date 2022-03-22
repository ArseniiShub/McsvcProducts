FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PlatformsService/PlatformsService.csproj", "PlatformsService/"]
RUN dotnet restore "PlatformsService/PlatformsService.csproj"
COPY . .
WORKDIR "/src/PlatformsService"
RUN dotnet build "PlatformsService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PlatformsService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlatformsService.dll"]

# docker build -f PlatformsService\Dockerfile -t eyndrahena/platforms-service .
# docker run -p 8080:80 -d eyndrahena/platforms-service
# docker push eyndrahena/platforms-service
