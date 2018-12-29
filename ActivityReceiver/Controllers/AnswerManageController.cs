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
using ActivityReceiver.ViewModels.AnswerManage;
using ActivityReceiver.Functions;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ActivityReceiver.DataBuilder;

namespace ActivityReceiver.Controllers
{
    [Authorize]
    public class AnswerManageController : Controller
    {
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly AnswerManageDataBuilder _dataBuilder;

        public AnswerManageController(ActivityReceiverDbContext arDbContext, UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _arDbContext = arDbContext;
            _userManager = userManager;
            _roleManager = roleManager;

            _dataBuilder = new AnswerManageDataBuilder(_arDbContext, _userManager, _roleManager);
        }

        // GET: QuestionManage
        [HttpGet]
        public async Task<IActionResult> Index()
        {    
            var answers = await  _arDbContext.Answsers.ToListAsync();

            var vm = new AnswerManageIndexViewModel
            {
                AnswerPresenterCollection = await _dataBuilder.BuildAnswerPresenterList()
            };

            return View(vm);
        }


        // GET: ItemManage/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var answer = await _arDbContext.Answsers.SingleOrDefaultAsync(q => q.ID == id);

            if (answer == null)
            {
                return NotFound();
            }

            var vm = _dataBuilder.BuildAnswerManageDetailsViewModel(id);

            return View(vm);
        }



    }
}