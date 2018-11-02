using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityReceiver.ViewModels
{
    public class GetNextQuestionViewModel
    {
        [Required]
        public int ExerciseID { get; set; }
    }
}
