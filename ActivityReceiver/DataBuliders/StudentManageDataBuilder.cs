using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Data;
using ActivityReceiver.Models;
using ActivityReceiver.ViewModels.StudentManage;
using AutoMapper.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ActivityReceiver.Functions;

namespace ActivityReceiver.DataBuilders
{
    public class StudentManageDataBuilder
    {
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public StudentManageDataBuilder(ActivityReceiverDbContext arDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _arDbContext = arDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Builds
        public async Task<IList<StudentPresenter>> BuildStudentPresenterList()
        {
            var studentList = await _userManager.GetUsersInRoleAsync("Student");

            var studentPresenterCollection = new List<StudentPresenter>();
            foreach (var student in studentList)
            {
                var studentPresenter = Mapper.Map<ApplicationUser, StudentPresenter>(student);

                var assignmentRecords = await _arDbContext.AssignmentRecords.Where(ar => ar.UserID == student.Id).ToListAsync();

                float totalAccuracyRate = 0;
                for (int i =0;i<assignmentRecords.Count;i++)
                {
                    var answerRecords = await  _arDbContext.AnswserRecords.Where(q => q.AssignmentRecordID == assignmentRecords[i].ID).ToListAsync();
                    float accuracyRate = answerRecords.Where(a => a.IsCorrect == true).Count() / (float)answerRecords.Count;

                    totalAccuracyRate += accuracyRate/assignmentRecords.Count;
                }
                studentPresenter.AccuracyRate = totalAccuracyRate;

                studentPresenter.FinishedExerciseCount = (await _arDbContext.AssignmentRecords.Where(ar => ar.UserID == student.Id).ToListAsync()).Count;

                studentPresenterCollection.Add(studentPresenter);
            }

            return studentPresenterCollection;
        }
    }
}
