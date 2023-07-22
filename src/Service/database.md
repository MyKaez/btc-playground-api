# crate migrations
dotnet ef migrations add InitialSetup -- --"connection string"

dotnet ef migrations script -- --"connection string"

# remove migrations

dotnet ef migrations remove -- "connection string"

# update database
dotnet ef database update -- "connection string"


# clear tables
```sql

SELECT DISTINCT s.ID AS Session, u.ID AS [User], i.ID AS Interaction, c.Id Connection
INTO #toClear
FROM [Sessions] s
    JOIN [Interactions] i ON i.SessionID = s.ID
    JOIN Users u ON u.ID = i.UserID
    JOIN Connections c ON c.SessionID = s.ID
WHERE 1=1
  AND s.Name <> 'Blocktrainer'

DELETE FROM Interactions
WHERE ID IN (SELECT Interaction from #toClear)

DELETE FROM Users
WHERE ID IN (SELECT [User] from #toClear)

DELETE FROM Connections
WHERE ID IN (SELECT Connection from #toClear)

DELETE FROM Sessions
--WHERE Name <> 'Blocktrainer'
WHERE ID IN (SELECT Session from #toClear)

DROP TABLE #toClear
```