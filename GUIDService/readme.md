Simple REST service for generating GUIDs

I mainly wrote this because I wanted to see what's the absolute fastest a web service can be using WebAPI (and still doing something, not just returning a constant string).
On my machine, this service returns a million GUIDs in a second and a half, as measured by the MVC application.
The total time to display the results (only the first 10 GUIDs) is 3 seconds.
