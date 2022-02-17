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
    /// 权限表
    /// </summary>
    [Table("Permission")]
    public class Permission:Entity<int>
    {
        [Required]
        [MaxLength(32)]
        public string PermissionName { get; set; }
        [MaxLength(255)]
        public string Description { get; set; }
    }
}
