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
using ActivityReceiver.ViewModels;
using ActivityReceiver.Functions;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace ActivityReceiver.Controllers
{
    [Authorize]
    public class AnswerManage : Controller
    {
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AnswerManage(ActivityReceiverDbContext arDbContext, UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _arDbContext = arDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: QuestionManage
        [HttpGet]
        public async Task<IActionResult> Index()
        {    
            var answers = await  _arDbContext.Answsers.ToListAsync();

            var vm = new AnswerManageIndexViewModel
            {
                AnswerDTOs = AnswerReplayHandler.ConvertToDTOCollection<Answer, AnswerDTO>(answers)
            };

            return View(vm);
        }


        // GET: ItemManage/Delete/5
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

            var vm = Mapper.Map<Answer, AnswerDTO>(answer);

            return View(vm);
        }



    }
}