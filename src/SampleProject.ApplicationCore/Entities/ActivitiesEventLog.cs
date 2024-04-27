using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleProject.ApplicationCore.Entities
{
    public class ActivitiesEventLog : BaseEntity
    {
        public ActivitiesEventLog()
        {
            CreatedDatetime = DateTime.UtcNow;
            UpdatedDatetime = DateTime.UtcNow;
        }

        public string? ActivityType { get; set; }
        public string? Description { get; set; }
        public string? UserId { get; set; }
    }
}
