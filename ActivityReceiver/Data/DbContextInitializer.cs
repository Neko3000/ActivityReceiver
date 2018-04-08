using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Models;
using Microsoft.AspNetCore.Identity;

namespace ActivityReceiver.Data
{
    public interface IDbContextInitializer
    {
        void Initialize();
    }
    public class DbContextInitializer:IDbContextInitializer
    {
        ApplicationDbContext _appDbContext;
        ActivityReceiverDbContext _arDbContext;
        UserManager<ApplicationUser> _userManager;
        RoleManager<IdentityRole> _roleManager;

        public DbContextInitializer(ApplicationDbContext appDbContext, ActivityReceiverDbContext arDbCotnext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _appDbContext = appDbContext;
            _arDbContext = arDbCotnext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            ApplicationDbContextInitialize();
            AcitvityReceiverDbCotextInitialize();
        }

        public void ApplicationDbContextInitialize()
        {
            _appDbContext.Database.EnsureCreated();

            if(_appDbContext.Users.Any())
            {
                return;
            }
        }

        public void AcitvityReceiverDbCotextInitialize()
        {
            _arDbContext.Database.EnsureCreated();
            
            if(_arDbContext.Movements.Any())
            {
                return;
            }

            var movements = new List<Movement>()
            {
                new Movement()
                {
                    UID = 1,
                    QID = 1,
                    Time = 4000
                },
                new Movement()
                {
                    UID = 1,
                    QID = 2,
                    Time = 3000
                }
            };

            _arDbContext.Movements.AddRange(movements);
            _arDbContext.SaveChanges();
        }
    }
}
