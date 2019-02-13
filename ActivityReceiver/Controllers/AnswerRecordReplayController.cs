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
using ActivityReceiver.ViewModels.AnswerReplay;
using ActivityReceiver.Functions;
using Microsoft.EntityFrameworkCore;

namespace ActivityReceiver.Controllers
{
    public class AnswerRecordReplayController : Controller
    {
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public AnswerRecordReplayController(ActivityReceiverDbContext arDbContext, UserManager<ApplicationUser> userManager)
        {
            _arDbContext = arDbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetAnswerRecord(int id)
        {
            var answerRecord = _arDbContext.AnswserRecords.Where(a => a.ID == id).SingleOrDefault();

            if(answerRecord == null)
            {
                return NotFound();
            }

            var movements = _arDbContext.Movements.Where(m => m.AnswerRecordID == id).ToList();
            var deviceAccelerations = _arDbContext.DeviceAccelerations.Where(d => d.AnswerRecordID == id).ToList();

            var vm = new AnswerReplayGetAnswerViewModel
            {
                ID = answerRecord.ID,
                AssignmentRecordID = answerRecord.AssignmentRecordID,

                SentenceEN = answerRecord.SentenceEN,
                SentenceJP = answerRecord.SentenceJP,
                Division = answerRecord.Division,
                StandardAnswerDivision = answerRecord.StandardAnswerDivision,

                Resolution = answerRecord.Resolution,

                AnswerDivision = answerRecord.AnswerDivision,
                IsCorrect = answerRecord.IsCorrect,

                ConfusionDegree = answerRecord.ConfusionDegree,
                ConfusionElement = answerRecord.ConfusionElement,

                StartDate = answerRecord.StartDate,
                EndDate = answerRecord.EndDate,

                MovementCollection = movements.OrderBy(m=>m.Index).ToList(),
                DeviceAccelerationCollection = deviceAccelerations.OrderBy(da=>da.Index).ToList(),
            };

            //Request.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            //Request.HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");

            return Ok(vm);
        }

        [HttpGet]
        public async Task<IActionResult> GetDeviceAccelerationCollection(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deviceAccelerationCollection = await _arDbContext.DeviceAccelerations.Where(da => da.AnswerRecordID == id).ToListAsync();

            return Ok(deviceAccelerationCollection);
        }

        [HttpGet]
        public async Task<IActionResult> GetMovementCollection(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movementCollection = await _arDbContext.Movements.Where(da => da.AnswerRecordID == id).ToListAsync();

            return Ok(movementCollection);
        }

        [HttpGet]
        public IActionResult Replayer(int id)
        {
            var vm = new AnswerReplayReplayerViewModel {
                AnswerRecordID = id
            };
            return View(vm);
        }


        [HttpGet]
        public IActionResult Index()
        {   
            return View();
        }

    }
}