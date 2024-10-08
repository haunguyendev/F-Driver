﻿using F_Driver.DataAccessObject.Models;
using F_Driver.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Repository.Repositories
{
    public class CancellationRepository : RepositoryBaseAsync<Cancellation>, ICancellationRepository
    {
        public CancellationRepository(FDriverContext dbContext) : base(dbContext)
        {
        }
    }
}
