using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    public interface IFullAudited : IAudited, IDeletionAudited
    {

    }

    /// <summary>
    /// Adds navigation properties to <see cref="IAudited"/> interface for user.
    /// </summary>
    /// <typeparam name="TUser">Type of the user</typeparam>
    public interface IFullAudited<TUser> : IAudited<TUser>, IDeletionAudited<TUser>
        where TUser : IEntity<Guid>
    {

    }
}
