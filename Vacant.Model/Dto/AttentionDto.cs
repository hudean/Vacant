using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Model.Dto
{
    public class AttentionDto
    {
        public  int Id { get; set; }

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
