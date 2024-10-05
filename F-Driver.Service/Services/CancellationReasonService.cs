using AutoMapper;
using F_Driver.DataAccessObject.Models;
using F_Driver.Repository.Interfaces;
using F_Driver.Service.BusinessModels;
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

        public async Task<List<CancellationReasonModel>> GetCancellationReasons()
        {
            var cancellationReasons = await _unitOfWork.CancellationReasons.FindAll().ToListAsync();
            return _mapper.Map<List<CancellationReasonModel>>(cancellationReasons);
        }

        public async Task<CancellationReasonModel> GetCancellationReason(int id)
        {
            var cancellationReason = await _unitOfWork.CancellationReasons.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return _mapper.Map<CancellationReasonModel>(cancellationReason);
        }

        public async Task<CancellationReasonModel> CreateCancellationReason(CancellationReasonModel cancellationReasonModel)
        {
            var cancellationReason = _mapper.Map<CancellationReason>(cancellationReasonModel);
            await _unitOfWork.CancellationReasons.CreateAsync(cancellationReason);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<CancellationReasonModel>(cancellationReason);
        }

        public async Task<CancellationReasonModel?> UpdateCancellationReason(int id, CancellationReasonModel cancellationReasonModel)
        {
            var cancellationReason = await _unitOfWork.CancellationReasons.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            if (cancellationReason == null)
            {
                return null;
            }

            _mapper.Map(cancellationReasonModel, cancellationReason);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<CancellationReasonModel>(cancellationReason);
        }

        public async Task<bool> DeleteCancellationReason(int id)
        {
            var cancellationReason = await _unitOfWork.CancellationReasons.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            if (cancellationReason == null)
            {
                return false;
            }
            var cancellation = await _unitOfWork.Cancellations.FindByCondition(x => x.ReasonId == id).FirstOrDefaultAsync();
            if (cancellation != null)
            {
                return false;
            }
            await _unitOfWork.CancellationReasons.DeleteAsync(cancellationReason);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
