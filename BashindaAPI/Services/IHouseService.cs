using AutoMapper;
using BashindaAPI.Data;
using BashindaAPI.DTOs;
using BashindaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BashindaAPI.Services
{
    public interface IHouseService
    {
        //Task<(bool success, IEnumerable<HouseDTO> houses, string[] errors)> GetAllHousesAsync();
        Task<(bool success, HouseDTO? house, string[] errors)> GetHouseByIdAsync(int id);
        Task<(bool success, HouseDTO? house, string[] errors)> CreateHouseAsync(CreateHouseDto model, int userId);
        //Task<(bool success, HouseDTO house, string[] errors)> UpdateHouseAsync(int id, UpdateHouseDto model);
        //Task<(bool success, string[] errors)> DeleteHouseAsync(int id);
    }

    public class HouseService : IHouseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<HouseService> _logger;

        public HouseService(ApplicationDbContext context, IMapper mapper, ILogger<HouseService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(bool success, HouseDTO? house, string[] errors)> CreateHouseAsync(CreateHouseDto model, int userId)
        {
            try
            {
                //create a new house
                var house = new House
                {
                    Name = model.Name,
                    Description = model.Description,
                    NumberOfFloors = model.NumberOfFloors,
                    ApartmentsPerFloor = model.ApartmentsPerFloor,
                    CreatedAt = model.CreatedAt,
                    ImagePath = model.ImagePath,
                    DivisionId = model.DivisionId,
                    DistrictId = model.DistrictId,
                    UpazilaId = model.UpazilaId,
                    AreaType = model.AreaType,
                    WardId = model.WardId,
                    VillageId = model.VillageId,
                    OwnerId = userId
                };
                _logger.LogInformation($"Creating house: {house.Name}, Owner ID: {userId}");
                _context.Houses.Add(house);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    // Reload the house with relationships
                    var createdHouse = await _context.Houses
                        .Include(h => h.Division)
                        .Include(h => h.District)
                        .Include(h => h.Upazila)
                        .Include(h => h.Ward)
                        .Include(h => h.Village)
                        .Include(h => h.Owner)
                        .FirstOrDefaultAsync(h => h.Id == house.Id);
                    if (createdHouse == null)
                    {
                        _logger.LogError($"House not found after creation: {house.Id}");
                        return (false, null, new[] { "House not found after creation." });

                    }
                        _logger.LogInformation($"House created successfully: {house.Id}");
                    // Map the house to HouseDTO
                    _logger.LogInformation($"Mapping house to DTO: {house.Id}");
                    var houseDto = new HouseDTO
                    {
                        Id = createdHouse.Id,
                        Name = createdHouse.Name,
                        Description = createdHouse.Description,
                        ImagePath = createdHouse.ImagePath,
                        NumberOfFloors = createdHouse.NumberOfFloors,
                        ApartmentsPerFloor = createdHouse.ApartmentsPerFloor,
                        Division = createdHouse.Division.Name,
                        District = createdHouse.District.Name,
                        Upazila = createdHouse.Upazila.Name,
                        AreaType = createdHouse.AreaType,
                        Ward = createdHouse.Ward.Name,
                        Village = createdHouse.Village.Name,
                        Owner = createdHouse.OwnerId
                    };
                    _logger.LogInformation($"House DTO created: {houseDto.Id}");
                    return (true, houseDto, Array.Empty<string>());

                }
                else
                {
                    _logger.LogError($"Failed to create house: {house.Name}");
                    return (false, null, new[] { "Failed to create house." });
                }
            }
            catch
            {
                _logger.LogError($"An error occurred while creating the house.");
                return (false, null, new[] { "An error occurred while creating the house." });
            }
            
        }

        public async Task<(bool success, HouseDTO? house, string[] errors)> GetHouseByIdAsync(int id)
        {
            try
            {
                var house = await _context.Houses
                    .Include(h => h.Division)
                    .Include(h => h.District)
                    .Include(h => h.Upazila)
                    .Include(h => h.Ward)
                    .Include(h => h.Village)
                    .Include(h => h.Owner)
                    .FirstOrDefaultAsync(h => h.Id == id);
                if (house == null)
                {
                    return (false, null, new[] { "House not found." });
                }
                var houseDto = _mapper.Map<HouseDTO>(house);
                return (true, houseDto, Array.Empty<string>());
            }
            catch
            {
                return (false, null, new[] { "An error occurred while retrieving the house." });
            }

        }
    }
}
