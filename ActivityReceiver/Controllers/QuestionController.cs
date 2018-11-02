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

namespace ActivityReceiver.Controllers
{
    [Produces("application/json")]
    public class QuestionController : Controller
    {
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuestionController(ActivityReceiverDbContext arDbContext, UserManager<ApplicationUser> userManager)
        {
            _arDbContext = arDbContext;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetNextQuestion(GetNextQuestionViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userID = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
            var user = await  _userManager.FindByIdAsync(userID);

            // Check if this Exercise has AssignmentRecord for the user exists
            var isSpecificAssignmentHasRecord = _arDbContext.AssignmentRecords.Where(ar => ar.UserID == user.Id && ar.ExerciseID == model.ExerciseID).Any();

            if (isSpecificAssignmentHasRecord)
            {
                var specificAssignment = _arDbContext.AssignmentRecords.Where(ar => ar.UserID == user.Id && ar.ExerciseID == model.ExerciseID).ToList().FirstOrDefault();

                // Check if this AssignmentRecord has been finished
                if(!specificAssignment.IsFinished)
                {
                    var questions = (from q in _arDbContext.Questions
                                     join eqc in _arDbContext.ExerciseQuestionCollection on q.ID equals eqc.QuestionID
                                     where eqc.ExerciseID == model.ExerciseID
                                     orderby eqc.SerialNumber ascending
                                     select q).ToList();

                    // Check if the CurrentQuestionIndex has exceeded the upperbound of the list
                    if(specificAssignment.CurrentQuestionIndex > questions.Count - 1)
                    {
                        return NotFound();
                    }

                    return Ok(questions[specificAssignment.CurrentQuestionIndex]);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {

                // Create a new AssignmentRecord
                var assignmentNew = new AssignmentRecord
                {
                    UserID = user.Id,
                    ExerciseID = model.ExerciseID,
                    CurrentQuestionIndex = 0,
                    StartDate = DateTime.Now,
                    IsFinished = false,
                    Grade = (float)0.0,
                };
                _arDbContext.Add(assignmentNew);
                _arDbContext.SaveChanges();

                var questions = (from q in _arDbContext.Questions
                                 join eqc in _arDbContext.ExerciseQuestionCollection on q.ID equals eqc.QuestionID
                                 where eqc.ExerciseID == model.ExerciseID
                                 orderby eqc.SerialNumber ascending
                                 select q).ToList();

                return Ok(questions[assignmentNew.CurrentQuestionIndex]);
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            var questions = _arDbContext.Questions.ToList();

            return Ok(questions);
        }


    }
}