# Yeti

A public library.

## State of the app

### Yeti.Api

Based on core and db, the api is intended to permit users to add and edit manuscripts for others to view. This functionality works. It currently lacks any way for users to register and only permits logins with test/test. Also, tokens never, ever expire.

### yeti-remix

A front end based on react remix. So far, this lists all the stories found in api/recent and does exactly nothing else.

### yeti-search

Eventually intended to provide full-text indexing for manuscripts stored on yeti. Right now, its functionality is limited to printing the data found in certain fragments.
