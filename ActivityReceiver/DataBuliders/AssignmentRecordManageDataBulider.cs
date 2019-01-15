using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Data;
using ActivityReceiver.Models;
using ActivityReceiver.ViewModels.AssignmentRecordManage;
using AutoMapper.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ActivityReceiver.Functions;

namespace ActivityReceiver.DataBuilders
{
    public class AssignmentRecordManageDataBulider
    {
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AssignmentRecordManageDataBulider(ActivityReceiverDbContext arDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _arDbContext = arDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IList<AssignmentRecordPresenter>> GetAssignmentRecordPresenterList()
        {
            var assignmentRecordList = await _arDbContext.AssignmentRecords.ToListAsync();

            var assignmentRecordPresenterList = new List<AssignmentRecordPresenter>();
            foreach(var assignmentRecord in assignmentRecordList)
            {
                var assignmentRecordPresenter = Mapper.Map<AssignmentRecord, AssignmentRecordPresenter>(assignmentRecord);

                assignmentRecordPresenter.Username = (await _userManager.FindByIdAsync(assignmentRecord.UserID)).UserName;
                assignmentRecordPresenter.ExerciseName =(await  _arDbContext.Exercises.FindAsync(assignmentRecord.ExerciseID)).Name;

                var sortedQuestions = (from q in _arDbContext.Questions
                                       join eqc in _arDbContext.ExerciseQuestionRelationMap on q.ID equals eqc.QuestionID
                                       where eqc.ExerciseID == assignmentRecord.ExerciseID
                                       orderby eqc.SerialNumber ascending
                                       select q).ToList();
                assignmentRecordPresenter.CurrentProgress = String.Format("{0}/{1}", assignmentRecord.CurrentQuestionIndex, sortedQuestions.Count);

                // Do not need it anymore in Index
                // var answers = await _arDbContext.Answsers.Where(a => a.AssignmentRecordID == assignmentRecord.ID).ToListAsync();
                // var answerPresenterCollection = AutoMapperHandler.ListMapper<Answer, AnswerPresenter>(answers);

                // assignmentRecordPresenter.AnswerPresenterCollection = answerPresenterCollection;

                assignmentRecordPresenterList.Add(assignmentRecordPresenter);
            }

            return assignmentRecordPresenterList;
        }
    }
}
