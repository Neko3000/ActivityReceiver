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

namespace ActivityReceiver.DataBuilders
{
    public class AnswerRecordManageDataBuilder
    {
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AnswerRecordManageDataBuilder(ActivityReceiverDbContext arDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _arDbContext = arDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IList<AnswerRecordPresenter>> BuildAnswerRecordPresenterList()
        {
            var answerRecordList = await _arDbContext.AnswserRecords.ToListAsync();
            var answerRecordPresenterCollection = AutoMapperHandler.ListMapper<AnswerRecord, AnswerRecordPresenter>(answerRecordList);

            return answerRecordPresenterCollection;
        }
    }
}
