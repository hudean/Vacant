using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    /// <summary>
    /// 点赞表
    /// </summary>
    public class Like:Entity<int>
    {
        /// <summary>
        /// 点赞用户id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 点赞所属类型表的id
        /// </summary>
        public int RelationId { get; set; }
        /// <summary>
        /// 点赞所属类型
        /// </summary>
        public string RelationType { get; set; }
      
        public DateTime CreatedDate { get; set; }
    }
}
