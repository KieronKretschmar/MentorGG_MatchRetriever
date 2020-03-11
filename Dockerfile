# ===============
# BUILD IMAGE
# ===============
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers

WORKDIR /app/MatchDb/MatchEntities/MatchEntities
COPY ./MatchDb/MatchEntities/MatchEntities/*.csproj ./
RUN dotnet restore

WORKDIR /app/MatchDb/Database
COPY ./MatchDb/Database/*.csproj ./
RUN dotnet restore

WORKDIR /app/EquipmentLib/EquipmentLib
COPY ./EquipmentLib/EquipmentLib/*.csproj ./
RUN dotnet restore

WORKDIR /app/ZoneReader/ZoneReader
COPY ./ZoneReader/ZoneReader/*.csproj ./
RUN dotnet restore

WORKDIR /app/MatchRetriever
COPY ./MatchRetriever/*.csproj ./
RUN dotnet restore

# Copy everything else and build
WORKDIR /app
COPY ./MatchDb/Database ./MatchDb/Database
COPY ./MatchDb/MatchEntities ./MatchDb/MatchEntities
COPY ./EquipmentLib/EquipmentLib ./EquipmentLib/EquipmentLib
COPY ./ZoneReader/ZoneReader ./ZoneReader/ZoneReader
COPY ./MatchRetriever/ ./MatchRetriever


RUN dotnet publish MatchRetriever/ -c Release -o out


# ===============
# RUNTIME IMAGE
# ===============
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app

COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "MatchRetriever.dll"]


# ===============
# SET ENVIRONMENT VARIABLES
# ===============
ENV ZONEREADER_RESOURCE_PATH /app/ZoneReader/ZoneReader/resources
ENV EQUIPMENT_CSV_DIRECTORY /app/EquipmentLib/EquipmentLib/EquipmentData