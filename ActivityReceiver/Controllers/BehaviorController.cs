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
    [Route("behavior")]
    public class BehaviorController : Controller
    {
        private ActivityReceiverDbContext _arDbContext;

        public BehaviorController(ActivityReceiverDbContext arDbContext)
        {
            _arDbContext = arDbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var movements = _arDbContext.Movements.ToList();

            return Ok(movements);
        }
    }
}