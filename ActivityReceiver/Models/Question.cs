using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ActivityReceiver.Models
{
    public class Question
    {
        [Key]
        public int ID { get; set; }

        public string SentenceEN { get; set; }
        public string SentenceJP { get; set; }

        public int Level { get; set; }
        public string GrammarIDString { get; set; }

        public string Division { get; set; }
        public string StandardAnswerDivision { get; set; }
        public string Remark { get; set; }

        public DateTime CreateDate { get; set; }
        public string EditorID { get; set; }
    }
}
