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
using ActivityReceiver.ViewModels;
using ActivityReceiver.Functions;

namespace ActivityReceiver.Controllers
{
    public class QuestionController : Controller
    {
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public QuestionController(ActivityReceiverDbContext arDbContext, UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _arDbContext = arDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: QuestionManage
        [HttpGet]
        public async Task<IActionResult> Index()
        {    
            var questions = await  _arDbContext.Questions.ToListAsync();
            var grammars = await  _arDbContext.Grammars.ToListAsync();
            var applicationUsers = await _userManager.Users.ToListAsync();

            var vm = new QuestionManageIndexViewModel {
                QuestionDTOs = ConvertToQuestionDTOForEachQuestion(questions,grammars,applicationUsers)
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
        public async Task<IActionResult> Create(QuestionmanageCreatePostViewModel model)
        { 
            if (ModelState.IsValid)
            {
                var question = Mapper.Map<QuestionmanageCreatePostViewModel,Question>(model);

                // I think grammar should be a multiple select and handled here.
                question.CreateDate = DateTime.Now;

                var user = await _userManager.GetUserAsync(HttpContext.User);
                question.EditorID = user.ID;

                _arDbContext.Questions.Add(question);
                await _arDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            //if isValid is false
            var vm = Mapper.Map<QuestionmanageCreatePostViewModel,QuestionmanageCreateGetViewModel>(model);

            var grammars =  await  _arDbContext.Grammars.ToListAsync();
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

            if (item == null)
            {
                return NotFound();
            }

            var vm = Mapper.Map<Question,QuestionManageEditGetViewModel>(question);

            vm.GrammarIDs = QuestionManageHandler.ConvertGrammarIDStringToGrammarIDList(question.Grammar);
            vm.Grammars = await  _arDbContext.Grammars.ToListAsync();

            var applicationUsers = await _userManager.Users.ToListAsync();
            vm.ApplicationUserDTOs = await ApplicationUserHandler.ConvertApplicationUsersToDTOs(_userManager, _roleManager, applicationUsers);

            return View(vm);
        }

        // POST: ItemManage/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ItemManageEditPostViewModel model)
        {

            var question = _arDbContext.Questions.Find(model.ID);

            if (question == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                question.Grammar = QuestionManageHandler.ConvertGrammarIDListToGrammarIDString(model.GrammarIDs);

                Mapper.Map<ItemManageEditPostViewModel,Item>(model,item);

                try
                {
                    _arDbContext.Update(item);
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
            vm.grammars = _arDbContext.Grammars.ToList();
            vm.applicationUsers = await _userManager.Users.ToListAsync();

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
            vm.GrammarNameString = QuestionManageHandler.ConvertGrammarIDStringToGrammarNameString(question.grammar);
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
            vm.GrammarNameString = QuestionManageHandler.ConvertGrammarIDStringToGrammarNameString(question.grammar);
            vm.EditorName = (await _userManager.FindByIdAsync(question.EditorID)).UserName;

            return View(vm);
        }

        // POST: ItemManage/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(QuestionManageDeletePost model)
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

            _arDbContext.Questions.Remove(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}