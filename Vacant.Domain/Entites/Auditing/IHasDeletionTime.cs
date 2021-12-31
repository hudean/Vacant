using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    /// <summary>
    /// 有删除时间(软删除专属)
    /// </summary>
    public interface IHasDeletionTime : ISoftDelete
    {
        /// <summary>
        /// 此实体的删除时间。
        /// </summary>
        DateTime? DeletionTime { get; set; }
    }
}
