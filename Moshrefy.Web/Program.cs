using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Moshrefy.Application.Authorization.Handlers;
using Moshrefy.Application.Authorization.Requirements;
using Moshrefy.Application.Helper;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Application.MappingProfiles;
using Moshrefy.Application.Services;
using Moshrefy.Application.Validators.AcademicYear;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Identity;
using Moshrefy.infrastructure.Configuration.DataSeeding;
using Moshrefy.infrastructure.Data;
using Moshrefy.infrastructure.Repositories;
using Moshrefy.infrastructure.TenantServices;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.infrastructure.UnitOfWork;
using System;
using System.Text;
using Moshrefy.Web.MappingProfiles;
using AcademicYearProfile = Moshrefy.Application.MappingProfiles.AcademicYearProfile;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateAcademicYearDTOValidator>();


// DbContext Configration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository registrations
builder.Services.AddScoped<IAcademicYearRepository, AcademicYearRepository>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<ICenterRepository, CenterRepository>();
builder.Services.AddScoped<IClassroomRepository, ClassroomRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IExamRepository, ExamRepository>();
builder.Services.AddScoped<IExamResultRepository, ExamResultRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<ITeacherCourseRepository, TeacherCourseRepository>();
builder.Services.AddScoped<ITeacherItemRepository, TeacherItemRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();


// Services registrations
builder.Services.AddScoped<IAcademicYearService, AcademicYearService>();
builder.Services.AddScoped<ICenterService, CenterService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<ISuperAdminService, SuperAdminService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IClassroomService, ClassroomService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<IExamResultService, ExamResultService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<ITeacherCourseService, TeacherCourseService>();
builder.Services.AddScoped<ITeacherItemService, TeacherItemService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IStudentService, StudentService>();


// Unit of Work registration
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// AutoMapper configuration
// For Application Mapping Profiles
builder.Services.AddAutoMapper(cfg => { }, typeof(AcademicYearProfile).Assembly);
// For Web Mapping Profiles
builder.Services.AddAutoMapper(cfg => { }, typeof(AuthProfile).Assembly);

// Identity configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
    };
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdminOnly", policy =>
        policy.RequireRole(RolesNames.SuperAdmin.ToString()));

    var entities = new[]
    {
        "Student", "Teacher", "Course", "Classroom", "AcademicYear",
        "Enrollment", "TeacherCourse", "TeacherItem", "Session",
        "Exam", "ExamResult", "Attendance", "Invoice", "Payment", "Item",
        "Center", "User"
    };

    var actions = new[] { "View", "Add", "Update", "Delete" };

    foreach (var entity in entities)
    {
        foreach (var action in actions)
        {
            options.AddPolicy($"{entity}.{action}", policy =>
                policy.Requirements.Add(new CenterAccessRequirement(entity, action)));
        }
    }
});

builder.Services.AddScoped<IAuthorizationHandler, CenterAccessHandler>();

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Run Identity Seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await IdentityDataSeeder.SeedAsync(userManager, roleManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

