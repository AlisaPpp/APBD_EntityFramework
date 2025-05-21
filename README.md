To run the application, the appsettings.json file must have the following:
"ConnectionStrings": {
    "PersonalDatabase": "the connection string"
}

In the end, I decided to split my solution into 4 projects just like I've 
done for the previous tasks before. In my opinion, it just makes the project 
structure feel much cleaner and comfortable to navigate (e.g. if I have to revisit 
this project in the future (for the midterm or just as an example for other future tasks),
it'll be much easier to switch between layers)

Besides, I've also done it in consideration of possible expansion of the project in the future tasks, just
like we've done with the Devices project before, as the layered structure allows for
just that without having to worry about tight coupling too much.
Also, I'm trying to get used to the SOLID principles to get more practice with them, DI in particular (like 
I do here with injecting from Repositories into Services and from Services into API).