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
    public class CancellationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CancellationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<CancellationModel>> GetCancellations()
        {
            var cancellations = await _unitOfWork.Cancellations.FindAll().ToListAsync();
            return _mapper.Map<List<CancellationModel>>(cancellations);
        }

        public async Task<CancellationModel> GetCancellation(int id)
        {
            var cancellation = await _unitOfWork.Cancellations.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return _mapper.Map<CancellationModel>(cancellation);
        }

        public async Task<CancellationModel> CreateCancellation(CancellationModel cancellationModel)
        {
            var cancellation = _mapper.Map<Cancellation>(cancellationModel);
            await _unitOfWork.Cancellations.CreateAsync(cancellation);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<CancellationModel>(cancellation);
        }

        public async Task<CancellationModel?> UpdateCancellation(int id, CancellationModel cancellationModel)
        {
            var cancellation = await _unitOfWork.Cancellations.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            if (cancellation == null)
            {
                return null;
            }

            _mapper.Map(cancellationModel, cancellation);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<CancellationModel>(cancellation);
        }

        public async Task<bool> DeleteCancellation(int id)
        {
            var cancellation = await _unitOfWork.Cancellations.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            if (cancellation == null)
            {
                return false;
            }

            await _unitOfWork.Cancellations.DeleteAsync(cancellation);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
