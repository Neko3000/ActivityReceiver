using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Data;
using ActivityReceiver.Models;
using ActivityReceiver.ViewModels.AnswerRecordManage;
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


        public async Task<IList<AnswerRecordPresenter>> BuildAnswerRecordPresenterList(int? assignmentRecordID)
        {
            var answerRecordList = await _arDbContext.AnswserRecords.ToListAsync();

            if (assignmentRecordID != null)
            {
                answerRecordList = answerRecordList.Where(ar => ar.AssignmentRecordID == assignmentRecordID).ToList();
            }
            
            var answerRecordPresenterCollection = new List<AnswerRecordPresenter>();
            foreach (var answerRecord in answerRecordList)
            {
                var answerRecordPresenter = Mapper.Map<AnswerRecord, AnswerRecordPresenter>(answerRecord);

                var movementCollection = await _arDbContext.Movements.Where(m => m.AnswerRecordID == answerRecord.ID).ToListAsync();

                // Parameter Analyze
                answerRecordPresenter.DDIntervalAVG = ParameterAnalyzer.CalculateDDIntervalAVG(movementCollection);
                answerRecordPresenter.DDIntervalMAX = ParameterAnalyzer.CalculateDDIntervalMAX(movementCollection);
                answerRecordPresenter.DDIntervalMIN = ParameterAnalyzer.CalculateDDIntervalMIN(movementCollection);
                answerRecordPresenter.DDProcessAVG = ParameterAnalyzer.CalculateDDProcessAVG(movementCollection);
                answerRecordPresenter.DDProcessMAX = ParameterAnalyzer.CalculateDDProcessMAX(movementCollection);
                answerRecordPresenter.DDProcessMIN = ParameterAnalyzer.CalculateDDProcessMIN(movementCollection);
                answerRecordPresenter.TotalDistance = ParameterAnalyzer.CalculateTotalDistance(movementCollection);
                answerRecordPresenter.DDSpeedAVG = ParameterAnalyzer.CalculateDDSpeedAVG(movementCollection);
                answerRecordPresenter.DDSpeedMAX = ParameterAnalyzer.CalculateDDSpeedMAX(movementCollection);
                answerRecordPresenter.DDSpeedMIN = ParameterAnalyzer.CalculateDDSpeedMIN(movementCollection);
                answerRecordPresenter.DDFirstTime = ParameterAnalyzer.CalculateDDFirstTime(movementCollection);
                answerRecordPresenter.DDCount = ParameterAnalyzer.CalculateDDCount(movementCollection);
                answerRecordPresenter.UTurnHorizontalCount = ParameterAnalyzer.CalculateUTurnHorizontalCount(movementCollection);
                answerRecordPresenter.UTurnVerticalCount = ParameterAnalyzer.CalculateUTurnVerticalCount(movementCollection);

                answerRecordPresenterCollection.Add(answerRecordPresenter);
            }

            return answerRecordPresenterCollection;
        }
    }
}
