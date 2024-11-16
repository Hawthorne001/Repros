var builder = WebApplication.CreateSlimBuilder(args);
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// app.UseSwagger();
app.MapSwagger();

app.Run();