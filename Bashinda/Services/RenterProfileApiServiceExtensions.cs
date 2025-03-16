using Bashinda.Models;
using Bashinda.ViewModels;

namespace Bashinda.Services
{
    public static class RenterProfileApiServiceExtensions
    {
        public static async Task<(bool Success, RenterProfileDto? Data, string[] Errors)> CreateWithLocationIdsAsync(
            this IRenterProfileApiService renterProfileApiService,
            RenterProfileViewModel viewModel,
            ILocationDataService locationDataService,
            ILocationApiService locationApiService,
            ILogger<IRenterProfileApiService> logger,
            string token)
        {
            // Create the DTO with basic information
            var dto = new CreateRenterProfileDto
            {
                IsAdult = viewModel.IsAdult,
                NationalId = viewModel.NationalId,
                BirthRegistrationNo = viewModel.BirthRegistrationNo,
                DateOfBirth = viewModel.DateOfBirth,
                FullName = viewModel.FullName,
                FatherName = viewModel.FatherName,
                MotherName = viewModel.MotherName, 
                // Convert enum types to strings
                Nationality = viewModel.Nationality.ToString(),
                BloodGroup = viewModel.BloodGroup.ToString(),
                Profession = viewModel.Profession.ToString(),
                Gender = viewModel.Gender.ToString(),
                MobileNo = viewModel.MobileNo,
                Email = viewModel.Email,
                // Store the string values for reference
                Division = viewModel.Division,
                District = viewModel.District,
                Upazila = viewModel.Upazila,
                LocalityType = viewModel.LocalityType,
                Ward = viewModel.Ward,
                VillageArea = viewModel.VillageArea,
                PostCode = viewModel.PostCode,
                HoldingNo = viewModel.HoldingNo
            };

            try
            {
                // First, try to get divisions from API
                var divisions = await locationApiService.GetDivisionsAsync(token);
                if (divisions.Count == 0)
                {
                    logger.LogWarning("No divisions found from API, falling back to mock data");
                }

                // Find the division ID by name
                int divisionId;
                var divisionItem = divisions.FirstOrDefault(d => string.Equals(d.Name, viewModel.Division, StringComparison.OrdinalIgnoreCase));
                if (divisionItem != null)
                {
                    divisionId = divisionItem.Id;
                    logger.LogInformation("Found division ID {DivisionId} for name {Division}", divisionId, viewModel.Division);
                }
                else
                {
                    // Fall back to mock data
                    divisionId = await locationDataService.GetDivisionIdAsync(viewModel.Division);
                    logger.LogInformation("Using fallback division ID {DivisionId} for name {Division}", divisionId, viewModel.Division);
                }
                dto.DivisionId = divisionId;

                // Get districts for the selected division
                var districts = await locationApiService.GetDistrictsAsync(divisionId, token);
                if (districts.Count == 0)
                {
                    logger.LogWarning("No districts found from API for division ID {DivisionId}, falling back to mock data", divisionId);
                }

                // Find the district ID by name
                int districtId;
                var districtItem = districts.FirstOrDefault(d => string.Equals(d.Name, viewModel.District, StringComparison.OrdinalIgnoreCase));
                if (districtItem != null)
                {
                    districtId = districtItem.Id;
                    logger.LogInformation("Found district ID {DistrictId} for name {District}", districtId, viewModel.District);
                }
                else
                {
                    // Fall back to mock data
                    districtId = await locationDataService.GetDistrictIdAsync(viewModel.District, divisionId);
                    logger.LogInformation("Using fallback district ID {DistrictId} for name {District}", districtId, viewModel.District);
                }
                dto.DistrictId = districtId;

                // Get upazilas for the selected district
                var upazilas = await locationApiService.GetUpazilasAsync(districtId, token);
                if (upazilas.Count == 0)
                {
                    logger.LogWarning("No upazilas found from API for district ID {DistrictId}, falling back to mock data", districtId);
                }

                // Find the upazila ID by name
                int upazilaId;
                var upazilaItem = upazilas.FirstOrDefault(u => string.Equals(u.Name, viewModel.Upazila, StringComparison.OrdinalIgnoreCase));
                if (upazilaItem != null)
                {
                    upazilaId = upazilaItem.Id;
                    logger.LogInformation("Found upazila ID {UpazilaId} for name {Upazila}", upazilaId, viewModel.Upazila);
                }
                else
                {
                    // Fall back to mock data
                    upazilaId = await locationDataService.GetUpazilaIdAsync(viewModel.Upazila, districtId);
                    logger.LogInformation("Using fallback upazila ID {UpazilaId} for name {Upazila}", upazilaId, viewModel.Upazila);
                }
                dto.UpazilaId = upazilaId;

                // Convert locality type string to enum
                var areaType = await locationDataService.GetAreaTypeAsync(viewModel.LocalityType);
                dto.AreaType = areaType;
                logger.LogInformation("Using area type {AreaType} for locality type {LocalityType}", areaType, viewModel.LocalityType);

                // Get wards for the selected upazila and area type
                var wards = await locationApiService.GetWardsAsync(upazilaId, areaType, token);
                if (wards.Count == 0)
                {
                    logger.LogWarning("No wards found from API for upazila ID {UpazilaId} and area type {AreaType}, falling back to mock data", 
                        upazilaId, areaType);
                }

                // Find the ward ID by name
                int wardId;
                var wardItem = wards.FirstOrDefault(w => string.Equals(w.Name, viewModel.Ward, StringComparison.OrdinalIgnoreCase));
                if (wardItem != null)
                {
                    wardId = wardItem.Id;
                    logger.LogInformation("Found ward ID {WardId} for name {Ward}", wardId, viewModel.Ward);
                }
                else
                {
                    // Fall back to mock data
                    wardId = await locationDataService.GetWardIdAsync(viewModel.Ward, upazilaId, viewModel.LocalityType);
                    logger.LogInformation("Using fallback ward ID {WardId} for name {Ward}", wardId, viewModel.Ward);
                }
                dto.WardId = wardId;

                // Get villages for the selected ward
                var villages = await locationApiService.GetVillagesAsync(wardId, token);
                if (villages.Count == 0)
                {
                    logger.LogWarning("No villages found from API for ward ID {WardId}, falling back to mock data", wardId);
                }

                // Find the village ID by name
                int villageId;
                var villageItem = villages.FirstOrDefault(v => string.Equals(v.Name, viewModel.VillageArea, StringComparison.OrdinalIgnoreCase));
                if (villageItem != null)
                {
                    villageId = villageItem.Id;
                    logger.LogInformation("Found village ID {VillageId} for name {Village}", villageId, viewModel.VillageArea);
                }
                else
                {
                    // Fall back to mock data
                    villageId = await locationDataService.GetVillageIdAsync(viewModel.VillageArea, wardId);
                    logger.LogInformation("Using fallback village ID {VillageId} for name {Village}", villageId, viewModel.VillageArea);
                }
                dto.VillageId = villageId;

                // Log the final DTO values for debugging
                logger.LogInformation("Creating renter profile with location IDs: Division={DivisionId}, District={DistrictId}, Upazila={UpazilaId}, Ward={WardId}, Village={VillageId}, AreaType={AreaType}", 
                    dto.DivisionId, dto.DistrictId, dto.UpazilaId, dto.WardId, dto.VillageId, dto.AreaType);

                // Call the original CreateAsync method with our processed DTO
                return await renterProfileApiService.CreateAsync(dto, token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error converting location names to IDs");
                return (false, null, new[] { $"Error converting location names to IDs: {ex.Message}" });
            }
        }
    }
} 