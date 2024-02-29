using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// [FromForm]
app.MapPost("form-implicit-interface-parsable", ([FromForm] ImplicitlyInterfaceParsableForm model) => Results.Ok(model));
app.MapPost("form-explicit-interface-parsable", ([FromForm] ExplicitlyInterfaceParsableForm model) => Results.Ok(model));
app.MapPost("form-implicit-parsable", ([FromForm] ImplicitlyParsableForm model) => Results.Ok(model));
app.MapPost("form-not-parsable", ([FromForm] NotParsableForm model) => Results.Ok(model));
app.MapPost("form-implicit-parse-only", ([FromForm] ImplicitlyParseOnlyForm model) => Results.Ok(model));
app.MapPost("form-implicit-try-parse-only", ([FromForm] ImplicitlyTryParseOnlyForm model) => Results.Ok(model));

// [FromBody]
app.MapPost("body-implicit-interface-parsable", ([FromBody] ImplicitlyInterfaceParsableBody model) => Results.Ok(model));
app.MapPost("body-explicit-interface-parsable", ([FromBody] ExplicitlyInterfaceParsableBody model) => Results.Ok(model));
app.MapPost("body-implicit-parsable", ([FromBody] ImplicitlyParsableBody model) => Results.Ok(model));
app.MapPost("body-not-parsable", ([FromBody] NotParsableBody model) => Results.Ok(model));
app.MapPost("body-implicit-parse-only", ([FromBody] ImplicitlyParseOnlyBody model) => Results.Ok(model));
app.MapPost("body-implicit-try-parse-only", ([FromBody] ImplicitlyTryParseOnlyBody model) => Results.Ok(model));

// [FromQuery]
app.MapPost("query-implicit-interface-parsable", ([FromQuery] ImplicitlyInterfaceParsableQuery model) => Results.Ok(model));
app.MapPost("query-explicit-interface-parsable", ([FromQuery] ExplicitlyInterfaceParsableQuery model) => Results.Ok(model));
app.MapPost("query-implicit-parsable", ([FromQuery] ImplicitlyParsableQuery model) => Results.Ok(model));
// app.MapPost("query-not-parsable", ([FromQuery] NotParsableQuery model) => Results.Ok(model)); // Query can only be parsed by TryParse
// app.MapPost("query-implicit-parse-only", ([FromQuery] ImplicitlyParseOnlyQuery model) => Results.Ok(model)); // Query can only be parsed by TryParse
app.MapPost("query-implicit-try-parse-only", ([FromQuery] ImplicitlyTryParseOnlyQuery model) => Results.Ok(model));


app.Run();

public sealed record ImplicitlyInterfaceParsableForm : IParsable<ImplicitlyInterfaceParsableForm>
{
    public required string Name { get; init; }
    public required int Age { get; init; }

    public static ImplicitlyInterfaceParsableForm Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out ImplicitlyInterfaceParsableForm result)
    {
        throw new NotImplementedException();
    }
}

public sealed record ExplicitlyInterfaceParsableForm : IParsable<ExplicitlyInterfaceParsableForm>
{
    public string Name { get; init; }
    public int Age { get; init; }

    static ExplicitlyInterfaceParsableForm IParsable<ExplicitlyInterfaceParsableForm>.Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    static bool IParsable<ExplicitlyInterfaceParsableForm>.TryParse(string? s, IFormatProvider? provider,
        out ExplicitlyInterfaceParsableForm result)
    {
        throw new NotImplementedException();
    }
}

public sealed class ImplicitlyParsableForm
{
    public string Name { get; set; }
    public int Age { get; set; }

    public static ImplicitlyParsableForm Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out ImplicitlyParsableForm result)
    {
        throw new NotImplementedException();
    }
}

public sealed class ImplicitlyParseOnlyForm
{
    public string Name { get; set; }
    public int Age { get; set; }

    public static ImplicitlyParseOnlyForm Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }
}

public sealed class ImplicitlyTryParseOnlyForm
{
    public string Name { get; set; }
    public int Age { get; set; }

    public static bool TryParse(string? s, IFormatProvider? provider, out ImplicitlyTryParseOnlyForm result)
    {
        throw new NotImplementedException();
    }
}

public sealed class NotParsableForm
{
    public string Name { get; set; }
    public int Age { get; set; }
}



public sealed record ImplicitlyInterfaceParsableBody : IParsable<ImplicitlyInterfaceParsableBody>
{
    public required string Name { get; init; }
    public required int Age { get; init; }

    public static ImplicitlyInterfaceParsableBody Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out ImplicitlyInterfaceParsableBody result)
    {
        throw new NotImplementedException();
    }
}

public sealed record ExplicitlyInterfaceParsableBody : IParsable<ExplicitlyInterfaceParsableBody>
{
    public string Name { get; init; }
    public int Age { get; init; }

    static ExplicitlyInterfaceParsableBody IParsable<ExplicitlyInterfaceParsableBody>.Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    static bool IParsable<ExplicitlyInterfaceParsableBody>.TryParse(string? s, IFormatProvider? provider,
        out ExplicitlyInterfaceParsableBody result)
    {
        throw new NotImplementedException();
    }
}

public sealed class ImplicitlyParsableBody
{
    public string Name { get; set; }
    public int Age { get; set; }

    public static ImplicitlyParsableBody Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out ImplicitlyParsableBody result)
    {
        throw new NotImplementedException();
    }
}

public sealed class ImplicitlyParseOnlyBody
{
    public string Name { get; set; }
    public int Age { get; set; }

    public static ImplicitlyParseOnlyBody Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }
}

public sealed class ImplicitlyTryParseOnlyBody
{
    public string Name { get; set; }
    public int Age { get; set; }

    public static bool TryParse(string? s, IFormatProvider? provider, out ImplicitlyTryParseOnlyBody result)
    {
        throw new NotImplementedException();
    }
}

public sealed class NotParsableBody
{
    public string Name { get; set; }
    public int Age { get; set; }
}

public sealed record ImplicitlyInterfaceParsableQuery : IParsable<ImplicitlyInterfaceParsableQuery>
{
    public required string Name { get; init; }
    public required int Age { get; init; }

    public static ImplicitlyInterfaceParsableQuery Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out ImplicitlyInterfaceParsableQuery result)
    {
        throw new NotImplementedException();
    }
}

public sealed record ExplicitlyInterfaceParsableQuery : IParsable<ExplicitlyInterfaceParsableQuery>
{
    public string Name { get; init; }
    public int Age { get; init; }

    static ExplicitlyInterfaceParsableQuery IParsable<ExplicitlyInterfaceParsableQuery>.Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    static bool IParsable<ExplicitlyInterfaceParsableQuery>.TryParse(string? s, IFormatProvider? provider,
        out ExplicitlyInterfaceParsableQuery result)
    {
        throw new NotImplementedException();
    }
}

public sealed class ImplicitlyParsableQuery
{
    public string Name { get; set; }
    public int Age { get; set; }

    public static ImplicitlyParsableQuery Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out ImplicitlyParsableQuery result)
    {
        throw new NotImplementedException();
    }
}

public sealed class ImplicitlyTryParseOnlyQuery
{
    public string Name { get; set; }
    public int Age { get; set; }

    public static bool TryParse(string? s, IFormatProvider? provider, out ImplicitlyTryParseOnlyQuery result)
    {
        throw new NotImplementedException();
    }
}
