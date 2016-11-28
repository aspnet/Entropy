Config.Options.Snapshot.Web
=======
This sample demonstrates an IOptionsSnapshot being created on the first request after config.json changes. 
TimeOptions is bound to config.json, but TimeOptions won't be recreated unless the config file changes. 

1. Hit the server and verify the creation time is unchanged between requests. 
2. Modify config.json and the time will be updated on the next request.
