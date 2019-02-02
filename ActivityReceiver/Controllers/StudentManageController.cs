using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ActivityReceiver.Data;
using ActivityReceiver.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using ActivityReceiver.ViewModels.StudentManage;
using ActivityReceiver.Functions;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ActivityReceiver.DataBuilders;

namespace ActivityReceiver.Controllers
{
    [Authorize]
    public class StudentManageController : Controller
    {
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly StudentManageDataBuilder _dataBuilder;

        public StudentManageController(ActivityReceiverDbContext arDbContext, UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _arDbContext = arDbContext;
            _userManager = userManager;
            _roleManager = roleManager;

            _dataBuilder = new StudentManageDataBuilder(_arDbContext, _userManager, _roleManager);
        }

        // GET: StudentManage
        [HttpGet]
        public async Task<IActionResult> Index()
        {    
            var vm = new StudentManageIndexViewModel
            {
                StudentPresenterCollection = await _dataBuilder.BuildStudentPresenterList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _userManager.FindByIdAsync(id);

            if(applicationUser == null)
            {
                return NotFound();
            }

            await  _userManager.RemovePasswordAsync(applicationUser);

            await _userManager.AddPasswordAsync(applicationUser, "000000");

            return RedirectToAction("Index");
        }
    }
}