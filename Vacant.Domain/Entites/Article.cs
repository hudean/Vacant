using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    /// <summary>
    /// 文章表
    /// </summary>
    public class Article:Entity<int>
    {

        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 文章内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 文章缩略图
        /// </summary>
        public string PicUrl { get; set; }

        /// <summary>
        /// 话题类型id 非必填 left join
        /// </summary>
        public int? TopicTypeId { get; set; }

        /// <summary>
        /// 文章类型
        /// </summary>
        public int ArticleTypeId { get; set; }

        /// <summary>
        /// 发布用户id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 阅读数量
        /// </summary>
        public int ReadCount { get; set; }

        /// <summary>
        /// 回复数量
        /// </summary>
        public int ReplyCount { get; set; }

        /// <summary>
        /// 点赞数量
        /// </summary>
        public int LikeCount { get; set; }
        /// <summary>
        /// 分享次数
        /// </summary>
        public int ShareCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //public int ArticleCategory { get; set; }


        /// <summary>
        /// 审批人ID
        /// </summary>
        public int? ReviewId { get; set; }

        /// <summary>
        /// 审批状态 0 待审核 1 审核通过 2 不通过
        /// </summary>
        public int ReviewStatus { get; set; }

        /// <summary>
        /// 审核失败原因
        /// </summary>
        public string ReasonFailure { get; set; }

        /// <summary>
        /// 审批日期
        /// </summary>
        public DateTime? ReviewDate { get; set; }
    }
}
