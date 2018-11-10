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
        public async Task<IActionResult> GetExerciseList()
        {
            var userID = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
            var user = await _userManager.FindByIdAsync(userID);

            if (user == null)
            {
                return BadRequest();
            }

            var allExercises = _arDbContext.Exercises.ToList();
            var assignmentForUser = _arDbContext.AssignmentRecords.Where(ar=>ar.UserID == userID).ToList();

            var exerciseDetails = new List<ExerciseDetail>();

            foreach(var exercise in allExercises)
            {
                var exerciseDetail = new ExerciseDetail
                {
                    ID = exercise.ID,
                    Name = exercise.Name,
                    Description = exercise.Description
                };

                var specificAssignmentRecord = assignmentForUser.Where(afu => afu.ExerciseID == exercise.ID).FirstOrDefault();
                exerciseDetail.CurrentNumber = specificAssignmentRecord == null ? 0 : specificAssignmentRecord.CurrentQuestionIndex;
                exerciseDetail.IsFinished = specificAssignmentRecord == null ? false : specificAssignmentRecord.IsFinished;

                var sortedQuestions = (from q in _arDbContext.Questions
                                       join eqc in _arDbContext.ExerciseQuestionCollection on q.ID equals eqc.QuestionID
                                       where eqc.ExerciseID == exercise.ID
                                       orderby eqc.SerialNumber ascending
                                       select q).ToList();

                exerciseDetail.TotalNumber = sortedQuestions.Count;

                exerciseDetails.Add(exerciseDetail);
            }

            var vm = new GetExerciseListGetViewModel{
                ExerciseDetails = exerciseDetails
            };

            return Ok(vm);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetNextQuestion([FromBody]GetNextQuestionPostViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userID = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
            var user = await  _userManager.FindByIdAsync(userID);

            if(user == null)
            {
                return BadRequest();
            }

            // Check if this Exercise has AssignmentRecord for the user exists
            var isSpecificAssignmentHasRecord = _arDbContext.AssignmentRecords.Where(ar => ar.UserID == user.Id && ar.ExerciseID == model.ExerciseID).Any();

            if (isSpecificAssignmentHasRecord)
            {
                var specificAssignment = _arDbContext.AssignmentRecords.Where(ar => ar.UserID == user.Id && ar.ExerciseID == model.ExerciseID).ToList().FirstOrDefault();

                // Check if this AssignmentRecord has been finished
                if(!specificAssignment.IsFinished)
                {
                    var allQuestionsInExercise = (from q in _arDbContext.Questions
                                     join eqc in _arDbContext.ExerciseQuestionCollection on q.ID equals eqc.QuestionID
                                     where eqc.ExerciseID == model.ExerciseID
                                     orderby eqc.SerialNumber ascending
                                     select q).ToList();

                    // Check if the CurrentQuestionIndex has exceeded the upperbound of the list
                    if (specificAssignment.CurrentQuestionIndex > allQuestionsInExercise.Count - 1)
                    {
                        return NotFound();
                    }

                    var question = allQuestionsInExercise[specificAssignment.CurrentQuestionIndex];

                    var vm = new GetNextQuestionGetViewModel{
                        AssignmentRecordID = specificAssignment.ID,
                        QuestionID = question.ID,

                        SentenceJP = question.SentenceJP,
                        Division = question.Division,

                        CurrentNumber = specificAssignment.CurrentQuestionIndex + 1
                    };
                    return Ok(vm);
                }
                else
                {
                    // When the user finishes the assignment return No-Content(204) to go to the next scrren
                    return NoContent();
                }
            }
            else
            {
                // Create a new AssignmentRecord
                var assignmentRecordNew = new AssignmentRecord
                {
                    UserID = user.Id,
                    ExerciseID = model.ExerciseID,

                    CurrentQuestionIndex = 0,
                    StartDate = DateTime.Now,

                    IsFinished = false,
                    Grade = (float)0.0,
                };
                _arDbContext.Add(assignmentRecordNew);
                _arDbContext.SaveChanges();

                var allQuestionsInExercise = (from q in _arDbContext.Questions
                                 join eqc in _arDbContext.ExerciseQuestionCollection on q.ID equals eqc.QuestionID
                                 where eqc.ExerciseID == model.ExerciseID
                                 orderby eqc.SerialNumber ascending
                                 select q).ToList();

                var question = allQuestionsInExercise[assignmentRecordNew.CurrentQuestionIndex];

                var vm = new GetNextQuestionGetViewModel
                {
                    AssignmentRecordID = assignmentRecordNew.ID,
                    QuestionID = question.ID,

                    SentenceJP = question.SentenceJP,
                    Division = question.Division,

                    CurrentNumber = assignmentRecordNew.CurrentQuestionIndex + 1
                };
                return Ok(vm);
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SubmitQuestionAnswer([FromBody]SubmitQuestionAnswerPostViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userID = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
            var user = await _userManager.FindByIdAsync(userID);

            if (user == null)
            {
                return BadRequest();
            }

            var specificQuestion = _arDbContext.Questions.Where(q => q.ID == model.QuestionID).ToList().SingleOrDefault();

            var answerNew = new Answer
            {
                QuestionID = model.QuestionID,
                AssignmentRecordID = model.AssignmentRecordID,

                Content = model.Answer,
                IsCorrect = QuestionHandler.ConvertSentenceToDivision(specificQuestion.SentenceEN) == model.Answer ? true:false,

                HesitationDegree = 0,

                StartDate = model.StartDate,
                EndDate = model.EndDate
            };
            _arDbContext.Answsers.Add(answerNew);
            _arDbContext.SaveChanges();

            foreach (var movementDTO in model.MovementDTOs)
            {
                var movementNew = new Movement
                {
                    AnswerID = answerNew.ID,
                    Time = movementDTO.Time,

                    State = movementDTO.State,
                    Index = movementDTO.Index,

                    XPosition = movementDTO.XPosition,
                    YPosition = movementDTO.YPosition
                };

                _arDbContext.Movements.Add(movementNew);
                _arDbContext.SaveChanges();
            }

            var specificAssignmentRecord = _arDbContext.AssignmentRecords.Where(ar => ar.ID == model.AssignmentRecordID).ToList().FirstOrDefault();
            specificAssignmentRecord.CurrentQuestionIndex = specificAssignmentRecord.CurrentQuestionIndex + 1;

            var allQuestionsInExercise = (from q in _arDbContext.Questions
                                          join eqc in _arDbContext.ExerciseQuestionCollection on q.ID equals eqc.QuestionID
                                          where eqc.ExerciseID == specificAssignmentRecord.ExerciseID
                                          orderby eqc.SerialNumber ascending
                                          select q).ToList();

            if (allQuestionsInExercise.Count <= specificAssignmentRecord.CurrentQuestionIndex)
            {
                specificAssignmentRecord.IsFinished = true;
                specificAssignmentRecord.EndDate = DateTime.Now;
            }
            _arDbContext.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAssignmentResult([FromBody]GetAssignmentResultPostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userID = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
            var user = await _userManager.FindByIdAsync(userID);

            if (user == null)
            {
                return BadRequest();
            }

            var specificAssignment = _arDbContext.AssignmentRecords.Where(ar => ar.UserID == user.Id && ar.ExerciseID == model.ExerciseID).ToList().FirstOrDefault();

            if (specificAssignment == null)
            {
                return NotFound();
            }

            if(specificAssignment.IsFinished == false)
            {
                return NotFound();
            }

            var answers = _arDbContext.Answsers.Where(q => q.AssignmentRecordID == specificAssignment.ID).ToList();
            float accuracyRate = answers.Where(a => a.IsCorrect == true).Count() / answers.Count;

            var allQuestion = _arDbContext.Questions.ToList();
            var answerDetails = new List<AnswerDetail>();

            foreach (var answer in answers)
            {
                var specificQuestion = allQuestion.Where(q => q.ID == answer.QuestionID).SingleOrDefault();

                var answerDetail = new AnswerDetail
                {
                    SentenceJP = specificQuestion.SentenceJP,
                    SentenceEN = specificQuestion.SentenceEN,

                    AnswerSentence = QuestionHandler.ConvertDivisionToSentence(answer.Content),
                    IsCorrect = answer.IsCorrect
                };
                answerDetails.Add(answerDetail);
            }

            var vm = new GetAssignmentResultGetViewModel
            {
                AccuracyRate = accuracyRate,
                AnswerDetails = answerDetails
            };

            return Ok(vm);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var questions = _arDbContext.Questions.ToList();

            return Ok(questions);
        }


    }
}