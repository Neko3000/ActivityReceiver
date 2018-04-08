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
    }
}
