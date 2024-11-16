using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<FormSendingService>();

var app = builder.Build();

app.MapPost("formfilecollection", ([FromForm] WithMultipleFormFileCollection model, HttpRequest request) =>
{
    if (model is null)
    {
        return "Model is null";
    }
    
    var requestFormFileCount = request.Form.Files.Count;
    var modelImportantFilesCount = model.ImportantFiles.Count;
    var modelUnimportantFilesCount = model.UnimportantFiles.Count;

    return $"""
           Total files in request: {requestFormFileCount}
           Important files in model: {modelImportantFilesCount}
           Unimportant files in model: {modelUnimportantFilesCount}
           """;
}).WithOpenApi().DisableAntiforgery();

app.MapPost("readonlylist", ([FromForm] WithMultipleReadOnlyListOfFormFile model, HttpRequest request) =>
{
    if (model is null)
    {
        return "Model is null";
    }
    
    var requestFormFileCount = request.Form.Files.Count;
    var modelImportantFilesCount = model.ImportantFiles?.Count;
    var modelUnimportantFilesCount = model.UnimportantFiles?.Count;

    return $"""
           Total files in request: {requestFormFileCount}
           Important files in model: {modelImportantFilesCount}
           Unimportant files in model: {modelUnimportantFilesCount}
           """;
}).WithOpenApi().DisableAntiforgery();

app.MapPost("list", ([FromForm]WithMultipleListOfFormFile model, HttpRequest request) =>
{
    if (model is null)
    {
        return "Model is null";
    }
    
    if (model.ImportantFiles is null)
    {
        return "Important files is null";
    }
    
    if (model.UnimportantFiles is null)
    {
        return "Unimportant files is null";
    }
    
    var requestFormFileCount = request.Form.Files.Count;
    var modelImportantFilesCount = model.ImportantFiles.Count;
    var modelUnimportantFilesCount = model.UnimportantFiles.Count;

    return $"""
           Total files in request: {requestFormFileCount}
           Important files in model: {modelImportantFilesCount}
           Unimportant files in model: {modelUnimportantFilesCount}
           """;
}).WithOpenApi().DisableAntiforgery();

app.Run();

abstract class DummyForm
{
    // See https://github.com/dotnet/aspnetcore/issues/54130 for why this is needed
    public IFormFileCollection DummyCollection { get; set; }
}

// the base class is technically not needed here, but consistent with the other examples
class WithMultipleFormFileCollection : DummyForm
{
    public IFormFileCollection ImportantFiles { get; set; }
    public IFormFileCollection UnimportantFiles { get; set; }
}

class WithMultipleReadOnlyListOfFormFile : DummyForm
{
    public IReadOnlyList<IFormFile> ImportantFiles { get; }
    public IReadOnlyList<IFormFile> UnimportantFiles { get; }
}

class WithMultipleListOfFormFile : DummyForm
{
    public List<IFormFile> ImportantFiles { get; set; }
    public List<IFormFile> UnimportantFiles { get; set; }
}

class FormSendingService : BackgroundService
{
    private static MultipartFormDataContent CreateFormContent()
    {
        
        var content = new MultipartFormDataContent();
        content.Add(
            new StringContent("important text 1"), 
            nameof(WithMultipleFormFileCollection.ImportantFiles), 
            "file1.txt"
            );
        content.Add(
            new StringContent("important text 2"),
            nameof(WithMultipleFormFileCollection.ImportantFiles),
            "file2.txt"
            );
        content.Add(
            new StringContent("unimportant text 1"),
            nameof(WithMultipleFormFileCollection.UnimportantFiles),
            "file3.txt"
            );
        content.Add(
            new StringContent("unimportant text 2"),
            nameof(WithMultipleFormFileCollection.UnimportantFiles),
            "file4.txt"
            );
        return content;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait for the server to start
        await Task.Delay(1000, stoppingToken);
        const string baseUri = "http://localhost:5206"; 
        var httpClient = new HttpClient();
        Console.WriteLine();
        
        Console.WriteLine("Sending form file collection");
        var formFileCollectionResponse = await httpClient.PostAsync(baseUri + "/formfilecollection", CreateFormContent(), stoppingToken);
        var formFileCollectionResponseContent = await formFileCollectionResponse.Content.ReadAsStringAsync(stoppingToken);
        Console.WriteLine(formFileCollectionResponseContent);
        Console.WriteLine();
        
        Console.WriteLine("Sending readonly list");
        var readonlyListResponse = await httpClient.PostAsync(baseUri + "/readonlylist", CreateFormContent(), stoppingToken);
        var readonlyListResponseContent = await readonlyListResponse.Content.ReadAsStringAsync(stoppingToken);
        Console.WriteLine(readonlyListResponseContent);
        Console.WriteLine();
        
        Console.WriteLine("Sending list");
        var listResponse = await httpClient.PostAsync(baseUri + "/list", CreateFormContent(), stoppingToken);
        var listResponseContent = await listResponse.Content.ReadAsStringAsync(stoppingToken);
        Console.WriteLine(listResponseContent);
        Console.WriteLine();
        
        // quit the app
        throw new Exception("Done");
    }
}