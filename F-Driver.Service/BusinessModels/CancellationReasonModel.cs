using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class CancellationReasonModel
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
    }
}
