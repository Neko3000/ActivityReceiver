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
using ActivityReceiver.Functions;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ActivityReceiver.ViewModels.AssignmentRecordManage;
using ActivityReceiver.DataBuilders;


namespace ActivityReceiver.Controllers
{
    [Authorize]
    public class AssignmentRecordManageController : Controller
    {
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly AssignmentRecordManageDataBulider _dataBuilder;

        public AssignmentRecordManageController(ActivityReceiverDbContext arDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _arDbContext = arDbContext;
            _userManager = userManager;
            _roleManager = roleManager;

            _dataBuilder = new AssignmentRecordManageDataBulider(_arDbContext, _userManager, _roleManager);
        }

        // GET: AssignmentManage/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new AssignmentRecordManageIndexViewModel
            {
                AssignmentRecordPresenterCollection = await _dataBuilder.GetAssignmentRecordPresenterList()
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

            var assignmentRecord = await _arDbContext.AssignmentRecords.SingleOrDefaultAsync(ar=>ar.ID == id);

            if (assignmentRecord == null)
            {
                return NotFound();
            }

            var vm = Mapper.Map<AssignmentRecord, AssignmentRecordManageDetailsViewModel>(assignmentRecord);

            vm.Username = (await _userManager.FindByIdAsync(assignmentRecord.UserID)).UserName;
            vm.ExerciseName = (await _arDbContext.Exercises.FindAsync(assignmentRecord.ExerciseID)).Name;

            var sortedQuestions = (from q in _arDbContext.Questions
                                   join eqc in _arDbContext.ExerciseQuestionCollection on q.ID equals eqc.QuestionID
                                   where eqc.ExerciseID == assignmentRecord.ExerciseID
                                   orderby eqc.SerialNumber ascending
                                   select q).ToList();
            vm.CurrentProgress = String.Format("{0}/{1}", assignmentRecord.CurrentQuestionIndex, sortedQuestions.Count);

            var answers = await _arDbContext.Answsers.Where(a => a.AssignmentRecordID == assignmentRecord.ID).ToListAsync();
            var answerPresenterCollection = AutoMapperHandler.ListMapper<Answer, AnswerPresenter>(answers);
            vm.AnswerPresenterCollection = answerPresenterCollection;

            return View(vm);
        }

    }
}