using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ActivityReceiver.ViewModels;
using ActivityReceiver.Models;
using ActivityReceiver.Data;
using Microsoft.AspNetCore.Identity;

namespace ActivityReceiver.Functions
{

    public class ApplicationUserHandler
    {
        public static async Task<IList<ApplicationUserDTO>> ConvertApplicationUsersToDTOs(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager,IList<ApplicationUser> applicationUsers)
        {
            var applicationUserDTOs = new List<ApplicationUserDTO>();
            foreach(var applicationUser in applicationUsers)
            {

                var applicationUserDTO = new ApplicationUserDTO{
                    ID = applicationUser.Id,
                    UserName = applicationUser.UserName,
                    Email = applicationUser.Email
                };

                var roleNames = await userManager.GetRolesAsync(applicationUser);

                applicationUserDTO.Roles = new List<IdentityRole>();
                foreach(var roleName in roleNames)
                {
                    var identityRole = await roleManager.FindByNameAsync(roleName);
                    applicationUserDTO.Roles.Add(identityRole);
                }

                applicationUserDTOs.Add(applicationUserDTO);
            }

            return applicationUserDTOs;
        }
        public static async Task<ApplicationUserDTO> ConvertApplicationUserToDTO(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager,ApplicationUser applicationUser)
        {
            var applicationUserDTO = new ApplicationUserDTO{
                ID = applicationUser.Id,
                UserName = applicationUser.UserName,
                Email = applicationUser.Email
            };

            var roleNames = await userManager.GetRolesAsync(applicationUser);

            applicationUserDTO.Roles = new List<IdentityRole>();
            foreach(var roleName in roleNames)
            {
                var identityRole = await roleManager.FindByNameAsync(roleName);
                applicationUserDTO.Roles.Add(identityRole);
            }
        

            return applicationUserDTO;
        }

        public static async Task<IList<string>> ConvertRoleNamesToIDs(RoleManager<IdentityRole> roleManager,IList<string> roleNames)
        {
            var roleIDs = new List<string>();
            foreach(var roleName in roleNames)
            {
                var identityRole = await roleManager.FindByNameAsync(roleName);
                roleIDs.Add(identityRole.Id);
            }
            return roleIDs;
        } 

        public static async Task<IList<string>> ConvertRoleIDsToNames(RoleManager<IdentityRole> roleManager,IList<string> roleIDs)
        {
            var roleNames = new List<string>();
            foreach(var roleID in roleIDs)
            {
                var identityRole = await roleManager.FindByIdAsync(roleID.ToString());
                roleNames.Add(identityRole.Name);
            }
            return roleNames;
        }
    }
}