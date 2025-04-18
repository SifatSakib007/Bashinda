using AutoMapper;
using BashindaAPI.Data;
using BashindaAPI.DTOs;
using BashindaAPI.Models;

namespace BashindaAPI.Services
{
    public interface IHouseService
    {
        //Task<(bool success, IEnumerable<HouseDTO> houses, string[] errors)> GetAllHousesAsync();
        //Task<(bool success, HouseDTO house, string[] errors)> GetHouseByIdAsync(int id);
        Task<(bool success, HouseDTO house, string[] errors)> CreateHouseAsync(CreateHouseDto model, int userId);
        //Task<(bool success, HouseDTO house, string[] errors)> UpdateHouseAsync(int id, UpdateHouseDto model);
        //Task<(bool success, string[] errors)> DeleteHouseAsync(int id);
    }

    public class HouseService : IHouseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public HouseService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<(bool success, HouseDTO? house, string[] errors)> CreateHouseAsync(CreateHouseDto model, int userId)
        {
            try
            {
                var house = _mapper.Map<House>(model);

                house.OwnerId = userId;

                _context.Houses.Add(house);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    var houseDto = _mapper.Map<HouseDTO>(house);
                    return (true, houseDto, Array.Empty<string>());
                }
                else
                {
                    return (false, null, new[] { "Failed to create house." });
                }
            }
            catch
            {
                return (false, null, new[] { "An error occurred while creating the house." });
            }
            
        }
    }
}
