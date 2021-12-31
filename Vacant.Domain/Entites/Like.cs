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
        public int RelationId { get; set; }
        public string RelationType { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
