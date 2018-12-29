﻿using System;
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

namespace ActivityReceiver.Controllers
{
    public class AnswerReplayController : Controller
    {
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public AnswerReplayController(ActivityReceiverDbContext arDbContext, UserManager<ApplicationUser> userManager)
        {
            _arDbContext = arDbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetAnswer(int id)
        {
            var answer = _arDbContext.Answsers.Where(a => a.ID == id).SingleOrDefault();

            if(answer == null)
            {
                return NotFound();
            }

            var movements = _arDbContext.Movements.Where(m => m.AnswerID == id).ToList();
            var deviceAccelerations = _arDbContext.DeviceAccelerations.Where(d => d.AnswerID == id).ToList();

            var vm = new AnswerReplayGetAnswerViewModel
            {
                ID = answer.ID,
                AssignmentRecordID = answer.AssignmentRecordID,

                SentenceEN = answer.SentenceEN,
                SentenceJP = answer.SentenceJP,
                Division = answer.Division,
                AnswerDivision = answer.AnswerDivision,

                Content = answer.Content,
                IsCorrect = answer.IsCorrect,

                HesitationDegree = answer.HesitationDegree,

                StartDate = answer.StartDate,
                EndDate = answer.EndDate,

                MovementCollection = movements,
                DeviceAccelerationCollection = deviceAccelerations,
            };

            //Request.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            //Request.HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");

            return Ok(vm);
        }

        [HttpGet]
        public IActionResult Replayer(int id)
        {
            var vm = new AnswerReplayReplayerViewModel {
                AnswerID = id
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