﻿using BusinessLogicsLayer.Appt;
using BusinessLogicsLayer.BasicDet;
using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.BdeCat;
using BusinessLogicsLayer.BdeCate;
using BusinessLogicsLayer.Corps;
using BusinessLogicsLayer.Div;
using BusinessLogicsLayer.Formation;
using BusinessLogicsLayer.Master;
using BusinessLogicsLayer.Unit;
using BusinessLogicsLayer.User;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Identitytable;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SqlServer.Management.Smo;

namespace BusinessLogicsLayer
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IUserBL, UserBL>();
            services.AddTransient<IUserDB, UserDB>();

            services.AddTransient<IComd, Comd>();
            services.AddTransient<IComdDB, ComdDB>();

            services.AddTransient<ICorpsBL, CorpsBL>();
            services.AddTransient<ICorpsDB, CorpsDB>();

            services.AddTransient<IBdeBL, BdeBL>();
            services.AddTransient<IBdeDB, BdeDB>();

            services.AddTransient<IDivBL, DivBL>();
            services.AddTransient<IDivDB, DivDB>();

            services.AddTransient<IUnitBL, UnitBL>();
            services.AddTransient<IUnitDB, UnitDB>();

            services.AddTransient<IMapUnitBL, MapUnitBL>();
            services.AddTransient<IMapUnitDB, MapUnitDB>();

            services.AddTransient<IFormationBL, FormationBL>();
            services.AddTransient<IFormationDB, FormationDB>();

            services.AddTransient<IApptBL, ApptBL>();
            services.AddTransient<IApptDB, ApptDB>();

            services.AddTransient<IArmedBL, ArmedBL>();
            services.AddTransient<IArmedDB, ArmedDB>();

            services.AddTransient<IBasicDetailBL, BasicDetailBL>();
            services.AddTransient<IBasicDetailDB, BasicDetailDB>();


            //services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer("Server=SDG20\\SQLEXPRESS; Database=AFSAC;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True"));
            //services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer("Server=.\\sqlexpress; Database=AFSAC;User Id=sa; Password=Admin@2018;MultipleActiveResultSets=True;TrustServerCertificate=True"));
            // services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer("Server=192.168.10.63; database=AFSAC_New; User Id=sa; Password=Admin@2018;Connect Timeout=30;TrustServerCertificate=True; MultipleActiveResultSets=true"));
            // services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            //{
            //    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            //    options.Lockout.MaxFailedAccessAttempts = 3;
            //    options.User.RequireUniqueEmail = false;
            //    options.SignIn.RequireConfirmedAccount = true;
            //    options.SignIn.RequireConfirmedEmail = false;
            //    options.Lockout.AllowedForNewUsers = true;

            //})
            // .AddDefaultUI()
            // .AddEntityFrameworkStores<ApplicationDbContext>()
            // .AddDefaultTokenProviders();



            // return services;
        }
        
    }
}