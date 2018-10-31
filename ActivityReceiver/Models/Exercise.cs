using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ActivityReceiver.Models
{
    public class Exercise
    {
        [Key]
        public int ID { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public int Level { get; set; }

        public DateTime CreateDate { get; set; }
        public string EditorID { get; set; }
    }
}
