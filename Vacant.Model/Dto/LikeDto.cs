using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Model.Dto
{
    public class LikeDto
    {
        public int Id { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }


        public int RelationId { get; set; }

        public string RelationType { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
