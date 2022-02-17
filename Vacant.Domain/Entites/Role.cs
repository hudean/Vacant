using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    /// <summary>
    /// 角色表
    /// </summary>
    [Table("Role")]
    public class Role : Entity<int>
    {
        [Required]
        [MaxLength(32)]
        public string RoleName { get; set; }
        [MaxLength(255)]
        public string Description { get; set; }


    }
}
