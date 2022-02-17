using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    /// <summary>
    /// 课程目录表
    /// </summary>
    public class CourseCatalog
    {
        public int Id { get; set; }
        /// <summary>
        /// 视频标题
        /// </summary>
        public string VideoTitle { get; set; }
        public string Vid { get; set; }
        /// <summary>
        /// 视频url地址
        /// </summary>
        public string VideoUrl { get; set; }

        /// <summary>
        /// 视频长度
        /// </summary>
        public int VideoLength { get; set; }

        /// <summary>
        /// 所属课程id
        /// </summary>
        public int CourseId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}
