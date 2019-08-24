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
using System.Text;

namespace ActivityReceiver.Controllers
{
    //[Authorize]
    public class ExperimentAnalyzeController : Controller
    {
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private float thACCBest = 0.017f;
        private float thFOCBest = 0.019f;

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

        // GET: ExperimentAnalyze/AbnormalTrajectory/5
        [HttpGet]
        public async Task<IActionResult> CrossValiadationForThreshold(int? exerciseID)
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

            var allAnswerRecordCollectionForExercise = new List<AnswerRecord>();
            foreach(var assignmentRecord in assignmentRecordCollection)
            {
                allAnswerRecordCollectionForExercise.AddRange(_arDbContext.AnswserRecords.Where(ar => ar.AssignmentRecordID == assignmentRecord.ID).ToList());
            }

            var vm = new AbnormalTrajectoryViewModel
            {
                AbnormalTrajectoryEvaluationCollection = new List<AbnormalTrajectoryEvaluation>()
            };

            float theBestThACCForTestData = -1;
            float theBestThFOCForTestData = -1;
            float theBestFMeasureForTestData = -1;

            var thresholdAndFMeasureForFoldCollection = new List<ThresholdAndFMeasureForFold>();
            int foldCount = 10;

            for (int i = 0; i < foldCount; i++)
            {
                // TestData
                var r = new Random();
                // var selectedAnswerRecordCollection = allAnswerRecordCollectionForExercise.OrderBy(x => r.Next()).Take((int)(allAnswerRecordCollectionForExercise.Count * ((foldCount - 1) / (float)foldCount))).ToList();
                var selectedAnswerRecordCollection = allAnswerRecordCollectionForExercise.OrderBy(x => r.Next()).Take((int)(allAnswerRecordCollectionForExercise.Count * (1) / (float)foldCount)).ToList();

                // TrainData
                float theBestThACCForTrainData = -1;
                float theBestThFOCForTrainData = -1;
                float theBestFMeasureForTrainData = -1;

                float fMeasureForOneThresholdPairForTrainData = 0;

                float fMeasureAverageForTestData = 0;

                float thACC = 0.005f;
                float stepThACC = 0.001f;
                while (thACC <= 0.030f)
                {
                    float thFOC = 0.005f;
                    float stepThFOC = 0.001f;
                    while (thFOC <= 0.020f)
                    {

                        for (int j = 0; j < selectedAnswerRecordCollection.Count; j++)
                        {
                            var answerRecord = selectedAnswerRecordCollection[j];
                            var movementCollection = await _arDbContext.Movements.Where(m => m.AnswerRecordID == answerRecord.ID).ToListAsync();
                            var deviceAccelerationCollection = await _arDbContext.DeviceAccelerations.Where(da => da.AnswerRecordID == answerRecord.ID).ToListAsync();

                            var movementSupervisor = new MovementSupervisor(movementCollection, deviceAccelerationCollection, thACC, thFOC);
                            var movementSupervised = movementSupervisor.Supervise();

                            var expermentStatistician = new ExperimentStatistician();
                            var precision = expermentStatistician.CalculatePrecision(movementSupervised);
                            var recall = expermentStatistician.CalculateRecall(movementSupervised);
                            var fMeasure = expermentStatistician.CalculateFMeasure(movementSupervised);
                            fMeasureForOneThresholdPairForTrainData += fMeasure;

                        }

                        fMeasureForOneThresholdPairForTrainData /= selectedAnswerRecordCollection.Count;

                        if (fMeasureForOneThresholdPairForTrainData > theBestFMeasureForTrainData)
                        {
                            theBestThACCForTrainData = thACC;
                            theBestThFOCForTrainData = thFOC;
                            theBestFMeasureForTrainData = fMeasureForOneThresholdPairForTrainData;
                        }

                        thFOC += stepThFOC;
                    }

                    thACC += stepThACC;
                }

                var thresholdAndFMeasureForFoldTrainData = new ThresholdAndFMeasureForFold();
                thresholdAndFMeasureForFoldTrainData.theBestThACC = theBestThACCForTrainData;
                thresholdAndFMeasureForFoldTrainData.theBestThFOC = theBestThFOCForTrainData;
                thresholdAndFMeasureForFoldTrainData.theBestFMeasure = theBestFMeasureForTrainData;

                thresholdAndFMeasureForFoldCollection.Add(thresholdAndFMeasureForFoldTrainData);

                for (int k = 0; k < allAnswerRecordCollectionForExercise.Count; k++)
                {
                    var answerRecord = allAnswerRecordCollectionForExercise[k];
                    var movementCollection = await _arDbContext.Movements.Where(m => m.AnswerRecordID == answerRecord.ID).ToListAsync();
                    var deviceAccelerationCollection = await _arDbContext.DeviceAccelerations.Where(da => da.AnswerRecordID == answerRecord.ID).ToListAsync();

                    var movementSupervisor = new MovementSupervisor(movementCollection, deviceAccelerationCollection, theBestThACCForTrainData, theBestThFOCForTrainData);
                    var movementSupervised = movementSupervisor.Supervise();

                    var expermentStatistician = new ExperimentStatistician();
                    var fMeasure = expermentStatistician.CalculateFMeasure(movementSupervised);
                    fMeasureAverageForTestData += fMeasure;
                }
                fMeasureAverageForTestData /= allAnswerRecordCollectionForExercise.Count;

                if (fMeasureAverageForTestData > theBestFMeasureForTestData)
                {
                    theBestThACCForTestData = theBestThACCForTrainData;
                    theBestThFOCForTestData = theBestThFOCForTrainData;
                    theBestFMeasureForTestData = fMeasureAverageForTestData;
                }

                //var thresholdAndFMeasureForFold = new ThresholdAndFMeasureForFold();
                //thresholdAndFMeasureForFold.theBestThACC = theBestThACCForTrainData;
                //thresholdAndFMeasureForFold.theBestThFOC = theBestThFOCForTrainData;
                //thresholdAndFMeasureForFold.theBestFMeasure = fMeasureAverageForTestData;

                //thresholdAndFMeasureForFoldCollection.Add(thresholdAndFMeasureForFold);
            }

            var memory = new MemoryStream();
            var writer = new StreamWriter(memory);
            var csv = new CsvWriter(writer);

            csv.Configuration.ShouldQuote = (field, context) => true;
            csv.Configuration.HasHeaderRecord = true;
            csv.WriteRecords(thresholdAndFMeasureForFoldCollection);
            writer.Flush();

            byte[] csvByte = memory.ToArray();
            string fileName = $"thresholdAndFMeasureForFoldCollection-{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(csvByte, "text/csv", fileName);
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

                if((await _userManager.FindByIdAsync(assignmentRecord.UserID)).UserName == "Ikeda" || (await _userManager.FindByIdAsync(assignmentRecord.UserID)).UserName == "Tanaka")
                {
                    continue;
                }

                var answerRecordCollection = _arDbContext.AnswserRecords.Where(ar => ar.AssignmentRecordID == assignmentRecord.ID).ToList();

                for (int j = 0; j < answerRecordCollection.Count; j++)
                {
                    var answerRecord = answerRecordCollection[j];

                    var movementCollection = await _arDbContext.Movements.Where(m => m.AnswerRecordID == answerRecord.ID).ToListAsync();

                    var parametersForMachineLearning = new ParametersForMachineLearning();
                    parametersForMachineLearning.User = i.ToString();
                    parametersForMachineLearning.Question = j;
                    parametersForMachineLearning.Understand = 4 - answerRecord.ConfusionDegree;
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

                if ((await _userManager.FindByIdAsync(assignmentRecord.UserID)).UserName == "Ikeda" || (await _userManager.FindByIdAsync(assignmentRecord.UserID)).UserName == "Tanaka")
                {
                    continue;
                }

                var answerRecordCollection = _arDbContext.AnswserRecords.Where(ar => ar.AssignmentRecordID == assignmentRecord.ID).ToList();

                for (int j = 0; j < answerRecordCollection.Count; j++)
                {
                    var answerRecord = answerRecordCollection[j];

                    var movementCollection = await _arDbContext.Movements.Where(m => m.AnswerRecordID == answerRecord.ID).ToListAsync();
                    var deviceAccelerationCollection = await _arDbContext.DeviceAccelerations.Where(da => da.AnswerRecordID == answerRecord.ID).ToListAsync();

                    // supervise process
                    var movementSupervisor = new MovementSupervisor(movementCollection, deviceAccelerationCollection, thACCBest, thFOCBest);
                    var movementSupervisedCollection = movementSupervisor.SuperviseByAcceleration();

                    var parametersForMachineLearning = new ParametersForMachineLearning();
                    parametersForMachineLearning.User = i.ToString();
                    parametersForMachineLearning.Question = j;
                    parametersForMachineLearning.Understand = 4 - answerRecord.ConfusionDegree;
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

        [HttpGet]
        public async Task<IActionResult> CalculateATDifferencePercentageForEachUser(int? exerciseID)
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

            List<ATDifferencePercentage> atDifferencePercentageCollection = new List<ATDifferencePercentage>();

            for (int i = 0; i < assignmentRecordCollection.Count; i++)
            {
                var assignmentRecord = assignmentRecordCollection[i];

                var answerRecordCollection = _arDbContext.AnswserRecords.Where(ar => ar.AssignmentRecordID == assignmentRecord.ID).ToList();

                var atDifferencePercentage = new ATDifferencePercentage();
                atDifferencePercentage.Username = (await _userManager.FindByIdAsync(assignmentRecord.UserID)).UserName;

                atDifferencePercentage.DDIntervalAVGDifferencePercentage = 0;
                atDifferencePercentage.DDIntervalMAXDifferencePercentage = 0;
                atDifferencePercentage.DDIntervalMINDifferencePercentage = 0;
                atDifferencePercentage.DDProcessAVGDifferencePercentage = 0;
                atDifferencePercentage.DDProcessMAXDifferencePercentage = 0;
                atDifferencePercentage.DDProcessMINDifferencePercentage = 0;
                atDifferencePercentage.TotalDistanceDifferencePercentage = 0;
                atDifferencePercentage.DDSpeedAVGDifferencePercentage = 0;
                atDifferencePercentage.DDSpeedMAXDifferencePercentage = 0;
                atDifferencePercentage.DDSpeedMINDifferencePercentage = 0;
                atDifferencePercentage.DDFirstTimeDifferencePercentage = 0;
                atDifferencePercentage.DDCountDifferencePercentage = 0;
                atDifferencePercentage.UTurnHorizontalCountDifferencePercentage = 0;
                atDifferencePercentage.UTurnVerticalCountDifferencePercentage = 0;
                atDifferencePercentage.AbnormalTrajectoryCount = 0;

                int movmentTotalCount = 0;
                int abnormalTrajectoryPointTotalCount = 0;

                for (int j = 0; j < answerRecordCollection.Count; j++)
                {
                    var answerRecord = answerRecordCollection[j];

                    var movementCollection = await _arDbContext.Movements.Where(m => m.AnswerRecordID == answerRecord.ID).ToListAsync();
                    var deviceAccelerationCollection = await _arDbContext.DeviceAccelerations.Where(da => da.AnswerRecordID == answerRecord.ID).ToListAsync();

                    // supervise process
                    var movementSupervisor = new MovementSupervisor(movementCollection, deviceAccelerationCollection , thACCBest ,thFOCBest);
                    var movementSupervisedCollection = movementSupervisor.SuperviseByAcceleration();

                    atDifferencePercentage.DDIntervalAVGDifferencePercentage += ExperimentStatistician.CalculateDifferencePercentage(ParameterAnalyzer.CalculateDDIntervalAVG(movementCollection), ParameterAnalyzerForMovementSupervised.CalculateDDIntervalAVG(movementSupervisedCollection));
                    atDifferencePercentage.DDIntervalMAXDifferencePercentage += ExperimentStatistician.CalculateDifferencePercentage(ParameterAnalyzer.CalculateDDIntervalMAX(movementCollection), ParameterAnalyzerForMovementSupervised.CalculateDDIntervalMAX(movementSupervisedCollection));
                    atDifferencePercentage.DDIntervalMINDifferencePercentage += ExperimentStatistician.CalculateDifferencePercentage(ParameterAnalyzer.CalculateDDIntervalMIN(movementCollection), ParameterAnalyzerForMovementSupervised.CalculateDDIntervalMIN(movementSupervisedCollection));
                    atDifferencePercentage.DDProcessAVGDifferencePercentage += ExperimentStatistician.CalculateDifferencePercentage(ParameterAnalyzer.CalculateDDProcessAVG(movementCollection), ParameterAnalyzerForMovementSupervised.CalculateDDProcessAVG(movementSupervisedCollection));
                    atDifferencePercentage.DDProcessMAXDifferencePercentage += ExperimentStatistician.CalculateDifferencePercentage(ParameterAnalyzer.CalculateDDProcessMAX(movementCollection), ParameterAnalyzerForMovementSupervised.CalculateDDProcessMAX(movementSupervisedCollection));
                    atDifferencePercentage.DDProcessMINDifferencePercentage += ExperimentStatistician.CalculateDifferencePercentage(ParameterAnalyzer.CalculateDDProcessMIN(movementCollection), ParameterAnalyzerForMovementSupervised.CalculateDDProcessMIN(movementSupervisedCollection));
                    atDifferencePercentage.TotalDistanceDifferencePercentage += ExperimentStatistician.CalculateDifferencePercentage(ParameterAnalyzer.CalculateTotalDistance(movementCollection), ParameterAnalyzerForMovementSupervised.CalculateTotalDistance(movementSupervisedCollection));
                    atDifferencePercentage.DDSpeedAVGDifferencePercentage += ExperimentStatistician.CalculateDifferencePercentage(ParameterAnalyzer.CalculateDDSpeedAVG(movementCollection), ParameterAnalyzerForMovementSupervised.CalculateDDSpeedAVG(movementSupervisedCollection));
                    atDifferencePercentage.DDSpeedMAXDifferencePercentage += ExperimentStatistician.CalculateDifferencePercentage(ParameterAnalyzer.CalculateDDSpeedMAX(movementCollection), ParameterAnalyzerForMovementSupervised.CalculateDDSpeedMAX(movementSupervisedCollection));
                    atDifferencePercentage.DDSpeedMINDifferencePercentage += ExperimentStatistician.CalculateDifferencePercentage(ParameterAnalyzer.CalculateDDSpeedMIN(movementCollection), ParameterAnalyzerForMovementSupervised.CalculateDDSpeedMIN(movementSupervisedCollection));
                    atDifferencePercentage.DDFirstTimeDifferencePercentage += ExperimentStatistician.CalculateDifferencePercentage(ParameterAnalyzer.CalculateDDFirstTime(movementCollection), ParameterAnalyzerForMovementSupervised.CalculateDDFirstTime(movementSupervisedCollection));
                    atDifferencePercentage.DDCountDifferencePercentage += ExperimentStatistician.CalculateDifferencePercentage(ParameterAnalyzer.CalculateDDCount(movementCollection), ParameterAnalyzerForMovementSupervised.CalculateDDCount(movementSupervisedCollection));
                    atDifferencePercentage.UTurnHorizontalCountDifferencePercentage += ExperimentStatistician.CalculateDifferencePercentage(ParameterAnalyzer.CalculateUTurnHorizontalCount(movementCollection), ParameterAnalyzerForMovementSupervised.CalculateUTurnHorizontalCount(movementSupervisedCollection));
                    atDifferencePercentage.UTurnVerticalCountDifferencePercentage += ExperimentStatistician.CalculateDifferencePercentage(ParameterAnalyzer.CalculateUTurnVerticalCount(movementCollection), ParameterAnalyzerForMovementSupervised.CalculateUTurnVerticalCount(movementSupervisedCollection));

                    atDifferencePercentage.AbnormalTrajectoryCount += MovementSupervisor.CalculateAbnormalTrajectoryCount(movementSupervisedCollection);

                    atDifferencePercentage.AbnormalTrajectoryPointCount += movementSupervisedCollection.Where(msc => msc.IsAbnormal == true).ToList().Count;

                    movmentTotalCount += movementCollection.Count;
                    abnormalTrajectoryPointTotalCount += movementSupervisedCollection.Where(msc=>msc.IsAbnormal == true).ToList().Count;
                }

                atDifferencePercentage.DDIntervalAVGDifferencePercentage /= answerRecordCollection.Count;
                atDifferencePercentage.DDIntervalMAXDifferencePercentage /= answerRecordCollection.Count;
                atDifferencePercentage.DDIntervalMINDifferencePercentage /= answerRecordCollection.Count;
                atDifferencePercentage.DDProcessAVGDifferencePercentage /= answerRecordCollection.Count;
                atDifferencePercentage.DDProcessMAXDifferencePercentage /= answerRecordCollection.Count;
                atDifferencePercentage.DDProcessMINDifferencePercentage /= answerRecordCollection.Count;
                atDifferencePercentage.TotalDistanceDifferencePercentage /= answerRecordCollection.Count;
                atDifferencePercentage.DDSpeedAVGDifferencePercentage /= answerRecordCollection.Count;
                atDifferencePercentage.DDSpeedMAXDifferencePercentage /= answerRecordCollection.Count;
                atDifferencePercentage.DDSpeedMINDifferencePercentage /= answerRecordCollection.Count;
                atDifferencePercentage.DDFirstTimeDifferencePercentage /= answerRecordCollection.Count;
                atDifferencePercentage.DDCountDifferencePercentage /= answerRecordCollection.Count;
                atDifferencePercentage.UTurnHorizontalCountDifferencePercentage /= answerRecordCollection.Count;
                atDifferencePercentage.UTurnVerticalCountDifferencePercentage /= answerRecordCollection.Count;

                atDifferencePercentage.AbnormalTrajectoryPointPercentage =  abnormalTrajectoryPointTotalCount / (float)movmentTotalCount;

                atDifferencePercentageCollection.Add(atDifferencePercentage);
            }

            var memory = new MemoryStream();
            var writer = new StreamWriter(memory);
            var csv = new CsvWriter(writer);

            csv.Configuration.ShouldQuote = (field, context) => true;
            csv.Configuration.HasHeaderRecord = true;
            csv.WriteRecords(atDifferencePercentageCollection);
            writer.Flush();

            byte[] csvByte = memory.ToArray();
            string fileName = $"atDifferencePercentageCollection-{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(csvByte, "text/csv", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadQuestionGrammarAndWordCountForExercise(int? exerciseID)
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

            var sortedQuestions = (from q in _arDbContext.Questions
                                   join eqc in _arDbContext.ExerciseQuestionRelationMap on q.ID equals eqc.QuestionID
                                   where eqc.ExerciseID == exercise.ID
                                   orderby eqc.SerialNumber ascending
                                   select q).ToList();

            var grammarCollection = _arDbContext.Grammars.ToList();

            var questionGrammarAndWordCountCollection = new List<QuestionGrammarAndWordCount>();

            for (int i = 0; i < sortedQuestions.Count; i++)
            {
                var question = sortedQuestions[i];

                var questionGrammarAndWordCount = new QuestionGrammarAndWordCount();
                if (question.GrammarIDString!="-1")
                {
                    var grammarIndexCollectionForQuestion = question.GrammarIDString.Split('#');

                    for(int j = 0; j < grammarIndexCollectionForQuestion.Count(); j++)
                    {
                        switch(grammarIndexCollectionForQuestion[j])
                        {
                            case "1":
                                questionGrammarAndWordCount.Grammar1 = "bingo";
                                break;
                            case "2":
                                questionGrammarAndWordCount.Grammar2 = "bingo";
                                break;
                            case "3":
                                questionGrammarAndWordCount.Grammar3 = "bingo";
                                break;
                            case "4":
                                questionGrammarAndWordCount.Grammar4 = "bingo";
                                break;
                            case "5":
                                questionGrammarAndWordCount.Grammar5 = "bingo";
                                break;
                            case "6":
                                questionGrammarAndWordCount.Grammar6 = "bingo";
                                break;
                            case "7":
                                questionGrammarAndWordCount.Grammar7 = "bingo";
                                break;
                            case "8":
                                questionGrammarAndWordCount.Grammar8 = "bingo";
                                break;
                            case "9":
                                questionGrammarAndWordCount.Grammar9 = "bingo";
                                break;
                            case "10":
                                questionGrammarAndWordCount.Grammar10 = "bingo";
                                break;
                            case "11":
                                questionGrammarAndWordCount.Grammar11 = "bingo";
                                break;
                            case "12":
                                questionGrammarAndWordCount.Grammar12 = "bingo";
                                break;
                            case "13":
                                questionGrammarAndWordCount.Grammar13 = "bingo";
                                break;
                            case "14":
                                questionGrammarAndWordCount.Grammar14 = "bingo";
                                break;
                            case "15":
                                questionGrammarAndWordCount.Grammar15 = "bingo";
                                break;
                            case "16":
                                questionGrammarAndWordCount.Grammar16 = "bingo";
                                break;
                            case "17":
                                questionGrammarAndWordCount.Grammar17 = "bingo";
                                break;
                            case "18":
                                questionGrammarAndWordCount.Grammar18 = "bingo";
                                break;
                            case "19":
                                questionGrammarAndWordCount.Grammar19 = "bingo";
                                break;
                            case "20":
                                questionGrammarAndWordCount.Grammar20 = "bingo";
                                break;
                            case "21":
                                questionGrammarAndWordCount.Grammar21 = "bingo";
                                break;
                            default:
                                break;
                        }

                    }


                }

                questionGrammarAndWordCount.QuestionID = i + 1;
                questionGrammarAndWordCount.WordCount = (question.StandardAnswerDivision.Split('|').Count());
                questionGrammarAndWordCountCollection.Add(questionGrammarAndWordCount);
            }

            var memory = new MemoryStream();
            var writer = new StreamWriter(memory);
            var csv = new CsvWriter(writer);

            csv.Configuration.ShouldQuote = (field, context) => true;
            csv.Configuration.HasHeaderRecord = false;
            csv.WriteRecords(questionGrammarAndWordCountCollection);
            writer.Flush();

            byte[] csvByte = memory.ToArray();
            string fileName = $"questionGrammarAndWordCountCollection-{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(csvByte, "text/csv", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadConfusionDegreeForAll(int? exerciseID)
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
            var allAnswerRecordCollection = _arDbContext.AnswserRecords.ToList();

            var confusionDegreeDisturbutionCollection = new List<ConfusionDegreeDisturbution>();

            var answerRecordCollection = new List<AnswerRecord>();
            foreach(var assignmentRecord in assignmentRecordCollection)
            {
                answerRecordCollection.AddRange(allAnswerRecordCollection.Where(arc => arc.AssignmentRecordID == assignmentRecord.ID).ToList());
            }

            var confusionDegreeDisturbution = new ConfusionDegreeDisturbution();
            confusionDegreeDisturbution.confusion0 = answerRecordCollection.Where(arc=>arc.ConfusionDegree == 0).Count();
            confusionDegreeDisturbution.confusion1 = answerRecordCollection.Where(arc => arc.ConfusionDegree == 1).Count();
            confusionDegreeDisturbution.confusion2 = answerRecordCollection.Where(arc => arc.ConfusionDegree == 2).Count();
            confusionDegreeDisturbution.confusion3 = answerRecordCollection.Where(arc => arc.ConfusionDegree == 3).Count();
            confusionDegreeDisturbutionCollection.Add(confusionDegreeDisturbution);

            var memory = new MemoryStream();
            var writer = new StreamWriter(memory);
            var csv = new CsvWriter(writer);

            csv.Configuration.ShouldQuote = (field, context) => true;
            csv.Configuration.HasHeaderRecord = true;
            csv.WriteRecords(confusionDegreeDisturbutionCollection);
            writer.Flush();

            byte[] csvByte = memory.ToArray();
            string fileName = $"confusionDegreeDisturbutionCollection-{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(csvByte, "text/csv", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadQuestionCollection(int? exerciseID)
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

            var sortedQuestions = (from q in _arDbContext.Questions
                                   join eqc in _arDbContext.ExerciseQuestionRelationMap on q.ID equals eqc.QuestionID
                                   where eqc.ExerciseID == exercise.ID
                                   orderby eqc.SerialNumber ascending
                                   select q).ToList();

            var questionEntityCollection = new List<QuestionEntity>();

            for(int i=0;i< sortedQuestions.Count;i++)
            {
                var question = sortedQuestions[i];

                var questionEntity = new QuestionEntity();
                questionEntity.SerialNumber = i + 1;
                questionEntity.Japanese = question.SentenceJP;
                questionEntity.English = question.SentenceEN;

                questionEntityCollection.Add(questionEntity);
            }

            var memory = new MemoryStream();
            var writer = new StreamWriter(memory, Encoding.UTF8);
            var csv = new CsvWriter(writer);

            csv.Configuration.ShouldQuote = (field, context) => true;
            csv.Configuration.HasHeaderRecord = true;
            csv.WriteRecords(questionEntityCollection);
            writer.Flush();

            byte[] csvByte = memory.ToArray();
            string fileName = $"questionEntityCollection-{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(csvByte, "text/csv", fileName);
        }
    }
}