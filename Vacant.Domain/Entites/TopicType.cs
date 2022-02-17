using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    /// <summary>
    /// 话题类型表
    /// </summary>
    public class TopicType
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public int SortNo { get; set; }
    }
}
