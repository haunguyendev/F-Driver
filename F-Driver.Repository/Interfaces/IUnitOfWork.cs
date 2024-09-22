using F_Driver.DataAccessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
      
        FDriverContext GetDbContext();

        Task<int> CommitAsync();
        Task BeginTransactionAsync();
        Task RollbackAsync();
    }
}
