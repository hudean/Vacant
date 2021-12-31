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
        public int UserId { get; set; }
        public int RelationId { get; set; }
        public string RelationType { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
