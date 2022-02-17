using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Model.ParamModel
{
    public class ParamLike : ParamPage
    {
        public int UserId { get; set; }

    }


    public class ParamLikeAdd
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }

      
        public int RelationId { get; set; }

        public string RelationType { get; set; }

    }
}
