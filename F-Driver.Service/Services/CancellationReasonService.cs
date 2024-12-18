﻿using AutoMapper;
using F_Driver.DataAccessObject.Models;
using F_Driver.Repository.Interfaces;
using F_Driver.Service.BusinessModels;
﻿using F_Driver.DataAccessObject.Models;
using F_Driver.Repository;
using F_Driver.Repository.Interfaces;
using F_Driver.Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.Services
{
    public class CancellationReasonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CancellationReasonService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CancellationReasonModel> GetCancellationReason(int id)
        {
            var cancellationReason = await _unitOfWork.CancellationReasons.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return _mapper.Map<CancellationReasonModel>(cancellationReason);
        }
        public async Task<List<CancellationReason>> GetAllCancellationReasonsAsync()
        {
            return await _unitOfWork.CancellationReasons.FindAll().ToListAsync();
        }
        public async Task CreateCancellationReasonAsync(CancellationReason cancellationReason)
        {
            await _unitOfWork.CancellationReasons.CreateAsync(cancellationReason);
            await _unitOfWork.CommitAsync();
        }
        public async Task UpdateCancellationReasonAsync(CancellationReason cancellationReason)
        {
            await _unitOfWork.CancellationReasons.UpdateAsync(cancellationReason);
            await _unitOfWork.CommitAsync();
        }
        public async Task<CancellationReason?> GetCancellationReasonByIdAsync(int id)
        {
            return await _unitOfWork.CancellationReasons.FindAsync(cr=>cr.Id==id);
        }
        public async Task DeleteCancellationReasonAsync(int id)
        {
            var cancellationReason = await _unitOfWork.CancellationReasons.FindAsync(cr => cr.Id == id);
            if (cancellationReason != null)
            {
                await _unitOfWork.CancellationReasons.DeleteAsync(cancellationReason);
                await _unitOfWork.CommitAsync();
            }
        }
    }
}
