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
                                   join eqc in _arDbContext.ExerciseQuestionRelationMap on q.ID equals eqc.QuestionID
                                   where eqc.ExerciseID == assignmentRecord.ExerciseID
                                   orderby eqc.SerialNumber ascending
                                   select q).ToList();
            vm.CurrentProgress = String.Format("{0}/{1}", assignmentRecord.CurrentQuestionIndex, sortedQuestions.Count);

            // Get AnswerRecordPresenterCollection
            var answerRecordList = await _arDbContext.AnswserRecords.Where(ar=>ar.AssignmentRecordID == assignmentRecord.ID).ToListAsync();

            var answerRecordPresenterCollection = new List<AnswerRecordPresenter>();
            foreach (var answerRecord in answerRecordList)
            {
                var answerRecordPresenter = Mapper.Map<AnswerRecord, AnswerRecordPresenter>(answerRecord);

                var movementCollection = await _arDbContext.Movements.Where(m => m.AnswerRecordID == answerRecord.ID).ToListAsync();

                // Parameter Analyze
                answerRecordPresenter.DDIntervalAVG = ParameterAnalyzer.CalculateDDIntervalAVG(movementCollection);
                answerRecordPresenter.DDIntervalMAX = ParameterAnalyzer.CalculateDDIntervalMAX(movementCollection);
                answerRecordPresenter.DDIntervalMIN = ParameterAnalyzer.CalculateDDIntervalMIN(movementCollection);
                answerRecordPresenter.DDProcessAVG = ParameterAnalyzer.CalculateDDProcessAVG(movementCollection);
                answerRecordPresenter.DDProcessMAX = ParameterAnalyzer.CalculateDDProcessMAX(movementCollection);
                answerRecordPresenter.DDProcessMIN = ParameterAnalyzer.CalculateDDProcessMIN(movementCollection);
                answerRecordPresenter.TotalDistance = ParameterAnalyzer.CalculateTotalDistance(movementCollection);
                answerRecordPresenter.DDSpeedAVG = ParameterAnalyzer.CalculateDDSpeedAVG(movementCollection);
                answerRecordPresenter.DDSpeedMAX = ParameterAnalyzer.CalculateDDSpeedMAX(movementCollection);
                answerRecordPresenter.DDSpeedMIN = ParameterAnalyzer.CalculateDDSpeedMIN(movementCollection);
                answerRecordPresenter.DDFirstTime = ParameterAnalyzer.CalculateDDFirstTime(movementCollection);
                answerRecordPresenter.DDCount = ParameterAnalyzer.CalculateDDCount(movementCollection);
                answerRecordPresenter.UTurnHorizontalCount = ParameterAnalyzer.CalculateUTurnHorizontalCount(movementCollection);
                answerRecordPresenter.UTurnVerticalCount = ParameterAnalyzer.CalculateUTurnVerticalCount(movementCollection);

                answerRecordPresenterCollection.Add(answerRecordPresenter);
            }

            vm.AnswerRecordPresenterCollection = answerRecordPresenterCollection;

            return View(vm);
        }

    }
}