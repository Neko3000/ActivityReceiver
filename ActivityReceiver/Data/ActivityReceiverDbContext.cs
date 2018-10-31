using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ActivityReceiver.Models;

namespace ActivityReceiver.Data
{
    public class ActivityReceiverDbContext:DbContext
    {
        public ActivityReceiverDbContext(DbContextOptions<ActivityReceiverDbContext> options):base(options)
        {

        }

        public DbSet<Movement> Movements { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<AssignmentRecord> AssignmentRecords { get; set; }
        public DbSet<Answer> Answsers { get; set; }

        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<ExerciseQuestion> ExerciseQuestionCollection { get; set; }
    }
}
