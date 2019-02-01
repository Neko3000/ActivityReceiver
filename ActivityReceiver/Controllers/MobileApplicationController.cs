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
    public class MobileApplicationController : Controller
    {
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public MobileApplicationController(ActivityReceiverDbContext arDbContext, UserManager<ApplicationUser> userManager)
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
                return NotFound(new
                {
                    message = "user's token is invalid"
                });
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
                                       join eqc in _arDbContext.ExerciseQuestionRelationMap on q.ID equals eqc.QuestionID
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
                return BadRequest(new
                {
                    message = "request forbidened"
                });
            }

            var userID = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
            var user = await  _userManager.FindByIdAsync(userID);

            if(user == null)
            {
                return NotFound(new
                {
                    message = "user's token is invalid"
                });
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
                                     join eqc in _arDbContext.ExerciseQuestionRelationMap on q.ID equals eqc.QuestionID
                                     where eqc.ExerciseID == model.ExerciseID
                                     orderby eqc.SerialNumber ascending
                                     select q).ToList();

                    // Check if the CurrentQuestionIndex has exceeded the upperbound of the list
                    if (specificAssignment.CurrentQuestionIndex > allQuestionsInExercise.Count - 1)
                    {
                        return NotFound(new
                        {
                            message = "the exercise has already finished"
                        });
                    }

                    var question = allQuestionsInExercise[specificAssignment.CurrentQuestionIndex];

                    var vm = new GetNextQuestionGetViewModel{
                        AssignmentRecordID = specificAssignment.ID,
                        QuestionID = question.ID,

                        SentenceEN = question.SentenceEN,
                        SentenceJP = question.SentenceJP,
                        Division = question.Division,
                        StandardAnswerDivision = question.StandardAnswerDivision,

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
                                 join eqc in _arDbContext.ExerciseQuestionRelationMap on q.ID equals eqc.QuestionID
                                 where eqc.ExerciseID == model.ExerciseID
                                 orderby eqc.SerialNumber ascending
                                 select q).ToList();

                var question = allQuestionsInExercise[assignmentRecordNew.CurrentQuestionIndex];

                var vm = new GetNextQuestionGetViewModel
                {
                    AssignmentRecordID = assignmentRecordNew.ID,
                    QuestionID = question.ID,

                    SentenceEN = question.SentenceEN,
                    SentenceJP = question.SentenceJP,
                    Division = question.Division,
                    StandardAnswerDivision = question.StandardAnswerDivision,

                    CurrentNumber = assignmentRecordNew.CurrentQuestionIndex + 1
                };
                return Ok(vm);
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SubmitAnswerRecord([FromBody]SubmitAnswerRecordPostViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "request forbidened"
                });
            }

            var userID = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
            var user = await _userManager.FindByIdAsync(userID);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "user's token is invalid"
                });
            }

            var answerRecordNew = new AnswerRecord
            {
                AssignmentRecordID = model.AssignmentRecordID,

                SentenceEN = model.SentenceEN,
                SentenceJP = model.SentenceJP,
                Division = model.Division,
                StandardAnswerDivision = model.StandardAnswerDivision,
                Resolution = model.Resolution,

                AnswerDivision = model.AnswerDivision,
                IsCorrect = model.StandardAnswerDivision == model.AnswerDivision ? true:false,

                ConfusionDegree = model.ConfusionDegree,
                ConfusionElement = model.ConfusionElement,

                StartDate = model.StartDate,
                EndDate = model.EndDate
            };
            _arDbContext.AnswserRecords.Add(answerRecordNew);
            _arDbContext.SaveChanges();

            // Save movements
            var movementCollection = model.MovementCollection.OrderBy(m => m.Index).ToList();
            for(int i = 0;i < movementCollection.Count; i++)
            {
                movementCollection[i].AnswerRecordID = answerRecordNew.ID;
            }
            _arDbContext.Movements.AddRange(movementCollection);
            _arDbContext.SaveChanges();

            // Save deviceAccelerations
            var deviceAccelerationCollection = model.DeviceAccelerationCollection.OrderBy(da => da.Index).ToList();
            for (int i = 0; i < deviceAccelerationCollection.Count; i++)
            {
                deviceAccelerationCollection[i].AnswerRecordID = answerRecordNew.ID;
            }
            _arDbContext.DeviceAccelerations.AddRange(deviceAccelerationCollection);
            _arDbContext.SaveChanges();


            var specificAssignmentRecord = _arDbContext.AssignmentRecords.Where(ar => ar.ID == model.AssignmentRecordID).ToList().FirstOrDefault();
            specificAssignmentRecord.CurrentQuestionIndex = specificAssignmentRecord.CurrentQuestionIndex + 1;

            var allQuestionsInExercise = (from q in _arDbContext.Questions
                                          join eqc in _arDbContext.ExerciseQuestionRelationMap on q.ID equals eqc.QuestionID
                                          where eqc.ExerciseID == specificAssignmentRecord.ExerciseID
                                          orderby eqc.SerialNumber ascending
                                          select q).ToList();

            if (allQuestionsInExercise.Count <= specificAssignmentRecord.CurrentQuestionIndex)
            {
                specificAssignmentRecord.IsFinished = true;
                specificAssignmentRecord.EndDate = DateTime.Now;
            }
            _arDbContext.SaveChanges();

            return Ok(new
            {
                message = "submit answer successfully"
            });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAssignmentResult([FromBody]GetAssignmentResultPostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                    {
                        message = "request forbidden"
                    });
            }

            var userID = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
            var user = await _userManager.FindByIdAsync(userID);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "user's token is invalid"
                });
            }

            var specificAssignment = _arDbContext.AssignmentRecords.Where(ar => ar.UserID == user.Id && ar.ExerciseID == model.ExerciseID).ToList().FirstOrDefault();

            if (specificAssignment == null)
            {
                return NotFound(new
                {
                    message = "there is no assignment record for this exercise"
                });
            }

            if(specificAssignment.IsFinished == false)
            {
                return NotFound(new
                {
                    message = "you haven't finished this assignment yet"
                });
            }

            var answerRecords = _arDbContext.AnswserRecords.Where(q => q.AssignmentRecordID == specificAssignment.ID).ToList();
            float accuracyRate = answerRecords.Where(a => a.IsCorrect == true).Count() / (float)answerRecords.Count;

            var allQuestion = _arDbContext.Questions.ToList();

            var answerDetails = new List<AnswerDetail>();
            foreach (var answerRecord in answerRecords)
            {
                var answerDetail = new AnswerDetail
                {
                    SentenceJP = answerRecord.SentenceJP,
                    SentenceEN = answerRecord.SentenceEN,

                    AnswerSentence = QuestionHandler.ConvertDivisionToSentence(answerRecord.AnswerDivision),
                    IsCorrect = answerRecord.IsCorrect
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