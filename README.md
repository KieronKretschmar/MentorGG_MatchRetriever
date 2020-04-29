# MatchRetriever
Retrieve data from Match Database.

## Environment Variables
- `MYSQL_CONNECTION_STRING` : Connection string for Match Database. [*]
- `STEAMUSEROPERATOR_URI` : Address to SteamUserOperator. [*]
- `ZONEREADER_RESOURCE_PATH` : Path to directory containing ZoneReader's resources. [**]
- `EQUIPMENT_CSV_DIRECTORY` : Path to directory containing EquipmentLib's .csv resources. [**]
- `EQUIPMENT_ENDPOINT` : Optional Url to an endpoint supplying data used by EquipmentLib. 

[*] *Required*
[**] *Optional, Default is configured in Dockerfile*

## Additional Information
- Upon Startup, this project runs migrations on MatchDb.

- A SubscriptionConfig file is expected at `/app/config/subscriptions.json`
