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
using ActivityReceiver.ViewModels.AnswerRecordManage;
using ActivityReceiver.Functions;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ActivityReceiver.DataBuilders;
using ActivityReceiver.Converters;

namespace ActivityReceiver.Controllers
{
    [Authorize]
    public class AnswerRecordManageController : Controller
    {
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly AnswerRecordManageDataBuilder _dataBuilder;

        public AnswerRecordManageController(ActivityReceiverDbContext arDbContext, UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _arDbContext = arDbContext;
            _userManager = userManager;
            _roleManager = roleManager;

            _dataBuilder = new AnswerRecordManageDataBuilder(_arDbContext,_userManager,_roleManager);
        }

        // GET: AnswerRecordManage
        [HttpGet]
        public async Task<IActionResult> Index(int? assignmentRecordID)
        {    
            var vm = new AnswerRecordManageIndexViewModel
            {
                AnswerRecordPresenterCollection = await _dataBuilder.BuildAnswerRecordPresenterList(assignmentRecordID)
            };

            return View(vm);
        }

        // GET: AnswerRecordManage/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var answerRecord = await _arDbContext.AnswserRecords.SingleOrDefaultAsync(q => q.ID == id);

            if (answerRecord == null)
            {
                return NotFound();
            }

            var vm = Mapper.Map<AnswerRecord, AnswerRecordManageDetailsViewModel>(answerRecord);

            var splitConfusionElement = answerRecord.ConfusionElement.Split('#');
            var splitDivision = answerRecord.Division.Split('|');
            var sortedWordCollection = new List<string>();
            foreach(var confusionElement in splitConfusionElement)
            {
                sortedWordCollection.Add(splitDivision[Convert.ToInt32(confusionElement)]);
            }
            vm.ConfusionWordString = StringConverter.ConvertToSingleString(sortedWordCollection,",");

            var movementCollection = await _arDbContext.Movements.Where(m => m.AnswerRecordID == answerRecord.ID).ToListAsync();
            var deviceAccelerationCollection = await _arDbContext.DeviceAccelerations.Where(da => da.AnswerRecordID == answerRecord.ID).ToListAsync();

            // supervise process
            var movementSupervisor = new MovementSupervisor(movementCollection, deviceAccelerationCollection);
            var movementSupervisedCollection = movementSupervisor.Supervise();

            vm.MovementCollection = movementCollection;
            vm.DeviceAccelerationCollection = deviceAccelerationCollection;

            // Parameter Analyze
            vm.DDIntervalAVG = ParameterAnalyzer.CalculateDDIntervalAVG(movementCollection);
            vm.DDIntervalMAX = ParameterAnalyzer.CalculateDDIntervalMAX(movementCollection);
            vm.DDIntervalMIN = ParameterAnalyzer.CalculateDDIntervalMIN(movementCollection);
            vm.DDProcessAVG = ParameterAnalyzer.CalculateDDProcessAVG(movementCollection);
            vm.DDProcessMAX = ParameterAnalyzer.CalculateDDProcessMAX(movementCollection);
            vm.DDProcessMIN = ParameterAnalyzer.CalculateDDProcessMIN(movementCollection);
            vm.TotalDistance = ParameterAnalyzer.CalculateTotalDistance(movementCollection);
            vm.DDSpeedAVG = ParameterAnalyzer.CalculateDDSpeedAVG(movementCollection);
            vm.DDSpeedMAX = ParameterAnalyzer.CalculateDDSpeedMAX(movementCollection);
            vm.DDSpeedMIN = ParameterAnalyzer.CalculateDDSpeedMIN(movementCollection);
            vm.DDFirstTime = ParameterAnalyzer.CalculateDDFirstTime(movementCollection);
            vm.DDCount = ParameterAnalyzer.CalculateDDCount(movementCollection);
            vm.UTurnHorizontalCount = ParameterAnalyzer.CalculateUTurnHorizontalCount(movementCollection);
            vm.UTurnVerticalCount = ParameterAnalyzer.CalculateUTurnVerticalCount(movementCollection);

            // ParameterFixed Analyze
            vm.DDProcessAVGFixed = ParameterAnalyzerForMovementSupervised.CalculateDDProcessAVG(movementSupervisedCollection);
            vm.DDProcessMAXFixed = ParameterAnalyzerForMovementSupervised.CalculateDDProcessMAX(movementSupervisedCollection);
            vm.DDProcessMINFixed = ParameterAnalyzerForMovementSupervised.CalculateDDProcessMIN(movementSupervisedCollection);
            vm.TotalDistanceFixed = ParameterAnalyzerForMovementSupervised.CalculateTotalDistance(movementSupervisedCollection);
            vm.DDSpeedAVGFixed = ParameterAnalyzerForMovementSupervised.CalculateDDSpeedAVG(movementSupervisedCollection);
            vm.DDSpeedMAXFixed = ParameterAnalyzerForMovementSupervised.CalculateDDSpeedMAX(movementSupervisedCollection);
            vm.DDSpeedMINFixed = ParameterAnalyzerForMovementSupervised.CalculateDDSpeedMIN(movementSupervisedCollection);
            vm.UTurnHorizontalCountFixed = ParameterAnalyzerForMovementSupervised.CalculateUTurnHorizontalCount(movementSupervisedCollection);
            vm.UTurnVerticalCountFixed = ParameterAnalyzerForMovementSupervised.CalculateUTurnVerticalCount(movementSupervisedCollection);

            return View(vm);
        }



    }
}