using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class ErrorRequest
    {
        public int UserId { get; set; }
        public List<int> ErrorCodes { get; set; } = null!;
    }
}
