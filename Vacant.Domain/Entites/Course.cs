using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    /// <summary>
    /// 课程表
    /// </summary>
    public class Course
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        /// <summary>
        /// 课程名称
        /// </summary>
        public string CourseName { get; set; }
        public int CourseTypeId { get; set; }
        public string PicUrl { get; set; }
        public string Introduction { get; set; }
        public decimal Price { get; set; }
        public decimal CourseStar { get; set; }
        public int LearningCount { get; set; }
        public int ReplyCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// 课程分类 0默认的课程1、最新（前沿）技术  2、学术培训
        /// </summary>
        public int CourseCategory { get; set; }

        /// <summary>
        /// 点赞次数
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 视频总时长
        /// </summary>
        public int TotalVideoDuration { get; set; }
    }
}
