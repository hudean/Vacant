using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    public class CourseProgress
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int LearningTimes { get; set; }
        public int CourseCatalogId { get; set; }
        public int LearningStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
