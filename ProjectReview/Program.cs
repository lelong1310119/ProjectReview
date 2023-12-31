using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ProjectReview.Common;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Repositories;
using ProjectReview.Services;
using ProjectReview.Services.CategoryProfiles;
using ProjectReview.Services.Departments;
using ProjectReview.Services.Documents;
using ProjectReview.Services.DocumentTypes;
using ProjectReview.Services.JobProfiles;
using ProjectReview.Services.Jobs;
using ProjectReview.Services.PermissionGroups;
using ProjectReview.Services.Positions;
using ProjectReview.Services.Ranks;
using ProjectReview.Services.Users;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<DataContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddIdentity<User, Role, U>()
//		.AddEntityFrameworkStores<ApplicationDbContext>()
//		.AddDefaultTokenProviders();
//builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
//   .AddRoles<Role>()
//   .AddEntityFrameworkStores<DataContext>();
//builder.Services.AddDbContext<DataContext>(options =>
//options.UseSqlServer("Data Source=LAPTOP-TC1PJ34D\\LONG;Initial Catalog=Review;Integrated Security=True;TrustServerCertificate=True;"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<DbSeeder>();
builder.Services.AddSingleton<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<IRankService, RankService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDocumentTypeService, DocumentTypeService>();
builder.Services.AddScoped<IPermissionGroupService, PermissionGroupService>();
builder.Services.AddScoped<ICategoryProfileService, CategoryProfileService>();
builder.Services.AddScoped<IJobProfileRepository, JobProfileRepository>();
builder.Services.AddScoped<IJobProfileService, JobProfileService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IGenericService, GenericService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seeder = services.GetService<DbSeeder>();
    await DbSeeder.Migrate(services);
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
