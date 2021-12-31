using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    /// <summary>
    /// 创建审核
    /// </summary>
    public interface ICreationAudited : IHasCreationTime
    {
        /// <summary>
        /// Id of the creator user of this entity.
        /// 创建此实体的用户ID。
        /// </summary>
        Guid? CreatorUserId { get; set; }
    }

    /// <summary>
    /// Adds navigation properties to <see cref="ICreationAudited"/> interface for user.
    /// </summary>
    /// <typeparam name="TUser">Type of the user</typeparam>
    public interface ICreationAudited<TUser> : ICreationAudited
        where TUser : IEntity<Guid>
    {
        /// <summary>
        /// Reference to the creator user of this entity.
        /// </summary>
        TUser CreatorUser { get; set; }
    }
}
