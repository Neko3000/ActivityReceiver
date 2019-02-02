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

                studentPresenterCollection.Add(studentPresenter);
            }

            return studentPresenterCollection;
        }
    }
}
