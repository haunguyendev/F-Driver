using F_Driver.DataAccessObject.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.DataAccessObject.Models
{
    public abstract class EntityBase : IEntityBase
    {
        [Key]
        public int Id { get; set; }
    }
}
