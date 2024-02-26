### Activity Testing

A simple repro of how unit testing the `Activity` class in .Net 8 is not working as expected.


### Steps to reproduce

1. Clone the repo
2. `dotnet test`
3. Notice that the tests... works
4. Change the `UseAspNetCoreInstrumentation` to `true` in the `appsettings.json` file. Leave the `UseAzureMonitor` as `true`
5. `dotnet test`
6. Notice that the tests... still works
7. Change the `UseAspNetCoreInstrumentation` to `false` in the `appsettings.json` file, and the `UseAzureMonitor` to `true`
8. `dotnet test`
9. Notice that the tests... no longer works
10. Run a single test. I do not know how to do this in the command line, but if you open an IDE (For example, the `Unit Tests` tool window in `JetBrains Rider`) and run a single test, you will notice that it works.
11. Change the `UseAspNetCoreInstrumentation` to `true` in the `appsettings.json` file. Leave the `UseAzureMonitor` as `true`
12. `dotnet test`
13. Notice that the tests... still don't work.
14. Run a single test again. You will notice that it still works.
15. Change the `UseAzureMonitor` to `false` in the `appsettings.json` file. Leave the `UseAspNetCoreInstrumentation` as `true`
16. `dotnet test`
17. Notice that the tests... works again.
18. Change the `UseAspNetCoreInstrumentation` to `false` in the `appsettings.json` file. Leave the `UseAzureMonitor` as `false`
19. `dotnet test`
20. Notice that the tests... still works