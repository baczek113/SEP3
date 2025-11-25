using LogicServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<RepresentativeService>();
builder.Services.AddSingleton<ApplicantService>();
builder.Services.AddSingleton<CompanyService>();
builder.Services.AddSingleton<AuthenticationService>();
builder.Services.AddSingleton<RecruiterService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();