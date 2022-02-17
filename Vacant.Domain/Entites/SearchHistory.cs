using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    /// <summary>
    /// 搜索历史记录表
    /// </summary>
    public class SearchHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string SearchKey { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
