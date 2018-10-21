using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ActivityReceiver.Data;
using ActivityReceiver.Models;

namespace ActivityReceiver.Controllers
{
    [Produces("application/json")]
    [Route("question")]
    public class QuestionController : Controller
    {
        private ActivityReceiverDbContext _arDbContext;

        public QuestionController(ActivityReceiverDbContext arDbContext)
        {
            _arDbContext = arDbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var questions = _arDbContext.Questions.ToList();

            return Ok(questions);
        }
    }
}