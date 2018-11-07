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

    public class MovementDTO
    {
        [Required]
        public int Index { get; set; }
        [Required]
        public int State { get; set; }
        [Required]
        public int Time { get; set; }
        [Required]
        public int XPosition { get; set; }
        [Required]
        public int YPosition { get; set; }
    }

    public class SubmitQuestionAnswerViewModel
    {
        [Required]
        public int ExerciseID { get; set; }
        [Required]
        public int QuestionID { get; set; }
        [Required]
        public string Answer { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }

        public IList<MovementDTO> MovementDTOs { get; set; }
    }

    public class GetAssignmentResultViewModel
    {
        [Required]
        public int ExerciseID { get; set; }
    }
}
