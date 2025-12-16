using HireFire.Grpc;
using LogicServer.Services;
using LogicServer.Services.Helper;
using ApplicantService = LogicServer.Services.ApplicantService;
using ApplicationService = LogicServer.Services.ApplicationService;
using AuthenticationService = LogicServer.Services.AuthenticationService;
using CompanyService = LogicServer.Services.CompanyService;
using JobListingService = LogicServer.Services.JobListingService;
using RecruiterService = LogicServer.Services.RecruiterService;
using RepresentativeService = LogicServer.Services.RepresentativeService;

var builder = WebApplication.CreateBuilder(args);

string chatServiceUrl = builder.Configuration["GrpcSettings:ChatServiceUrl"] ?? "https://localhost:9090";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();
builder.Services.AddSingleton<LogicServer.Services.RepresentativeService>();
builder.Services.AddSingleton<ApplicantService>();
builder.Services.AddSingleton<CompanyService>();
builder.Services.AddSingleton<AuthenticationService>();
builder.Services.AddSingleton<RecruiterService>();
builder.Services.AddSingleton<JobListingService>();
builder.Services.AddSingleton<ApplicationService>();
builder.Services.AddGrpcClient<ChatService.ChatServiceClient>(o =>
{
    o.Address = new Uri(chatServiceUrl); 
}).ConfigurePrimaryHttpMessageHandler(() => GrpcChannelHelper.GetSecureHandler());;
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapHub<HubChatService>("/hub/chat");
app.Run();