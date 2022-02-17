using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    public class UserRole : Entity<int>
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }

    }
}
