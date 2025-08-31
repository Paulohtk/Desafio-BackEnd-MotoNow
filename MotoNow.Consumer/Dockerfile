
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY NuGet.config ./
ENV NUGET_PACKAGES=/root/.nuget/packages
ENV NUGET_FALLBACK_PACKAGES=   

COPY MotoNow.Consumer/*.csproj MotoNow.Consumer/
COPY MotoNow.API/*.csproj MotoNow.API/
COPY MotoNow.Application/*.csproj MotoNow.Application/
COPY MotoNow.Domain/*.csproj MotoNow.Domain/
COPY MotoNow.Infrastructure/*.csproj MotoNow.Infrastructure/

RUN dotnet restore MotoNow.Consumer/MotoNow.Consumer.csproj --configfile NuGet.config

COPY . .

RUN rm -rf MotoNow.Consumer/bin MotoNow.Consumer/obj \
           MotoNow.API/bin MotoNow.API/obj \
           MotoNow.Application/bin MotoNow.Application/obj \
           MotoNow.Domain/bin MotoNow.Domain/obj \
           MotoNow.Infrastructure/bin MotoNow.Infrastructure/obj

RUN dotnet restore MotoNow.Consumer/MotoNow.Consumer.csproj --configfile NuGet.config

RUN dotnet publish MotoNow.Consumer/MotoNow.Consumer.csproj -c Release -o /app --no-restore


FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app ./
ENV TZ=America/Sao_Paulo
ENTRYPOINT ["dotnet", "MotoNow.Consumer.dll"]
