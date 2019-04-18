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

namespace ActivityReceiver.Controllers
{
    [Authorize]
    public class ExperimentAnalyzeController : Controller
    {
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ExperimentAnalyzeController(ActivityReceiverDbContext arDbContext, UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
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

            float thACC, thFOC;
            float step = 0.05f;
            MovementSupervisor movementSupervisor;
            ExperimentStatistician expermentStatistician = new ExperimentStatistician();

            var vm = new AbnormalTrajectoryViewModel
            {
                AbnormalTrajectoryEvaluationCollection = new List<AbnormalTrajectoryEvaluation>()
            };

            // ThACC, ThFOC
            thACC = 0.05f;
            thFOC = 0.05f;
            while (thACC <= 0.5f)
            {
                while (thFOC <= 0.5f)
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

                            movementSupervisor = new MovementSupervisor(movementCollection, deviceAccelerationCollection, thACC, thFOC);
                            var movementSupervised = movementSupervisor.Supervise();

                            var precision = expermentStatistician.CalculatePrecision(movementSupervised);
                            var recall = expermentStatistician.CalculateRecall(movementSupervised);
                            var fMeasure = expermentStatistician.CalculateFMeasure(movementSupervised);

                            precisionAverageForSingleAssignmentRecord += precision;
                            recallAverageForSingleAssignmentRecord += recall;
                            fMeasureAverageForSingleAssignmentRecord += fMeasure;
                        }

                        precisionAverageForAllAssignmentRecord += precisionAverageForSingleAssignmentRecord;
                        recallAverageForAllAssignmentRecord += recallAverageForAllAssignmentRecord;
                        fMeasureAverageForAllAssignmentRecord += fMeasureAverageForAllAssignmentRecord;
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

                    thFOC += step;
                }

                thACC += step;
            }

            for (int i = 0; i< assignmentRecordCollection.Count; i++)
            {
                var assignmentRecord = assignmentRecordCollection[i];
                var answerRecordCollection = await _arDbContext.AnswserRecords.Where(ar => ar.AssignmentRecordID == assignmentRecord.ID).ToListAsync();

                for(int j = 0; j<answerRecordCollection.Count; j++)
                {
                    var answerRecord = answerRecordCollection[j];
                    var movementCollection = await _arDbContext.Movements.Where(m => m.AnswerRecordID == answerRecord.ID).ToListAsync();
                    var deviceAccelerationCollection = await _arDbContext.DeviceAccelerations.Where(da => da.AnswerRecordID == answerRecord.ID).ToListAsync();

                    // ThACC, ThFOC
                    thACC = 0.05f;
                    thFOC = 0.05f;
                    while (thACC <= 0.5f)
                    {
                        while(thFOC <= 0.5f)
                        {
                            movementSupervisor = new MovementSupervisor(movementCollection, deviceAccelerationCollection, thACC, thFOC);
                            var movementSupervised = movementSupervisor.Supervise();

                            vm.AbnormalTrajectoryEvaluationCollection.Add(new AbnormalTrajectoryEvaluation
                            {
                                ThACC = thACC,
                                ThFOC = thFOC,

                                Precision = expermentStatistician.CalculatePrecision(movementSupervised),
                                Recall = expermentStatistician.CalculateRecall(movementSupervised),
                                FMeasure = expermentStatistician.CalculateFMeasure(movementSupervised),
                            });

                            thFOC += step;
                        }

                        thACC += step;
                    }


                }
            }

            return View(vm);
        }



    }
}