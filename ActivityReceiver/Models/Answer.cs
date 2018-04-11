using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ActivityReceiver.Models
{
    public class Answer
    {
        [Key]
        public int ID { get; set; }
        public string Content { get; set; }
        public int? HesitationDegree { get; set; }
    }
}
