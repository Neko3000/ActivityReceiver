using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Data;
using ActivityReceiver.Models;
using ActivityReceiver.ViewModels.AnswerManage;
using AutoMapper.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ActivityReceiver.Functions;

namespace ActivityReceiver.DataBuilder
{
    public class AnswerManageDataBuilder
    {
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AnswerManageDataBuilder(ActivityReceiverDbContext arDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _arDbContext = arDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IList<AnswerPresenter>> BuildAnswerPresenterList()
        {
            var answerList = await _arDbContext.Answsers.ToListAsync();
            var answerPresenterCollection = AutoMapperHandler.ListMapper<Answer, AnswerPresenter>(answerList);

            return answerPresenterCollection;
        }

        public async Task<AnswerManageDetailsViewModel> BuildAnswerManageDetailsViewModel(int? id)
        {
            var answer = await _arDbContext.Answsers.FindAsync(id);

            var vm = Mapper.Map<Answer, AnswerManageDetailsViewModel>(answer);

            var movementCollection = await _arDbContext.Movements.Where(m => m.AnswerID == answer.ID).ToListAsync();
            var deviceAccelerationCollection = await _arDbContext.DeviceAccelerations.Where(da => da.AnswerID == answer.ID).ToListAsync();

            vm.MovementCollection = movementCollection;
            vm.DeviceAccelerationCollection = deviceAccelerationCollection;

            return vm;
        }
    }
}
