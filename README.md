To run the application, the appsettings.json file must have the following:
"ConnectionStrings": {
    "PersonalDatabase": "the connection string"
}
The following must be in the appsettings.Development.json file:
"Jwt": {
    "Issuer": "http://localhost:5300",
    "Audience": "http://localhost:5300",
    "Key": "the key",
    "ValidInMinutes": 10
},
"ConnectionStrings": {
    "PersonalDatabase": "the connection string"
}

I slightly changed the structure of the solution to only contain 
3 projects, as Repositories project was redundant. DbContext serves as the 
repository. 