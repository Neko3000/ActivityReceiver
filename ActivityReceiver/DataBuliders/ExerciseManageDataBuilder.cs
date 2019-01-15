using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Data;
using ActivityReceiver.Models;
using ActivityReceiver.ViewModels.ExerciseManage;
using AutoMapper.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ActivityReceiver.Functions;

namespace ActivityReceiver.DataBuilders
{
    public class ExerciseManageDataBuilder
    {
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ExerciseManageDataBuilder(ActivityReceiverDbContext arDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _arDbContext = arDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Builds
        public async Task<IList<ExercisePresenter>> BuildExercisePresenterList()
        {
            var exereciseList = await _arDbContext.Exercises.ToListAsync();

            var exerecisePresenterCollection = new List<ExercisePresenter>();
            foreach (var exercise in exereciseList)
            {
                var exerecisePresenter = Mapper.Map<Exercise, ExercisePresenter>(exercise);
                exerecisePresenter.EditorName = (await _userManager.FindByIdAsync(exercise.EditorID)).UserName;

                var allQuestionsInExercise = (from q in _arDbContext.Questions
                                              join eqc in _arDbContext.ExerciseQuestionRelationMap on q.ID equals eqc.QuestionID
                                              where eqc.ExerciseID == exercise.ID
                                              orderby eqc.SerialNumber ascending
                                              select q).ToList();
                exerecisePresenter.QuestionCollection = allQuestionsInExercise;

                exerecisePresenterCollection.Add(exerecisePresenter);
            }

            return exerecisePresenterCollection;
        }
    }
}
