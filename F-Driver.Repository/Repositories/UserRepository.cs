using F_Driver.DataAccessObject.Models;
using F_Driver.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Repository.Repositories
{
    public class UserRepository : RepositoryBaseAsync<User>, IUserRepository
    {
        public UserRepository(FDriverContext dbContext) : base(dbContext)
        {
        }

        public async Task<User> GetDriverById(int id)
        {
            return await _dbContext.Users.Include(x => x.Driver).ThenInclude(x => x.Vehicles)
                 .Include(x => x.Wallet).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User> GetPassengerById(int id)
        {
            return await _dbContext.Users.Include(x => x.Wallet).FirstOrDefaultAsync(x => x.Id == id);
           
        }
        
    }
}
