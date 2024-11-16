### RequestDelegateGenerator requires the object to implement `IParsable<T>` or otherwise have a `TryParse` method.

[Link to GitHub issue on dotnet/aspnetcore](https://github.com/dotnet/aspnetcore/issues/54130)

## Steps to reproduce

1. Clone the repository
2. Build the solution
3. Notice the following errors:
* `Cannot implicitly convert type 'string' to 'NotParsableForm'`
* `Cannot implicitly convert type 'string' to 'ImplicitlyParseOnlyForm'`

Additionally, notice how it only affects `[FromForm]`, and not `[FromBody]` or `[FromQuery]`.
