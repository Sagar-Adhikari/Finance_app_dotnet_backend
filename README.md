# Readme

The server side uses IdentityServer 4 to manage access control over the server resources.
https://identityserver.io/

SQL server is used as backend database.

Database Connection
---------------------
Ensure you have the right connection in each `appSettings.json` file for each proejct.

Run Migrations For ASP.NET Identity Tables
-------------------------------------------
`dotnet ef database update --context "AppDbContext"`


Run Migrations For IdentityServer Token Tables
-------------------------------------------
`dotnet ef database update --context "PersistedGrantDbContext"`


Solution has two main projects:
1) IdentityServer
2) RoleManager.API

Both the projects has to be running/deployed for the client app to work.
Also, the IdentityServer needs to be running on HTTPS.



Seed User
--------------
A super user is initally seeded from `IdentityServer/Program.cs` file.
