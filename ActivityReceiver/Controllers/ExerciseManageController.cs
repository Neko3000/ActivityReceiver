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
using ActivityReceiver.ViewModels.ExerciseManage;
using ActivityReceiver.Functions;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ActivityReceiver.DataBuilders;

namespace ActivityReceiver.Controllers
{
    [Authorize]
    public class ExerciseManageController : Controller
    {
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly ExerciseManageDataBuilder _dataBuilder;

        public ExerciseManageController(ActivityReceiverDbContext arDbContext, UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _arDbContext = arDbContext;
            _userManager = userManager;
            _roleManager = roleManager;

            _dataBuilder = new ExerciseManageDataBuilder(_arDbContext, _userManager, _roleManager);
        }

        // GET: ExerciseManage
        [HttpGet]
        public async Task<IActionResult> Index()
        {    
            var vm = new ExerciseManageIndexViewModel
            {
                ExercisePresenterCollection = await _dataBuilder.BuildExercisePresenterList()
            };

            return View(vm);
        }

        // GET: ExerciseManage/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {            
            var questions =  await  _arDbContext.Questions.ToListAsync();

            var vm = new ExerciseManageCreateGetViewModel{
               EntireQuestionCollection = questions
            };
            return View(vm);
        }

        // POST: ExerciseManage/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExerciseManageCreatePostViewModel model)
        {
            if (ModelState.IsValid)
            {
                var exercise = Mapper.Map<ExerciseManageCreatePostViewModel,Exercise>(model);
                _arDbContext.Exercises.Add(exercise);
                await _arDbContext.SaveChangesAsync();

                foreach (var questionID in model.QuestionIDCollection)
                {
                    var question = _arDbContext.Questions.SingleOrDefault(q=>q.ID == questionID);

                    if(question == null)
                    {
                        return NotFound();
                    }

                    var exerciseQuestionRelationship = new ExerciseQuestion
                    {
                        ExerciseID = exercise.ID,
                        QuestionID = question.ID
                    };

                    _arDbContext.ExerciseQuestionCollection.Add(exerciseQuestionRelationship);
                    await _arDbContext.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            //if isValid is false
            var vm = Mapper.Map<ExerciseManageCreatePostViewModel,ExerciseManageCreateGetViewModel>(model);
            vm.EntireQuestionCollection = _arDbContext.Questions.ToList();

            return View(vm);
        }

        // GET: ExerciseManage/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercise = await _arDbContext.Exercises.SingleOrDefaultAsync(m => m.ID == id);

            if (exercise == null)
            {
                return NotFound();
            }

            var vm = Mapper.Map<Exercise,ExerciseManageEditGetViewModel>(exercise);

            var allQuestionsInExercise = (from q in _arDbContext.Questions
                                          join eqc in _arDbContext.ExerciseQuestionCollection on q.ID equals eqc.QuestionID
                                          where eqc.ExerciseID == exercise.ID
                                          orderby eqc.SerialNumber ascending
                                          select q).ToList();

            var selectedQuestionIDCollection = new List<int>();
            foreach (var question in allQuestionsInExercise)
            {
                selectedQuestionIDCollection.Add(question.ID);
            }
            vm.SelectedQuestionIDCollection = selectedQuestionIDCollection;
            vm.EntireQuestionCollection = await _arDbContext.Questions.ToListAsync();

            var applicationUsers = await _userManager.Users.ToListAsync();
            vm.ApplicationUserPresenterCollection = await ApplicationUserHandler.ConvertApplicationUsersToPresenterCollection(_userManager, _roleManager, applicationUsers);

            return View(vm);
        }

        // POST: ItemManage/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ExerciseManageEditPostViewModel model)
        {

            var exercise = _arDbContext.Exercises.Find(model.ID);

            if (exercise == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Mapper.Map<ExerciseManageEditPostViewModel,Exercise>(model, exercise);

                try
                {
                    _arDbContext.Update(exercise);
                    await _arDbContext.SaveChangesAsync();

                    foreach (var questionID in model.SelectedQuestionIDCollection)
                    {
                        var question = _arDbContext.Questions.SingleOrDefault(q => q.ID == questionID);

                        if (question == null)
                        {
                            return NotFound();
                        }

                        var exerciseQuestionRelationship = new ExerciseQuestion
                        {
                            ExerciseID = exercise.ID,
                            QuestionID = question.ID
                        };

                        _arDbContext.ExerciseQuestionCollection.Add(exerciseQuestionRelationship);
                        await _arDbContext.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_arDbContext.Exercises.Any(e=>e.ID == exercise.ID))
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

            var vm = Mapper.Map<ExerciseManageEditPostViewModel,ExerciseManageEditGetViewModel>(model);

            vm.EntireQuestionCollection = await _arDbContext.Questions.ToListAsync();

            var applicationUsers = await _userManager.Users.ToListAsync();
            vm.ApplicationUserPresenterCollection = await ApplicationUserHandler.ConvertApplicationUsersToPresenterCollection(_userManager, _roleManager, applicationUsers);

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

            var exercise = await _arDbContext.Exercises.SingleOrDefaultAsync(e => e.ID == id);

            if (exercise == null)
            {
                return NotFound();
            }

            var vm = Mapper.Map<Exercise, ExerciseManageDetailsViewModel>(exercise);
            vm.EditorName = (await _userManager.FindByIdAsync(exercise.EditorID)).UserName;

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercise = await _arDbContext.Exercises.SingleOrDefaultAsync(e => e.ID == id);

            if (exercise == null)
            {
                return NotFound();
            }

            var vm = Mapper.Map<Exercise, ExerciseManageDeleteGetViewModel>(exercise);
            vm.EditorName = (await _userManager.FindByIdAsync(exercise.EditorID)).UserName;

            return View(vm);
        }

        // POST: ItemManage/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ExerciseManageDeletePostViewModel model)
        {  
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var exercise = await _arDbContext.Exercises.SingleOrDefaultAsync(e => e.ID == model.ID);

            if (exercise == null)
            {
                return NotFound();
            }

            _arDbContext.Exercises.Remove(exercise);

            var deletedExerciseQuestionRelationships =_arDbContext.ExerciseQuestionCollection.Where(eqc => eqc.ExerciseID == exercise.ID).ToList();
            _arDbContext.ExerciseQuestionCollection.RemoveRange(deletedExerciseQuestionRelationships);

            await _arDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}