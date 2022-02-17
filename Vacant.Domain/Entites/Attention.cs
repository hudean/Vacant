using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    /// <summary>
    /// 关注表
    /// </summary>
    public class Attention : Entity<int>
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 被关注的用户id
        /// </summary>
        public int FollowedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
