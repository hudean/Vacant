using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain.Entites
{
    public class Entity : IEntity
    {
    }

    public class Entity<TKey> : Entity, IEntity<TKey>
    {
        [Key]
        public virtual TKey Id { get; set; }
    }
}
