using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain
{
    public interface IAuditPropertySetter
    {
        void SetCreationProperties(object targetObject);

        void SetModificationProperties(object targetObject);

        void SetDeletionProperties(object targetObject);
    }
}
