using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    /// <summary>
    /// 文章类型
    /// </summary>
    public class ArticleType:Entity<int>
    {
        public string TypeName { get; set; }
        public int SortNo { get; set; }

    }
}
