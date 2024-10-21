using AutoMapper;
using F_Driver.DataAccessObject.Models;
using F_Driver.Repository.Interfaces;
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
    public class ZoneService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ZoneService(IUnitOfWork unitOfWork, IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        
        //Create zone
        public async Task<bool> CreateZone(ZoneModel zoneModel)
        {
            var existingZone = await _unitOfWork.Zones.FindAllAsync(zone => zone.ZoneName == zoneModel.ZoneName);
            if (existingZone.Count()> 0)
            {
                return false;
            }
            var zone = _mapper.Map<Zone>(zoneModel);
            await _unitOfWork.Zones.CreateAsync(zone);
            var rs = await _unitOfWork.CommitAsync();
            if(rs > 0)
            {
                return true;
            }
            return false;
        }

        //Update zone
        public async Task<ZoneModel?> UpdateZone(int zoneId ,ZoneModel zoneModel)
        {
            var existingZone = await _unitOfWork.Zones
                    .FindByCondition(z => z.ZoneName == zoneModel.ZoneName && z.Id != zoneId).FirstOrDefaultAsync();

            if (existingZone != null)
            {
                return null;
            }
            var zone = await _unitOfWork.Zones.GetByIdAsync(zoneId);
            if (zone == null)
            {
                return null;
            }
            zoneModel.Id = zoneId;
            await _unitOfWork.Zones.UpdateAsync(_mapper.Map<Zone>(zoneModel));
            await _unitOfWork.CommitAsync();
            return zoneModel;
        }

        //Get by id
        public async Task<ZoneModel?> GetZoneById(int zoneId)
        {
            var zone = await _unitOfWork.Zones.GetByIdAsync(zoneId);
            if (zone == null)
            {
                return null;
            }
            return _mapper.Map<ZoneModel>(zone);
        }


        //Get list zone
        public async Task<IEnumerable<ZoneModel>> GetListZone()
        {
            var zones = await _unitOfWork.Zones.FindAllAsync(zone => true);
            return _mapper.Map<IEnumerable<ZoneModel>>(zones);
        }

        //get list zone by from zone id or to zone id in price table
        public async Task<List<ZoneModel>> GetListZoneByFromZoneIdOrToZoneId(int? fromZoneId, int? toZoneId)
        {
            if (fromZoneId == null && toZoneId == null)
            {
                return new List<ZoneModel>();
            }
            if (fromZoneId == null)
            {
                var priceTables = await _unitOfWork.PriceTables.FindByCondition(p => p.ToZoneId == toZoneId).ToListAsync();
                var zones = new List<Zone>();
                foreach (var priceTable in priceTables)
                {
                    var zone = await _unitOfWork.Zones.GetByIdAsync(priceTable.FromZoneId);
                    if (zone != null)
                    {
                        zones.Add(zone);
                    }
                }
                return _mapper.Map<List<ZoneModel>>(zones);
            }
            if (toZoneId == null)
            {
                var priceTables = await _unitOfWork.PriceTables.FindByCondition(p => p.FromZoneId == fromZoneId).ToListAsync();
                var zones = new List<Zone>();
                foreach (var priceTable in priceTables)
                {
                    var zone = await _unitOfWork.Zones.GetByIdAsync(priceTable.ToZoneId);
                    if (zone != null)
                    {
                        zones.Add(zone);
                    }
                }
                return _mapper.Map<List<ZoneModel>>(zones);
            }
            return new List<ZoneModel>();
        }

        public async Task<PaginatedList<ZoneModel>> GetAllZonesAsync(ZoneQueryParameters filterRequest)
        {
            // Query the zones
            var query = _unitOfWork.Zones.FindAll(false,
                z => z.PriceTableFromZones,
                z => z.PriceTableToZones,
                z => z.TripRequestFromZones,
                z => z.TripRequestToZones
            );

            // Apply filtering by ZoneName (if provided)
            if (!string.IsNullOrEmpty(filterRequest.ZoneName))
            {
                query = query.Where(z => z.ZoneName.Contains(filterRequest.ZoneName));
            }

            // Apply filtering by Description (if provided)
            if (!string.IsNullOrEmpty(filterRequest.Description))
            {
                query = query.Where(z => z.Description.Contains(filterRequest.Description));
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(filterRequest.SortBy))
            {
                switch (filterRequest.SortBy.ToLower())
                {
                    case "zonename":
                        query = filterRequest.IsAscending ? query.OrderBy(z => z.ZoneName) : query.OrderByDescending(z => z.ZoneName);
                        break;
                    case "description":
                        query = filterRequest.IsAscending ? query.OrderBy(z => z.Description) : query.OrderByDescending(z => z.Description);
                        break;
                    default:
                        query = filterRequest.IsAscending ? query.OrderBy(z => z.ZoneName) : query.OrderByDescending(z => z.ZoneName);
                        break;
                }
            }

            // Apply pagination
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((filterRequest.Page - 1) * filterRequest.PageSize)
                .Take(filterRequest.PageSize)
                .ToListAsync();

            var zoneModels = _mapper.Map<List<ZoneModel>>(items);

            return new PaginatedList<ZoneModel>(zoneModels, totalCount, filterRequest.Page, filterRequest.PageSize);
        }

        public async Task<bool> DeleteZoneAsync(int zoneId)
        {
            var zone = await _unitOfWork.Zones.GetByIdAsync(zoneId);
            if (zone == null)
            {
                return false; // Category not found
            }
           

            // Update category status tocategory.Status = "Deleted";
            await _unitOfWork.Zones.DeleteAsync(zone);
            await _unitOfWork.CommitAsync();

            return true;
        }

    }
}
