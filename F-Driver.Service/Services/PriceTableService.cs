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
    public class PriceTableService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PriceTableService(IUnitOfWork unitOfWork, IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //Create price table
        public async Task<bool> CreatePriceTable(PriceTableModel priceTableModel)
        {
            var priceTableExist = await _unitOfWork.PriceTables.FindByCondition(p => p.FromZoneId == priceTableModel.FromZoneId && p.ToZoneId == priceTableModel.ToZoneId).FirstOrDefaultAsync();
            if (priceTableExist != null)
            {
                return false;
            }
            var priceTable = _mapper.Map<PriceTable>(priceTableModel);
            await _unitOfWork.PriceTables.CreateAsync(priceTable);
            var rs = await _unitOfWork.CommitAsync();
            if (rs > 0)
            {
                return true;
            }
            return false;

        }

        //Update price table

        public async Task<PriceTableModel?> UpdatePriceTable(int priceTableId, PriceTableModel priceTableModel)
        {
            var priceTableExist = await _unitOfWork.PriceTables.FindByCondition(p => p.FromZoneId == priceTableModel.FromZoneId && p.ToZoneId == priceTableModel.ToZoneId && p.Id != priceTableId).FirstOrDefaultAsync();
            if (priceTableExist != null)
            {
                return null;
            }
            var priceTable = await _unitOfWork.PriceTables.GetByIdAsync(priceTableId);
            if (priceTable == null)
            {
                return null;
            }
            priceTableModel.Id = priceTableId;
            await _unitOfWork.PriceTables.UpdateAsync(_mapper.Map<PriceTable>(priceTableModel));
            await _unitOfWork.CommitAsync();
            return priceTableModel;
        }

        //Get by id
        public async Task<PriceTableModel?> GetPriceTableById(int priceTableId)
        {
            var priceTable = await _unitOfWork.PriceTables.GetByIdAsync(priceTableId);
            if (priceTable == null)
            {
                return null;
            }
            return _mapper.Map<PriceTableModel>(priceTable);
        }

        //Get price table by ZoneFrom and ZoneTo
        public async Task<PriceTableModel?> GetPriceTableByZoneFromAndZoneTo(int zoneFromId, int zoneToId)
        {
            var priceTable = await _unitOfWork.PriceTables.FindByCondition(z => z.FromZoneId == zoneFromId && z.ToZoneId == zoneToId).FirstOrDefaultAsync();
            if (priceTable == null)
            {
                return null;
            }
            return _mapper.Map<PriceTableModel>(priceTable);
        }
    }
}
