using F_Driver.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F_Driver.DataAccessObject.Models;

namespace F_Driver.Repository.Repositories
{
    public class TransactionRepository : RepositoryBaseAsync<Transaction>, ITripMatchRepository
    {
        public TransactionRepository(DataAccessObject.Models.FDriverContext dbContext) : base(dbContext)
        {
        }
    }
}
