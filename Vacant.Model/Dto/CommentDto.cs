using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Model.Dto
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }


        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string HeadImg { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 回复次数
        /// </summary>
        public int ReplyCount { get; set; }
        /// <summary>
        /// 点赞次数
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 是否喜欢
        /// </summary>
        public bool IsLike { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
