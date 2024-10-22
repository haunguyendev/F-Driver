using AutoMapper;
using F_Driver.DataAccessObject.Models;
using F_Driver.Repository.Interfaces;
using F_Driver.Repository.Repositories;
using F_Driver.Service.BusinessModels;
using F_Driver.Service.BusinessModels.QueryParameters;
using F_Driver.Service.Helpers;
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

        //delete price table by id without using
        /// <summary>
        /// Chưa tính tới trường hơp có trip request mà chưa có giá ở đây, hoặc có rồi mà xóa giá
        /// </summary>
        /// <param name="priceTableId"></param>
        /// <returns></returns>
        public async Task<bool> DeletePriceTable(int priceTableId)
        {
            var priceTable = await _unitOfWork.PriceTables.GetByIdAsync(priceTableId);
            if (priceTable == null)
            {
                return false;
            }
            await _unitOfWork.PriceTables.DeleteAsync(priceTable);
            var rs = await _unitOfWork.CommitAsync();
            if (rs > 0)
            {
                return true;
            }
            return false;
        }

        #region get all filter
        public async Task<PaginatedList<PriceTableModel>> GetAllPriceTablesAsync(PriceTableQueryParams parameters)
        {
            // Query cơ bản
            var query = _unitOfWork.PriceTables.FindAll();

            // Lọc theo FromZoneId
            if (parameters.FromZoneId.HasValue)
            {
                query = query.Where(pt => pt.FromZoneId == parameters.FromZoneId.Value);
            }

            // Lọc theo ToZoneId
            if (parameters.ToZoneId.HasValue)
            {
                query = query.Where(pt => pt.ToZoneId == parameters.ToZoneId.Value);
            }

            // Lọc theo giá tối thiểu và tối đa
            if (parameters.MinUnitPrice.HasValue)
            {
                query = query.Where(pt => pt.UnitPrice >= parameters.MinUnitPrice.Value);
            }

            if (parameters.MaxUnitPrice.HasValue)
            {
                query = query.Where(pt => pt.UnitPrice <= parameters.MaxUnitPrice.Value);
            }

            // Sắp xếp theo trường và thứ tự
            if (!string.IsNullOrWhiteSpace(parameters.Sort))
            {
                var sortBy = parameters.Sort;
                var sortOrder = parameters.SortOrder?.ToLower() == "desc" ? "desc" : "asc";

                query = sortOrder == "desc" ? query.OrderByDescending(pt => EF.Property<object>(pt, sortBy))
                                            : query.OrderBy(pt => EF.Property<object>(pt, sortBy));
            }
            else
            {
                // Sắp xếp mặc định theo FromZoneId và ToZoneId nếu không có tham số Sort
                query = query.OrderBy(pt => pt.FromZoneId).ThenBy(pt => pt.ToZoneId);
            }

            // Tính toán tổng số bảng giá
            var totalCount = await query.CountAsync();

            // Phân trang
            var priceTables = await query.Skip((parameters.Page - 1) * parameters.PageSize)
                                         .Take(parameters.PageSize)
                                         .ToListAsync();

            // Map PriceTable entity sang PriceTableResponseModel
            var priceTableDtos = _mapper.Map<List<PriceTableModel>>(priceTables);

            // Trả về kết quả phân trang
            return new PaginatedList<PriceTableModel>(priceTableDtos, totalCount, parameters.Page, parameters.PageSize);
        }

        #endregion
    }
}
