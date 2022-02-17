using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    /// <summary>
    /// 收藏表
    /// </summary>
    public class Collection : Entity<int>
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 收藏类型表的id
        /// </summary>
        public int RelationId { get; set; }

        /// <summary>
        /// 收藏类型
        /// </summary>
        public string RelationType { get; set; }


        public DateTime CreatedDate { get; set; }
    }
}
