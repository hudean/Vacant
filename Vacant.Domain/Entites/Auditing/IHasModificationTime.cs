using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    /// <summary>
    /// 有修改时间
    /// </summary>
    public interface IHasModificationTime
    {
        /// <summary>
        /// 此实体的上次修改时间。
        /// </summary>
        DateTime? LastModificationTime { get; set; }
    }
}
