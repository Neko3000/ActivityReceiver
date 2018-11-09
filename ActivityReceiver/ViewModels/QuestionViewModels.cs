using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityReceiver.ViewModels
{
    // GetExerciseList
    public class ExerciseDetail
    {
        public int ID { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public int CurrentNumber { get; set; }
        public int TotalNumber { get; set; }

        public bool IsFinished { get; set; }
    }

    public class GetExerciseListGetViewModel
    {
        public IList<ExerciseDetail> ExerciseDetails { get; set; }
    }

    // GetNextQuestion
    public class GetNextQuestionPostViewModel
    {
        [Required]
        public int ExerciseID { get; set; }
    }

    public class GetNextQuestionGetViewModel
    {
        public int AssignmentRecordID { get; set; }
        public int QuestionID { get; set; }

        public string SentenceJP { get; set; }
        public string Division { get; set; }
    }

    // SubmitQuestionAnswer
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

    public class SubmitQuestionAnswerPostViewModel
    {
        [Required]
        public int AssignmentRecordID { get; set; }
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

    // GetAssignmentResult
    public class GetAssignmentResultPostViewModel
    {
        [Required]
        public int ExerciseID { get; set; }
    }
    public class AnswerDetail
    {
        public string SentenceJP { get; set; }
        public string SentenceEN { get; set; }

        public string AnswerSentence { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class GetAssignmentResultGetViewModel
    {
        public float AccuracyRate { get; set; }

        public IList<AnswerDetail> AnswerDetails { get; set; }
    }
}
