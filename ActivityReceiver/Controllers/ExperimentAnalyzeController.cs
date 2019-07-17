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
using ActivityReceiver.ViewModels.ExperimentAnalyze;
using ActivityReceiver.Functions;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ActivityReceiver.DataBuilders;
using ActivityReceiver.Converters;
using ActivityReceiver.DataTransferObjects;
using System.IO;
using CsvHelper;

namespace ActivityReceiver.Controllers
{
    //[Authorize]
    public class ExperimentAnalyzeController : Controller
    {
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ExperimentAnalyzeController(ActivityReceiverDbContext arDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _arDbContext = arDbContext;
            _userManager = userManager;
            _roleManager = roleManager;

        }

        // GET: AnswerRecordManage
        [HttpGet]
        public async Task<IActionResult> Index(int? assignmentRecordID)
        {

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AverageForceForEachUser(int? exerciseID)
        {
            if (exerciseID == null)
            {
                return NotFound();
            }

            var exercise = await _arDbContext.Exercises.SingleOrDefaultAsync(q => q.ID == exerciseID);

            if (exercise == null)
            {
                return NotFound();
            }

            var assignmentRecordCollection = await _arDbContext.AssignmentRecords.Where(ar => ar.ExerciseID == exercise.ID).ToListAsync();

            var vm = new ForceAverageViewModel
            {
                ForceAverageForUserCollection = new List<ForceAverageForUser>()
            };

            for (int i = 0; i < assignmentRecordCollection.Count; i++)
            {
                var assignmentRecord = assignmentRecordCollection[i];
                var answerRecordCollection = await _arDbContext.AnswserRecords.Where(ar => ar.AssignmentRecordID == assignmentRecord.ID).ToListAsync();

                float forceTotalForSingleAssignmentRecord = 0;
                int forceCountForSingleAssignmentRecord = 0;
                for (int j = 0; j < answerRecordCollection.Count; j++)
                {
                    var answerRecord = answerRecordCollection[j];
                    var movementCollection = await _arDbContext.Movements.Where(m => m.AnswerRecordID == answerRecord.ID).ToListAsync();
                    var deviceAccelerationCollection = await _arDbContext.DeviceAccelerations.Where(da => da.AnswerRecordID == answerRecord.ID).ToListAsync();

                    for (int k = 0; k < movementCollection.Count; k++)
                    {
                        forceTotalForSingleAssignmentRecord += movementCollection[k].Force;
                        forceCountForSingleAssignmentRecord++;
                    }
                }

                vm.ForceAverageForUserCollection.Add(new ForceAverageForUser
                {
                    Force = forceTotalForSingleAssignmentRecord / forceCountForSingleAssignmentRecord,
                    Username = (await _userManager.FindByIdAsync(assignmentRecord.UserID)).UserName
                });
            }

            float averageForceForAll = 0;
            for (int i = 0; i < vm.ForceAverageForUserCollection.Count; i++)
            {
                averageForceForAll += vm.ForceAverageForUserCollection[i].Force;
            }

            vm.ForceAverageForUserCollection.Add(new ForceAverageForUser
            {
                Force = averageForceForAll / vm.ForceAverageForUserCollection.Count,
                Username = "For ALL"
            });
            return View(vm);
        }


        // GET: ExperimentAnalyze/AbnormalTrajectory/5
        [HttpGet]
        public async Task<IActionResult> AbnormalTrajectory(int? exerciseID)
        {
            if (exerciseID == null)
            {
                return NotFound();
            }

            var exercise = await _arDbContext.Exercises.SingleOrDefaultAsync(q => q.ID == exerciseID);

            if (exercise == null)
            {
                return NotFound();
            }

            var assignmentRecordCollection = await _arDbContext.AssignmentRecords.Where(ar => ar.ExerciseID == exercise.ID).ToListAsync();

            var vm = new AbnormalTrajectoryViewModel
            {
                AbnormalTrajectoryEvaluationCollection = new List<AbnormalTrajectoryEvaluation>()
            };

            // ThACC, ThFOC



            float thACC = 0.014f;
            float stepThACC = 0.001f;
            while (thACC <= 0.016f)
            {
                float thFOC = 0.015f;
                float stepThFOC = 0.001f;
                while (thFOC <= 0.025f)
                {
                    float precisionAverageForAllAssignmentRecord = 0;
                    float recallAverageForAllAssignmentRecord = 0;
                    float fMeasureAverageForAllAssignmentRecord = 0;
                    for (int i = 0; i < assignmentRecordCollection.Count; i++)
                    {
                        var assignmentRecord = assignmentRecordCollection[i];
                        var answerRecordCollection = await _arDbContext.AnswserRecords.Where(ar => ar.AssignmentRecordID == assignmentRecord.ID).ToListAsync();

                        float precisionAverageForSingleAssignmentRecord = 0;
                        float recallAverageForSingleAssignmentRecord = 0;
                        float fMeasureAverageForSingleAssignmentRecord = 0;
                        for (int j = 0; j < answerRecordCollection.Count; j++)
                        {
                            var answerRecord = answerRecordCollection[j];
                            var movementCollection = await _arDbContext.Movements.Where(m => m.AnswerRecordID == answerRecord.ID).ToListAsync();
                            var deviceAccelerationCollection = await _arDbContext.DeviceAccelerations.Where(da => da.AnswerRecordID == answerRecord.ID).ToListAsync();

                            var movementSupervisor = new MovementSupervisor(movementCollection, deviceAccelerationCollection, thACC, thFOC);
                            var movementSupervised = movementSupervisor.Supervise();

                            var expermentStatistician = new ExperimentStatistician();
                            var precision = expermentStatistician.CalculatePrecision(movementSupervised);
                            var recall = expermentStatistician.CalculateRecall(movementSupervised);
                            var fMeasure = expermentStatistician.CalculateFMeasure(movementSupervised);

                            precisionAverageForSingleAssignmentRecord += precision;
                            recallAverageForSingleAssignmentRecord += recall;
                            fMeasureAverageForSingleAssignmentRecord += fMeasure;
                        }
                        precisionAverageForSingleAssignmentRecord = precisionAverageForSingleAssignmentRecord / answerRecordCollection.Count;
                        recallAverageForSingleAssignmentRecord = recallAverageForSingleAssignmentRecord / answerRecordCollection.Count;
                        fMeasureAverageForSingleAssignmentRecord = fMeasureAverageForSingleAssignmentRecord / answerRecordCollection.Count;

                        precisionAverageForAllAssignmentRecord += precisionAverageForSingleAssignmentRecord;
                        recallAverageForAllAssignmentRecord += recallAverageForSingleAssignmentRecord;
                        fMeasureAverageForAllAssignmentRecord += fMeasureAverageForSingleAssignmentRecord;
                    }
                    precisionAverageForAllAssignmentRecord = precisionAverageForAllAssignmentRecord / assignmentRecordCollection.Count;
                    recallAverageForAllAssignmentRecord = recallAverageForAllAssignmentRecord / assignmentRecordCollection.Count;
                    fMeasureAverageForAllAssignmentRecord = fMeasureAverageForAllAssignmentRecord / assignmentRecordCollection.Count;

                    vm.AbnormalTrajectoryEvaluationCollection.Add(new AbnormalTrajectoryEvaluation
                    {
                        ThACC = thACC,
                        ThFOC = thFOC,

                        Precision = precisionAverageForAllAssignmentRecord,
                        Recall = recallAverageForAllAssignmentRecord,
                        FMeasure = fMeasureAverageForAllAssignmentRecord,
                    });

                    thFOC += stepThFOC;
                }

                thACC += stepThACC;
            }

            return View(vm);
        }

        // GET: ExperimentAnalyze/AbnormalTrajectory/5
        [HttpGet]
        public async Task<IActionResult> AbnormalTrajectoryForEachUser(int? exerciseID)
        {
            if (exerciseID == null)
            {
                return NotFound();
            }

            var exercise = await _arDbContext.Exercises.SingleOrDefaultAsync(q => q.ID == exerciseID);

            if (exercise == null)
            {
                return NotFound();
            }

            var assignmentRecordCollection = await _arDbContext.AssignmentRecords.Where(ar => ar.ExerciseID == exercise.ID).ToListAsync();

            var vm = new AbnormalTrajectoryViewModel
            {
                AbnormalTrajectoryEvaluationCollection = new List<AbnormalTrajectoryEvaluation>()
            };

            // ThACC, ThFOC  

            float thACC = 0.00f;
            float stepThACC = 0.001f;
            while (thACC <= 0.0302f)
            {
                float thFOC = 0.00f;
                float stepThFOC = 0.001f;
                while (thFOC <= 0.0302f)
                {
                    for (int i = 0; i < assignmentRecordCollection.Count; i++)
                    {
                        var assignmentRecord = assignmentRecordCollection[i];
                        var answerRecordCollection = await _arDbContext.AnswserRecords.Where(ar => ar.AssignmentRecordID == assignmentRecord.ID).ToListAsync();

                        float precisionAverageForSingleAssignmentRecord = 0;
                        float recallAverageForSingleAssignmentRecord = 0;
                        float fMeasureAverageForSingleAssignmentRecord = 0;
                        for (int j = 0; j < answerRecordCollection.Count; j++)
                        {
                            var answerRecord = answerRecordCollection[j];
                            var movementCollection = await _arDbContext.Movements.Where(m => m.AnswerRecordID == answerRecord.ID).ToListAsync();
                            var deviceAccelerationCollection = await _arDbContext.DeviceAccelerations.Where(da => da.AnswerRecordID == answerRecord.ID).ToListAsync();

                            var movementSupervisor = new MovementSupervisor(movementCollection, deviceAccelerationCollection, thACC, thFOC);
                            var movementSupervised = movementSupervisor.Supervise();

                            var expermentStatistician = new ExperimentStatistician();
                            var precision = expermentStatistician.CalculatePrecision(movementSupervised);
                            var recall = expermentStatistician.CalculateRecall(movementSupervised);
                            var fMeasure = expermentStatistician.CalculateFMeasure(movementSupervised);

                            precisionAverageForSingleAssignmentRecord += precision;
                            recallAverageForSingleAssignmentRecord += recall;
                            fMeasureAverageForSingleAssignmentRecord += fMeasure;
                        }
                        precisionAverageForSingleAssignmentRecord = precisionAverageForSingleAssignmentRecord / answerRecordCollection.Count;
                        recallAverageForSingleAssignmentRecord = recallAverageForSingleAssignmentRecord / answerRecordCollection.Count;
                        fMeasureAverageForSingleAssignmentRecord = fMeasureAverageForSingleAssignmentRecord / answerRecordCollection.Count;

                        bool isExisted = false;
                        string username = (await _userManager.FindByIdAsync(assignmentRecord.UserID)).UserName;
                        var abnormalTrajectoryEvaluationForSpecificUser = vm.AbnormalTrajectoryEvaluationCollection.FirstOrDefault(atec => atec.Username == username);

                        if (abnormalTrajectoryEvaluationForSpecificUser != null)
                        {
                            if (abnormalTrajectoryEvaluationForSpecificUser.FMeasure < fMeasureAverageForSingleAssignmentRecord)
                            {
                                vm.AbnormalTrajectoryEvaluationCollection.Remove(abnormalTrajectoryEvaluationForSpecificUser);

                                vm.AbnormalTrajectoryEvaluationCollection.Add(new AbnormalTrajectoryEvaluation
                                {
                                    ThACC = thACC,
                                    ThFOC = thFOC,

                                    Precision = precisionAverageForSingleAssignmentRecord,
                                    Recall = recallAverageForSingleAssignmentRecord,
                                    FMeasure = fMeasureAverageForSingleAssignmentRecord,

                                    Username = (await _userManager.FindByIdAsync(assignmentRecord.UserID)).UserName
                                });
                            }
                        }
                        else
                        {
                            vm.AbnormalTrajectoryEvaluationCollection.Add(new AbnormalTrajectoryEvaluation
                            {
                                ThACC = thACC,
                                ThFOC = thFOC,

                                Precision = precisionAverageForSingleAssignmentRecord,
                                Recall = recallAverageForSingleAssignmentRecord,
                                FMeasure = fMeasureAverageForSingleAssignmentRecord,

                                Username = (await _userManager.FindByIdAsync(assignmentRecord.UserID)).UserName
                            });
                        }

                        //vm.AbnormalTrajectoryEvaluationCollection.Add(new AbnormalTrajectoryEvaluation
                        //{
                        //    ThACC = thACC,
                        //    ThFOC = thFOC,

                        //    Precision = precisionAverageForSingleAssignmentRecord,
                        //    Recall = recallAverageForSingleAssignmentRecord,
                        //    FMeasure = fMeasureAverageForSingleAssignmentRecord,

                        //    Username = (await _userManager.FindByIdAsync(assignmentRecord.UserID)).UserName
                        //});
                    }

                    thFOC += stepThFOC;
                }

                thACC += stepThACC;
            }

            vm.AbnormalTrajectoryEvaluationCollection = vm.AbnormalTrajectoryEvaluationCollection.OrderBy(atec => atec.Username).ThenByDescending(atec => atec.FMeasure).ToList();
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadParameterCSV(int? exerciseID)
        {
            if (exerciseID == null)
            {
                return NotFound();
            }

            var exercise = await _arDbContext.Exercises.SingleOrDefaultAsync(q => q.ID == exerciseID);

            if (exercise == null)
            {
                return NotFound();
            }

            var assignmentRecordCollection = await _arDbContext.AssignmentRecords.Where(ar => ar.ExerciseID == exercise.ID && ar.IsFinished == true).ToListAsync();

            List<ParametersForMachineLearning> parametersForMachineLearningCollection = new List<ParametersForMachineLearning>();

            for (int i = 0; i < assignmentRecordCollection.Count; i++)
            {
                var assignmentRecord = assignmentRecordCollection[i];

                var answerRecordCollection = _arDbContext.AnswserRecords.Where(ar => ar.AssignmentRecordID == assignmentRecord.ID).ToList();

                for (int j = 0; j < answerRecordCollection.Count; j++)
                {
                    var answerRecord = answerRecordCollection[j];

                    var movementCollection = await _arDbContext.Movements.Where(m => m.AnswerRecordID == answerRecord.ID).ToListAsync();

                    var parametersForMachineLearning = new ParametersForMachineLearning();
                    parametersForMachineLearning.User = assignmentRecord.UserID;
                    parametersForMachineLearning.Question = j;
                    parametersForMachineLearning.Understand = answerRecord.ConfusionDegree;
                    parametersForMachineLearning.Date = answerRecord.EndDate;
                    parametersForMachineLearning.Check = 0;
                    parametersForMachineLearning.Time = (int)(answerRecord.EndDate - answerRecord.StartDate).TotalMilliseconds;
                    parametersForMachineLearning.Distance = ParameterAnalyzer.CalculateTotalDistance(movementCollection);
                    parametersForMachineLearning.AverageSpeed = ParameterAnalyzer.CalculateDDSpeedAVG(movementCollection);
                    parametersForMachineLearning.MaxSpeed = ParameterAnalyzer.CalculateDDSpeedMAX(movementCollection);
                    parametersForMachineLearning.ThinkingTime = ParameterAnalyzer.CalculateDDFirstTime(movementCollection);
                    parametersForMachineLearning.AnsweringTime = ParameterAnalyzer.CalculateDDFromFirstTime(movementCollection);
                    parametersForMachineLearning.TotalStopTime = 0;
                    parametersForMachineLearning.MaxStopTime = 0;
                    parametersForMachineLearning.TotalDDIntervalTime = ParameterAnalyzer.CalculateDDIntervalTotal(movementCollection);
                    parametersForMachineLearning.MaxDDIntervalTime = ParameterAnalyzer.CalculateDDIntervalMAX(movementCollection);
                    parametersForMachineLearning.MaxDDTime = ParameterAnalyzer.CalculateDDProcessMAX(movementCollection);
                    parametersForMachineLearning.MinDDTime = ParameterAnalyzer.CalculateDDProcessMIN(movementCollection);
                    parametersForMachineLearning.DDCount = ParameterAnalyzer.CalculateDDCount(movementCollection);
                    parametersForMachineLearning.GroupingDDCount = ParameterAnalyzer.CalculateGroupingDDCount(movementCollection);
                    parametersForMachineLearning.XUTurnCount = ParameterAnalyzer.CalculateUTurnHorizontalCount(movementCollection);
                    parametersForMachineLearning.YUTurnCount = ParameterAnalyzer.CalculateUTurnVerticalCount(movementCollection);

                    parametersForMachineLearningCollection.Add(parametersForMachineLearning);
                }
            }

            var memory = new MemoryStream();
            var writer = new StreamWriter(memory);
            var csv = new CsvWriter(writer);

            csv.Configuration.ShouldQuote = (field, context) => true;
            csv.Configuration.HasHeaderRecord = false;
            csv.WriteRecords(parametersForMachineLearningCollection);
            writer.Flush();

            byte[] csvByte = memory.ToArray();
            string fileName = $"parametersForMachineLearningCollection-{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(csvByte, "text/csv", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadParametersSupervisedCSV(int? exerciseID)
        {
            if (exerciseID == null)
            {
                return NotFound();
            }

            var exercise = await _arDbContext.Exercises.SingleOrDefaultAsync(q => q.ID == exerciseID);

            if (exercise == null)
            {
                return NotFound();
            }

            var assignmentRecordCollection = await _arDbContext.AssignmentRecords.Where(ar => ar.ExerciseID == exercise.ID && ar.IsFinished == true).ToListAsync();

            List<ParametersForMachineLearning> parametersSupervisedForMachineLearningCollection = new List<ParametersForMachineLearning>();

            for (int i = 0; i < assignmentRecordCollection.Count; i++)
            {
                var assignmentRecord = assignmentRecordCollection[i];

                var answerRecordCollection = _arDbContext.AnswserRecords.Where(ar => ar.AssignmentRecordID == assignmentRecord.ID).ToList();

                for (int j = 0; j < answerRecordCollection.Count; j++)
                {
                    var answerRecord = answerRecordCollection[j];

                    var movementCollection = await _arDbContext.Movements.Where(m => m.AnswerRecordID == answerRecord.ID).ToListAsync();
                    var deviceAccelerationCollection = await _arDbContext.DeviceAccelerations.Where(da => da.AnswerRecordID == answerRecord.ID).ToListAsync();

                    // supervise process
                    var movementSupervisor = new MovementSupervisor(movementCollection, deviceAccelerationCollection);
                    var movementSupervisedCollection = movementSupervisor.SuperviseByAcceleration();

                    var parametersForMachineLearning = new ParametersForMachineLearning();
                    parametersForMachineLearning.User = assignmentRecord.UserID;
                    parametersForMachineLearning.Question = j;
                    parametersForMachineLearning.Understand = answerRecord.ConfusionDegree;
                    parametersForMachineLearning.Date = answerRecord.EndDate;
                    parametersForMachineLearning.Check = 0;
                    parametersForMachineLearning.Time = (int)(answerRecord.EndDate - answerRecord.StartDate).TotalMilliseconds;
                    parametersForMachineLearning.Distance = ParameterAnalyzerForMovementSupervised.CalculateTotalDistance(movementSupervisedCollection);
                    parametersForMachineLearning.AverageSpeed = ParameterAnalyzerForMovementSupervised.CalculateDDSpeedAVG(movementSupervisedCollection);
                    parametersForMachineLearning.MaxSpeed = ParameterAnalyzerForMovementSupervised.CalculateDDSpeedMAX(movementSupervisedCollection);
                    parametersForMachineLearning.ThinkingTime = ParameterAnalyzerForMovementSupervised.CalculateDDFirstTime(movementSupervisedCollection);
                    parametersForMachineLearning.AnsweringTime = ParameterAnalyzerForMovementSupervised.CalculateDDFromFirstTime(movementSupervisedCollection);
                    parametersForMachineLearning.TotalStopTime = 0;
                    parametersForMachineLearning.MaxStopTime = 0;
                    parametersForMachineLearning.TotalDDIntervalTime = ParameterAnalyzerForMovementSupervised.CalculateDDIntervalTotal(movementSupervisedCollection);
                    parametersForMachineLearning.MaxDDIntervalTime = ParameterAnalyzerForMovementSupervised.CalculateDDIntervalMAX(movementSupervisedCollection);
                    parametersForMachineLearning.MaxDDTime = ParameterAnalyzerForMovementSupervised.CalculateDDProcessMAX(movementSupervisedCollection);
                    parametersForMachineLearning.MinDDTime = ParameterAnalyzerForMovementSupervised.CalculateDDProcessMIN(movementSupervisedCollection);
                    parametersForMachineLearning.DDCount = ParameterAnalyzerForMovementSupervised.CalculateDDCount(movementSupervisedCollection);
                    parametersForMachineLearning.GroupingDDCount = ParameterAnalyzerForMovementSupervised.CalculateGroupingDDCount(movementSupervisedCollection);
                    parametersForMachineLearning.XUTurnCount = ParameterAnalyzerForMovementSupervised.CalculateUTurnHorizontalCount(movementSupervisedCollection);
                    parametersForMachineLearning.YUTurnCount = ParameterAnalyzerForMovementSupervised.CalculateUTurnVerticalCount(movementSupervisedCollection);

                    parametersSupervisedForMachineLearningCollection.Add(parametersForMachineLearning);
                }
            }

            var memory = new MemoryStream();
            var writer = new StreamWriter(memory);
            var csv = new CsvWriter(writer);

            csv.Configuration.ShouldQuote = (field, context) => true;
            csv.Configuration.HasHeaderRecord = false;
            csv.WriteRecords(parametersSupervisedForMachineLearningCollection);
            writer.Flush();

            byte[] csvByte = memory.ToArray();
            string fileName = $"parametersSupervisedForMachineLearningCollection-{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(csvByte, "text/csv", fileName);
        }
    }
}