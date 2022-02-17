using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Model.ParamModel
{
    public class ParamAttention : ParamPage
    {
        public int UserId { get; set; }
    }


    public class ParamAttentionAdd 
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 被关注的用户id
        /// </summary>
        public int FollowedUserId { get; set; }
    }
}
