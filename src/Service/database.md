﻿# crate migrations
dotnet ef migrations add InitialSetup -- --"connection string"

dotnet ef migrations script -- --"connection string"

# update database
dotnet ef database update -- "connection string"

