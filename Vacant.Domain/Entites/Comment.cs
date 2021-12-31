using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    /// <summary>
    /// 一级评论表
    /// </summary>
    public class Comment : Entity<int>
    {
        /// <summary>
        /// 评论用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 关联ID
        /// </summary>
        public int RelationId { get; set; }

        /// <summary>
        /// 关联类型
        /// </summary>
        public string RelationType { get; set; }

        /// <summary>
        /// 审批人ID
        /// </summary>
        public int? ReviewId { get; set; }

        /// <summary>
        /// 审批状态 0 待审核 1 审核通过 2 不通过
        /// </summary>
        public int ReviewStatus { get; set; }

        /// <summary>
        /// 审批日期
        /// </summary>
        public DateTime? ReviewDate { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool? IsSetTop { get; set; }

        /// <summary>
        /// 置顶时间
        /// </summary>
        public DateTime? SetTopTime { get; set; }


        /// <summary>
        /// 点赞数量
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 回复数量
        /// </summary>
        public int ReplyCount { get; set; }


        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool Removed { get; set; }

    }
}
