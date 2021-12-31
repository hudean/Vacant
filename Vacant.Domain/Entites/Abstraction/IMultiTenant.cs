using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    public interface IMultiTenant
    {
        /// <summary>
        /// Id of the related tenant.
        /// </summary>
        Guid? TenantId { get; }
    }
}
