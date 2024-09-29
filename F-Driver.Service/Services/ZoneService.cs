using AutoMapper;
using F_Driver.DataAccessObject.Models;
using F_Driver.Repository.Interfaces;
using F_Driver.Service.BusinessModels;
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
        public async Task<ZoneModel> UpdateZone(int zoneId ,ZoneModel zoneModel)
        {
            var zone = _unitOfWork.Zones.GetByIdAsync(zoneId);
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


    }
}
