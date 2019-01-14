using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Models;

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

        public string SentenceEN { get; set; }
        public string SentenceJP { get; set; }
        public string Division { get; set; }
        public string StandardAnswerDivision { get; set; }

        public int CurrentNumber { get; set; }
    }

    // SubmitQuestionAnswer
    public class SubmitQuestionAnswerPostViewModel
    {
        [Required]
        public int AssignmentRecordID { get; set; }

        [Required]
        public string SentenceEN { get; set; }
        [Required]
        public string SentenceJP { get; set; }
        [Required]
        public string Division { get; set; }
        [Required]
        public string StandardAnswerDivision { get; set; }
        [Required]
        public string Resolution { get; set; }

        [Required]
        public string AnswerDivision { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }

        public IList<Movement> MovementCollection{ get; set; }
        public IList<DeviceAcceleration> DeviceAccelerationCollection { get; set; }
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
