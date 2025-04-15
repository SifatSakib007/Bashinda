using Bashinda.Models;

namespace Bashinda.Services
{
    public interface ILocationDataService
    {
        Task<List<string>> GetDivisionsAsync();
        Task<List<string>> GetDistrictsAsync(string division);
        Task<List<string>> GetUpazilasAsync(string district);
        Task<List<string>> GetWardsAsync(string upazila, AreaType areaType);
        Task<List<string>> GetVillagesAsync(string ward);
        Task<AreaType> GetAreaTypeAsync(string localityTypeName);
        Task<List<string>> GetAreaTypesAsync();


    }

    public class LocationDataService : ILocationDataService
    {
        // Mock data for locations as string lists
        private readonly Dictionary<string, List<string>> _districtsByDivision = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "Dhaka", new List<string> { "Dhaka", "Gazipur", "Narayanganj", "Narsingdi", "Tangail" } },
            { "Chittagong", new List<string> { "Chittagong", "Cox's Bazar", "Bandarban", "Rangamati", "Khagrachari" } },
            { "Khulna", new List<string> { "Khulna", "Bagerhat", "Satkhira", "Jessore", "Jhenaidah" } },
            { "Rajshahi", new List<string> { "Rajshahi", "Chapainawabganj", "Natore", "Naogaon", "Bogra" } },
            { "Sylhet", new List<string> { "Sylhet", "Moulvibazar", "Habiganj", "Sunamganj", "Sreemangal" } },
            { "Barisal", new List<string> { "Barisal", "Bhola", "Patuakhali", "Pirojpur", "Jhalokati" } },
            { "Rangpur", new List<string> { "Rangpur", "Dinajpur", "Kurigram", "Gaibandha", "Nilphamari" } },
            { "Mymensingh", new List<string> { "Mymensingh", "Jamalpur", "Sherpur", "Netrokona", "Kishoreganj" } }
        };

        private readonly Dictionary<string, List<string>> _upazilasByDistrict = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "Dhaka", new List<string> { "Dhaka North", "Dhaka South", "Savar", "Dhamrai", "Keraniganj" } },
            { "Gazipur", new List<string> { "Gazipur Sadar", "Kaliakoir", "Kaliganj", "Kapasia", "Sreepur" } },
            { "Chittagong", new List<string> { "Chittagong City", "Anwara", "Boalkhali", "Chandanaish", "Fatikchhari" } }
            // Add more districts as needed
        };

        private readonly Dictionary<(string Upazila, AreaType AreaType), List<string>> _wardsByUpazilaAndType = new Dictionary<(string, AreaType), List<string>>(new WardComparer())
        {
            { ("Dhaka North", AreaType.CityCorporation), new List<string> { "Ward 1", "Ward 2", "Ward 3", "Ward 4", "Ward 5" } },
            { ("Dhaka South", AreaType.CityCorporation), new List<string> { "Ward 6", "Ward 7", "Ward 8", "Ward 9", "Ward 10" } },
            { ("Savar", AreaType.Pourasava), new List<string> { "Ward 1", "Ward 2", "Ward 3", "Ward 4", "Ward 5" } },
            { ("Savar", AreaType.Union), new List<string> { "Union 1", "Union 2", "Union 3", "Union 4", "Union 5" } }
            // Add more upazilas and area types as needed
        };

        private readonly Dictionary<string, List<string>> _villagesByWard = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "Ward 1", new List<string> { "Mohakhali", "Banani", "Gulshan", "Baridhara", "Niketan" } },
            { "Ward 2", new List<string> { "Dhanmondi", "Lalmatia", "Mohammadpur", "Adabar", "Shyamoli" } },
            { "Ward 6", new List<string> { "Lalbagh", "Hazaribagh", "Nawabganj", "Kamrangirchar", "Chawkbazar" } }
            // Add more wards as needed
        };

        public Task<List<string>> GetDivisionsAsync()
        {
            return Task.FromResult(_districtsByDivision.Keys.ToList());
        }

        public Task<List<string>> GetDistrictsAsync(string division)
        {
            if (_districtsByDivision.TryGetValue(division, out var districts))
            {
                return Task.FromResult(districts);
            }
            return Task.FromResult(new List<string> { "Dhaka" }); // Default
        }

        public Task<List<string>> GetUpazilasAsync(string district)
        {
            if (_upazilasByDistrict.TryGetValue(district, out var upazilas))
            {
                return Task.FromResult(upazilas);
            }
            
            // If district not found in the dictionary, return a default list
            return Task.FromResult(new List<string> { "Dhaka North", "Dhaka South" });
        }

        public Task<List<string>> GetWardsAsync(string upazila, AreaType areaType)
        {
            var key = (upazila, areaType);
            if (_wardsByUpazilaAndType.TryGetValue(key, out var wards))
            {
                return Task.FromResult(wards);
            }
            
            // If not found, return a default list
            return Task.FromResult(new List<string> { "Ward 1", "Ward 2", "Ward 3" });
        }

        public Task<List<string>> GetVillagesAsync(string ward)
        {
            if (_villagesByWard.TryGetValue(ward, out var villages))
            {
                return Task.FromResult(villages);
            }
            
            // If not found, return a default list
            return Task.FromResult(new List<string> { "Area 1", "Area 2", "Area 3" });
        }

        public Task<AreaType> GetAreaTypeAsync(string localityTypeName)
        {
            return Task.FromResult(localityTypeName.ToLower() switch
            {
                "citycorporation" => AreaType.CityCorporation,
                "pourashava" => AreaType.Pourasava,
                "union" => AreaType.Union,
                _ => AreaType.CityCorporation // Default
            });
        }

        public Task<List<string>> GetAreaTypesAsync()
        {
            // Return the enum values as strings, including all possible values
            return Task.FromResult(Enum.GetNames(typeof(AreaType)).ToList());
        }

        // Custom comparer class for complex key
        private class WardComparer : IEqualityComparer<(string Upazila, AreaType AreaType)>
        {
            public bool Equals((string Upazila, AreaType AreaType) x, (string Upazila, AreaType AreaType) y)
            {
                return string.Equals(x.Upazila, y.Upazila, StringComparison.OrdinalIgnoreCase) && 
                       x.AreaType == y.AreaType;
            }

            public int GetHashCode((string Upazila, AreaType AreaType) obj)
            {
                return HashCode.Combine(obj.Upazila.ToLower(), obj.AreaType);
            }
        }
    }
} 