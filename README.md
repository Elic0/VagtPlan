# VagtPlan

Application for creating and viewing shifts, managing remote and non work days
all in one place.

## Info
The website is supposed to make your workday easier, by giving you a simpler way,
of managing your workers schedules and workflow. Allowing you to input
work from home, sick and vacation days. So you can easily generate a schedule,
based on the availability of your workers.

This application might include a future version, with an app based in Flutter.
This app is supposed to allow the user/worker to, quickly view their schedule, or
call in sick for the day. Along with requesting, work from home or vacation days.

More features are to be decided, but can also be requested through GitHub issues.

## How it works:
First select in the calendar view, which department and minimum required physical
workers, there needs to be. Then select the days that you wish to, generate the
schedule from and to.

When generating a work schedule, you have to beforehand make sure that you have,
input all of your workers wishes, like their work from home and vacation days.
This can be done through, the worker overview page.

Upon generating the schedule, you will get a calendar view, showing which days
now have a planned schedule ready. The days are clickable, so you can view who
has been assigned, to the selected day.

Knowing there might be sudden changes, like someone being sick, or emergencies
needing a work from home day, you can then input this into another page, saying
\[ExampleWorkerName] sick, which will give you a list, of possible substitutes 
basing the list on, travel time and previous substitute shifts.

## Technologies
This application runs in Dotnet with Aspire and Blazor setup.
Backend is written in C# with EFCore for DB management.
Frontend is Blazor with Bootstrap.
