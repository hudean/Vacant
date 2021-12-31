using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    /// <summary>
    /// 有创建时间
    /// </summary>
    public interface IHasCreationTime
    {
        /// <summary>
        /// 此实体的创建时间。
        /// </summary>
        DateTime CreationTime { get; set; }
    }
}
