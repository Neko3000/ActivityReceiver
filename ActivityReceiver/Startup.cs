using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ActivityReceiver.Data;
using ActivityReceiver.Models;
using ActivityReceiver.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ActivityReceiver.ViewModels;
using ActivityReceiver.Functions;
using ActivityReceiver.DataBuilders;

namespace ActivityReceiver
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ApplicationDbContextConnection")));

            services.AddDbContext<ActivityReceiverDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ActivityReceiverDbContextConnection")));

            // Change the policy of password
            services.AddIdentity<ApplicationUser, IdentityRole>(options => {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // JWT
            services.AddAuthentication().
                AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"]))
                    };
                });

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IDbContextInitializer, DbContextInitializer>();

            //services.AddScoped<AnswerManageDataBuilder>();

            services.AddMvc();

            // Authorize service
            services.ConfigureApplicationCookie(options => options.LoginPath = "/Account/Login");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbContext appDbContext, ActivityReceiverDbContext arDbContext, IDbContextInitializer dbContextInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Initialize Database 
            dbContextInitializer.ApplicationDbContextInitialize().Wait();
            dbContextInitializer.MainDbContextInitialize().Wait();

            // Auto Mapper
            AutoMapper.Mapper.Initialize(cfg => {

                /* QuestionManage */
                // Index
                cfg.CreateMap<Question, ActivityReceiver.ViewModels.QuestionManage.QuestionPresenter>();
                // Create
                cfg.CreateMap<ActivityReceiver.ViewModels.QuestionManage.QuestionManageCreatePostViewModel, Question>();
                cfg.CreateMap<ActivityReceiver.ViewModels.QuestionManage.QuestionManageCreatePostViewModel, ActivityReceiver.ViewModels.QuestionManage.QuestionManageCreateGetViewModel>();
                // Edit
                cfg.CreateMap<Question, ActivityReceiver.ViewModels.QuestionManage.QuestionManageEditGetViewModel>();
                cfg.CreateMap<ActivityReceiver.ViewModels.QuestionManage.QuestionManageEditPostViewModel, Question>()
                    .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.EditorID, opt => opt.Ignore());
                cfg.CreateMap<ActivityReceiver.ViewModels.QuestionManage.QuestionManageEditPostViewModel, ActivityReceiver.ViewModels.QuestionManage.QuestionManageEditGetViewModel>();
                // Details
                cfg.CreateMap<Question, ActivityReceiver.ViewModels.QuestionManage.QuestionManageDetailsViewModel>();
                // Delete
                cfg.CreateMap<Question, ActivityReceiver.ViewModels.QuestionManage.QuestionManageDeleteGetViewModel>();
                cfg.CreateMap<ActivityReceiver.ViewModels.QuestionManage.QuestionManageDeletePostViewModel, ActivityReceiver.ViewModels.QuestionManage.QuestionManageDeleteGetViewModel>();

                /* AnswerRepaly */
                //cfg.CreateMap<Movement,ActivityReceiver.ViewModels.AnswerReplay.>();
                //cfg.CreateMap<DeviceAcceleration, DeviceAccelerationDTO>();


                /* QuestionAnswerManage*/
                cfg.CreateMap<AnswerRecord, ActivityReceiver.ViewModels.AnswerManage.AnswerRecordPresenter>();
                // Details
                cfg.CreateMap<AnswerRecord, ActivityReceiver.ViewModels.AnswerManage.AnswerRecordManageDetailsViewModel>();


                /* AssignmentMange */
                cfg.CreateMap<AnswerRecord, ActivityReceiver.ViewModels.AssignmentRecordManage.AnswerPresenter>();
                // Index
                cfg.CreateMap<AssignmentRecord, ActivityReceiver.ViewModels.AssignmentRecordManage.AssignmentRecordPresenter>();
                // Details
                cfg.CreateMap<AssignmentRecord, ActivityReceiver.ViewModels.AssignmentRecordManage.AssignmentRecordManageDetailsViewModel>();

                /* ExerciseManage */
                // Index
                cfg.CreateMap<Exercise, ActivityReceiver.ViewModels.ExerciseManage.ExercisePresenter>();
                // Create
                cfg.CreateMap<ActivityReceiver.ViewModels.ExerciseManage.ExerciseManageCreatePostViewModel, Exercise>();
                cfg.CreateMap<ActivityReceiver.ViewModels.ExerciseManage.ExerciseManageCreatePostViewModel, ActivityReceiver.ViewModels.ExerciseManage.ExerciseManageCreateGetViewModel>();
                // Edit
                cfg.CreateMap<Exercise, ActivityReceiver.ViewModels.ExerciseManage.ExerciseManageEditGetViewModel>();
                cfg.CreateMap<ActivityReceiver.ViewModels.ExerciseManage.ExerciseManageEditPostViewModel, Exercise>()
                    .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.EditorID, opt => opt.Ignore());
                cfg.CreateMap<ActivityReceiver.ViewModels.ExerciseManage.ExerciseManageEditPostViewModel, ActivityReceiver.ViewModels.ExerciseManage.ExerciseManageEditGetViewModel>();
                // Details
                cfg.CreateMap<Exercise, ActivityReceiver.ViewModels.ExerciseManage.ExerciseManageDetailsViewModel>();
                // Delete
                cfg.CreateMap<Exercise, ActivityReceiver.ViewModels.ExerciseManage.ExerciseManageDeleteGetViewModel>();
                cfg.CreateMap<ActivityReceiver.ViewModels.ExerciseManage.ExerciseManageDeletePostViewModel, ActivityReceiver.ViewModels.ExerciseManage.ExerciseManageDeleteGetViewModel>();

                /* StudentManage */
                // Index
                cfg.CreateMap<ApplicationUser, ActivityReceiver.ViewModels.StudentManage.StudentPresenter>();
            });


        }
    }
}
