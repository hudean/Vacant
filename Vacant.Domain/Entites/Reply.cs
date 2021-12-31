using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    /// <summary>
    /// 二级回复表
    /// </summary>
    public class Reply : Entity<int>
    {
        /// <summary>
        /// 回复用户id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 回复内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 被回复的id
        /// </summary>
        public int? ToReplyId { get; set; }

        /// <summary>
        /// 被回复的用户id
        /// </summary>
        public int ToRepliedUserId { get; set; }

        #region

        //public int? RelationId { get; set; }

        //public string RelationType { get; set; }

        #endregion

        /// <summary>
        /// 审核id
        /// </summary>
        public int? ReviewId { get; set; }

        /// <summary>
        /// 审核状态  0 待审核 1 审核通过 2 不通过
        /// </summary>
        public int ReviewStatus { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? ReviewDate { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsSetTop { get; set; }

        /// <summary>
        /// 置顶时间
        /// </summary>
        public DateTime? SetTopTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool Removed { get; set; }

        /// <summary>
        /// 回复所属的一级评论Id
        /// </summary>
        public int CommentId { get; set; }


        /// <summary>
        /// 回复类型
        /// comment的话，那么replyId＝commitId，如果reply_type是reply的话，这表示这条回复的父回复。
        /// </summary>
       // public string ReplyType { get; set; }

        /// <summary>
        /// 点赞数量
        /// </summary>
        public int LikeCount { get; set; }
    }
}
