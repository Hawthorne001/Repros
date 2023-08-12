# How to run:

1. docker compose up
2. dotnet run
3. Open https://localhost:7154/swagger/index.html#/default/PostLogin
4. Click on "Try it out"
5. Enter username and password (`test` and `Password123!`)
6. Set `CookieMode` (and `PersistCookies`? Recently added without any mention of what it does) to `true`
7. Click on "Execute"
8. Open https://localhost:7154/swagger/index.html#/default/GetAccountInfo
9. Click on "Try it out"
10. Click on "Execute"
11. Observe the exception