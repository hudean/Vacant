using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Model.ParamModel
{
    public class ParamPage
    {
        public virtual int PageIndex { get; set; } = 1;
        public virtual int PageSize { get; set; } = 10;
    }
}
