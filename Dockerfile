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
COPY ./MatchRetriever/ ./MatchRetriever
COPY ./EquipmentLib/EquipmentLib ./EquipmentLib/EquipmentLib
COPY ./ZoneReader/ZoneReader ./ZoneReader/ZoneReader

RUN dotnet publish MatchRetriever/ -c Release -o out


# ===============
# RUNTIME IMAGE
# ===============
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime

# Runtime resources
WORKDIR /app/data
COPY ./EquipmentLib/EquipmentLib/EquipmentData ./EquipmentData
COPY ./ZoneReader/ZoneReader/resources ./ZoneData

COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "MatchRetriever.dll"]


# ===============
# SET ENVIRONMENT VARIABLES
# ===============
ENV ZONEREADER_RESOURCE_PATH /app/data/ZoneData
ENV EQUIPMENT_CSV_DIRECTORY /app/data/EquipmentData
