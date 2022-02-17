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
    /// 用户表
    /// </summary>
    [Table("Role")]
    public class User : Entity<int>
    {
        [Required]
        [MaxLength(32)]
        public string LoginName { get; set; }
        [Required]
        [MaxLength(32)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(32)]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        [MaxLength(32)]
        public string Mobile { get; set; }

        [Required]
        [MaxLength(32)]
        [EmailAddress]
        public string Email { get; set; }

        public DateTime CreateData { get; set; }

        public DateTime LastLoginTime { get; set; }

        public int LoginErrorCount { get; set; }


    }


}
