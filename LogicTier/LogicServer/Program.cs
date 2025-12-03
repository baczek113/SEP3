using HireFire.Grpc;
using LogicServer.Services;
using ApplicantService = LogicServer.Services.ApplicantService;
using ApplicationService = LogicServer.Services.ApplicationService;
using AuthenticationService = LogicServer.Services.AuthenticationService;
using CompanyService = LogicServer.Services.CompanyService;
using JobListingService = LogicServer.Services.JobListingService;
using RecruiterService = LogicServer.Services.RecruiterService;
using RepresentativeService = LogicServer.Services.RepresentativeService;

var builder = WebApplication.CreateBuilder(args);

string chatServiceUrl = builder.Configuration["GrpcSettings:ChatServiceUrl"] ?? "http://localhost:9090";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();
builder.Services.AddSingleton<ApplicationService>();
builder.Services.AddSingleton<RepresentativeService>();
builder.Services.AddSingleton<ApplicantService>();
builder.Services.AddSingleton<CompanyService>();
builder.Services.AddSingleton<AuthenticationService>();
builder.Services.AddSingleton<RecruiterService>();
builder.Services.AddSingleton<JobListingService>();
builder.Services.AddGrpcClient<ChatService.ChatServiceClient>(o =>
{
    o.Address = new Uri(chatServiceUrl); 
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.MapHub<HubChatService>("/chathub");

app.Run();