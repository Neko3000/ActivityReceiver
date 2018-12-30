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
using ActivityReceiver.ViewModels.QuestionManage;
using ActivityReceiver.Functions;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ActivityReceiver.DataBuilders;

namespace ActivityReceiver.Controllers
{
    [Authorize]
    public class QuestionManageController : Controller
    {
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly QuestionManageDataBuilder _dataBuilder;

        public QuestionManageController(ActivityReceiverDbContext arDbContext, UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _arDbContext = arDbContext;
            _userManager = userManager;
            _roleManager = roleManager;

            _dataBuilder = new QuestionManageDataBuilder(_arDbContext, _userManager, _roleManager);
        }

        // GET: QuestionManage
        [HttpGet]
        public async Task<IActionResult> Index()
        {    
            var questions = await  _arDbContext.Questions.ToListAsync();

            var vm = new QuestionManageIndexViewModel
            {
                QuestionPresenterCollection = await _dataBuilder.BuildQuestionPresenterList()
            };


            return View(vm);
        }

        // GET: QuestionManage/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {            
            var grammars =  await  _arDbContext.Grammars.ToListAsync();

            var vm = new QuestionManageCreateGetViewModel{
               Grammars = _arDbContext.Grammars.ToList()
            };
            return View(vm);
        }

        // POST: QuestionManage/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionManageCreatePostViewModel model)
        {
            if (ModelState.IsValid)
            {
                var question = Mapper.Map<QuestionManageCreatePostViewModel,Question>(model);

                // I think grammar should be a multiple select and handled here.
                question.GrammarIDString = QuestionManageHandler.ConvertGrammarIDListToGrammarIDString(model.GrammarIDs);
                question.CreateDate = DateTime.Now;

                var user = await _userManager.GetUserAsync(HttpContext.User);
                question.EditorID = user.Id;

                _arDbContext.Questions.Add(question);
                await _arDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            //if isValid is false
            var vm = Mapper.Map<QuestionManageCreatePostViewModel,QuestionManageCreateGetViewModel>(model);

            vm.Grammars = _arDbContext.Grammars.ToList();

            return View(vm);
        }

        // GET: QuestionManage/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _arDbContext.Questions.SingleOrDefaultAsync(m => m.ID == id);

            if (question == null)
            {
                return NotFound();
            }

            var vm = Mapper.Map<Question,QuestionManageEditGetViewModel>(question);

            vm.GrammarIDs = QuestionManageHandler.ConvertGrammarIDStringToGrammarIDList(question.GrammarIDString);
            vm.Grammars = await  _arDbContext.Grammars.ToListAsync();

            var applicationUsers = await _userManager.Users.ToListAsync();
            vm.ApplicationUserPresenterCollection = await ApplicationUserHandler.ConvertApplicationUsersToPresenterCollection(_userManager, _roleManager, applicationUsers);

            return View(vm);
        }

        // POST: ItemManage/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(QuestionManageEditPostViewModel model)
        {

            var question = _arDbContext.Questions.Find(model.ID);

            if (question == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                question.GrammarIDString = QuestionManageHandler.ConvertGrammarIDListToGrammarIDString(model.GrammarIDs);

                Mapper.Map<QuestionManageEditPostViewModel,Question>(model, question);

                try
                {
                    _arDbContext.Update(question);
                    await _arDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_arDbContext.Questions.Any(q=>q.ID == question.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var vm = Mapper.Map<QuestionManageEditPostViewModel,QuestionManageEditGetViewModel>(model); 
            vm.Grammars = _arDbContext.Grammars.ToList();
            vm.ApplicationUserPresenterCollection = await ApplicationUserHandler.ConvertApplicationUsersToPresenterCollection(_userManager, _roleManager, _userManager.Users.ToList());

            return View(vm);
        }

        // GET: ItemManage/Delete/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _arDbContext.Questions.SingleOrDefaultAsync(q => q.ID == id);

            if (question == null)
            {
                return NotFound();
            }

            var vm = Mapper.Map<Question, QuestionManageDetailsViewModel>(question);
            vm.GrammarNameString = QuestionManageHandler.ConvertGrammarIDStringToGrammarNameString(question.GrammarIDString,_arDbContext.Grammars.ToList());
            vm.EditorName = (await _userManager.FindByIdAsync(question.EditorID)).UserName;

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _arDbContext.Questions.SingleOrDefaultAsync(q => q.ID == id);

            if (question == null)
            {
                return NotFound();
            }

            var vm = Mapper.Map<Question, QuestionManageDeleteGetViewModel>(question);
            vm.GrammarNameString = QuestionManageHandler.ConvertGrammarIDStringToGrammarNameString(question.GrammarIDString,_arDbContext.Grammars.ToList());
            vm.EditorName = (await _userManager.FindByIdAsync(question.EditorID)).UserName;

            return View(vm);
        }

        // POST: ItemManage/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(QuestionManageDeletePostViewModel model)
        {  
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var question = await _arDbContext.Questions.SingleOrDefaultAsync(q => q.ID == model.ID);

            if (question == null)
            {
                return NotFound();
            }

            _arDbContext.Questions.Remove(question);
            await _arDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}