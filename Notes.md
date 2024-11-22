Database design:
  * I created a Player, Casino and CasinoWager tables
  * Casino table stored game details because games exists within a Casino to the user
  * CasinoWager table stored details on each individual wager made by a player referencing information in Player and Casino tables
  * Created Stored procedures for Data manipulation and viewing
    
OT.Assessment.App and OT.Assessment.Consumer
  * Made use of the Repository pattern - Having the Data access layer separated from the Business Logic
  * Data access/Repository layer handled all interaction with the database.
  * Business Logic is where the services with additional logic was placed for formating response from the data retrieved
     and can be extended for other functionality in future 
  * Used Dependency Injection
  * All config was stored in appSettings.json files
  * Used Stored procedures to save and retrieve data over Entity framework for performance gain
  * SeriLog for logging - the logs are written to a Logs.txt file
    
Added 'Common' project 
  * This was added with intention to centralise entities that would be used across the projects in the solution
  * This helps with not repeating code in the different projects
