using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Model.Dto
{
    public class RoleDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(32)]
        public string RoleName { get; set; }
        [MaxLength(255)]
        public string Description { get; set; }
    }
}
