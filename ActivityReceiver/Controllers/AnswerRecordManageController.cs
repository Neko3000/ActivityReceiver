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
using ActivityReceiver.DataBuilders;

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

        // GET: QuestionManage
        [HttpGet]
        public async Task<IActionResult> Index()
        {    

            var vm = new AnswerRecordManageIndexViewModel
            {
                AnswerRecordPresenterCollection = await _dataBuilder.BuildAnswerRecordPresenterList()
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> GetDeviceAccelerationCollection(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var deviceAccelerationCollection = await _arDbContext.DeviceAccelerations.Where(da => da.AnswerID == id).ToListAsync();

            return Ok(deviceAccelerationCollection);
        }

        // GET: ItemManage/Details/5
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

            var movementCollection = await _arDbContext.Movements.Where(m => m.AnswerID == answerRecord.ID).ToListAsync();
            var deviceAccelerationCollection = await _arDbContext.DeviceAccelerations.Where(da => da.AnswerID == answerRecord.ID).ToListAsync();

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

            return View(vm);
        }



    }
}