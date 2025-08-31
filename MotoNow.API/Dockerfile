FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY NuGet.config ./

COPY MotoNow.API/*.csproj MotoNow.API/
COPY MotoNow.Application/*.csproj MotoNow.Application/
COPY MotoNow.Domain/*.csproj MotoNow.Domain/
COPY MotoNow.Infrastructure/*.csproj MotoNow.Infrastructure/

RUN dotnet restore MotoNow.API/MotoNow.API.csproj --configfile NuGet.config

COPY . .

RUN dotnet publish MotoNow.API/MotoNow.API.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENV TZ=America/Sao_Paulo
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "MotoNow.API.dll"]
