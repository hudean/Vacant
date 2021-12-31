using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    /// <summary>
    /// This interface is used to make an entity active/passive.
    /// 此接口用于使实体启用/禁用。
    /// </summary>
    public interface IPassivable
    {
        /// <summary>
        /// True: This entity is active.
        /// False: This entity is not active.
        /// True: 该实体处于启用状态。
        /// False: 该实体处于禁用状态。
        /// </summary>
        bool IsActive { get; set; }
    }
}
