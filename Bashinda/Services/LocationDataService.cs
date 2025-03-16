using Bashinda.Models;

namespace Bashinda.Services
{
    public interface ILocationDataService
    {
        Task<int> GetDivisionIdAsync(string divisionName);
        Task<int> GetDistrictIdAsync(string districtName, int divisionId);
        Task<int> GetUpazilaIdAsync(string upazilaName, int districtId);
        Task<int> GetWardIdAsync(string wardName, int upazilaId, string localityType);
        Task<int> GetVillageIdAsync(string villageName, int wardId);
        Task<AreaType> GetAreaTypeAsync(string localityTypeName);
    }

    public class LocationDataService : ILocationDataService
    {
        // Mock data for location IDs (in a real application, this would come from the database)
        private readonly Dictionary<string, int> _divisions = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "Dhaka", 1 },
            { "Chittagong", 2 },
            { "Khulna", 3 },
            { "Rajshahi", 4 },
            { "Sylhet", 5 }
        };

        private readonly Dictionary<(int DivisionId, string District), int> _districts = new Dictionary<(int, string), int>(new DistrictComparer())
        {
            { (1, "Dhaka"), 101 },
            { (1, "Gazipur"), 102 },
            { (1, "Narayanganj"), 103 },
            { (1, "Narsingdi"), 104 },
            { (1, "Tangail"), 105 },
            { (2, "Chittagong"), 201 },
            { (2, "Cox's Bazar"), 202 },
            { (2, "Bandarban"), 203 },
            { (2, "Rangamati"), 204 },
            { (2, "Khagrachari"), 205 },
            { (3, "Khulna"), 301 },
            { (3, "Bagerhat"), 302 },
            { (3, "Satkhira"), 303 },
            { (3, "Jessore"), 304 },
            { (3, "Jhenaidah"), 305 },
            { (4, "Rajshahi"), 401 },
            { (4, "Chapainawabganj"), 402 },
            { (4, "Natore"), 403 },
            { (4, "Naogaon"), 404 },
            { (4, "Bogra"), 405 },
            { (5, "Sylhet"), 501 },
            { (5, "Moulvibazar"), 502 },
            { (5, "Habiganj"), 503 },
            { (5, "Sunamganj"), 504 }
        };

        private readonly Dictionary<(int DistrictId, string Upazila), int> _upazilas = new Dictionary<(int, string), int>(new UpazilaComparer())
        {
            { (101, "Dhaka North"), 10101 },
            { (101, "Dhaka South"), 10102 },
            { (101, "Savar"), 10103 },
            { (101, "Dhamrai"), 10104 },
            { (101, "Keraniganj"), 10105 },
            { (102, "Gazipur Sadar"), 10201 },
            { (102, "Kaliakoir"), 10202 },
            { (102, "Kaliganj"), 10203 },
            { (102, "Kapasia"), 10204 },
            { (102, "Sreepur"), 10205 },
            { (201, "Chittagong City"), 20101 },
            { (201, "Anwara"), 20102 },
            { (201, "Boalkhali"), 20103 },
            { (201, "Chandanaish"), 20104 },
            { (201, "Fatikchhari"), 20105 }
        };

        private readonly Dictionary<(int UpazilaId, string Ward, AreaType AreaType), int> _wards = new Dictionary<(int, string, AreaType), int>(new WardComparer())
        {
            { (10101, "Ward 1", AreaType.CityCorporation), 1010101 },
            { (10101, "Ward 2", AreaType.CityCorporation), 1010102 },
            { (10101, "Ward 3", AreaType.CityCorporation), 1010103 },
            { (10101, "Ward 4", AreaType.CityCorporation), 1010104 },
            { (10101, "Ward 5", AreaType.CityCorporation), 1010105 },
            { (10102, "Ward 6", AreaType.CityCorporation), 1010201 },
            { (10102, "Ward 7", AreaType.CityCorporation), 1010202 },
            { (10102, "Ward 8", AreaType.CityCorporation), 1010203 },
            { (10102, "Ward 9", AreaType.CityCorporation), 1010204 },
            { (10102, "Ward 10", AreaType.CityCorporation), 1010205 },
            { (10103, "Ward 1", AreaType.Pourashava), 1010301 },
            { (10103, "Ward 2", AreaType.Pourashava), 1010302 },
            { (10103, "Ward 3", AreaType.Pourashava), 1010303 }
        };

        private readonly Dictionary<(int WardId, string Village), int> _villages = new Dictionary<(int, string), int>(new VillageComparer())
        {
            { (1010101, "Mohakhali"), 101010101 },
            { (1010101, "Banani"), 101010102 },
            { (1010101, "Gulshan"), 101010103 },
            { (1010101, "Baridhara"), 101010104 },
            { (1010101, "Niketan"), 101010105 },
            { (1010102, "Dhanmondi"), 101010201 },
            { (1010102, "Lalmatia"), 101010202 },
            { (1010102, "Mohammadpur"), 101010203 },
            { (1010102, "Adabar"), 101010204 },
            { (1010102, "Shyamoli"), 101010205 },
            { (1010201, "Lalbagh"), 101020101 },
            { (1010201, "Hazaribagh"), 101020102 },
            { (1010201, "Nawabganj"), 101020103 },
            { (1010201, "Kamrangirchar"), 101020104 },
            { (1010201, "Chawkbazar"), 101020105 }
        };

        public Task<int> GetDivisionIdAsync(string divisionName)
        {
            if (_divisions.TryGetValue(divisionName, out int id))
            {
                return Task.FromResult(id);
            }
            return Task.FromResult(1); // Default to Dhaka if not found
        }

        public Task<int> GetDistrictIdAsync(string districtName, int divisionId)
        {
            var key = (divisionId, districtName);
            if (_districts.TryGetValue(key, out int id))
            {
                return Task.FromResult(id);
            }
            return Task.FromResult(101); // Default to Dhaka district if not found
        }

        public Task<int> GetUpazilaIdAsync(string upazilaName, int districtId)
        {
            var key = (districtId, upazilaName);
            if (_upazilas.TryGetValue(key, out int id))
            {
                return Task.FromResult(id);
            }
            return Task.FromResult(10101); // Default to Dhaka North if not found
        }

        public Task<int> GetWardIdAsync(string wardName, int upazilaId, string localityType)
        {
            AreaType areaType = GetAreaTypeAsync(localityType).Result;
            var key = (upazilaId, wardName, areaType);
            if (_wards.TryGetValue(key, out int id))
            {
                return Task.FromResult(id);
            }
            return Task.FromResult(1010101); // Default to Ward 1 if not found
        }

        public Task<int> GetVillageIdAsync(string villageName, int wardId)
        {
            var key = (wardId, villageName);
            if (_villages.TryGetValue(key, out int id))
            {
                return Task.FromResult(id);
            }
            return Task.FromResult(101010101); // Default to Mohakhali if not found
        }

        public Task<AreaType> GetAreaTypeAsync(string localityTypeName)
        {
            return Task.FromResult(localityTypeName.ToLower() switch
            {
                "citycorporation" => AreaType.CityCorporation,
                "pourashava" => AreaType.Pourashava,
                "union" => AreaType.Union,
                _ => AreaType.CityCorporation // Default
            });
        }

        // Custom comparer classes for complex keys
        private class DistrictComparer : IEqualityComparer<(int DivisionId, string District)>
        {
            public bool Equals((int DivisionId, string District) x, (int DivisionId, string District) y)
            {
                return x.DivisionId == y.DivisionId && 
                       string.Equals(x.District, y.District, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode((int DivisionId, string District) obj)
            {
                return HashCode.Combine(obj.DivisionId, obj.District.ToLower());
            }
        }

        private class UpazilaComparer : IEqualityComparer<(int DistrictId, string Upazila)>
        {
            public bool Equals((int DistrictId, string Upazila) x, (int DistrictId, string Upazila) y)
            {
                return x.DistrictId == y.DistrictId && 
                       string.Equals(x.Upazila, y.Upazila, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode((int DistrictId, string Upazila) obj)
            {
                return HashCode.Combine(obj.DistrictId, obj.Upazila.ToLower());
            }
        }

        private class WardComparer : IEqualityComparer<(int UpazilaId, string Ward, AreaType AreaType)>
        {
            public bool Equals((int UpazilaId, string Ward, AreaType AreaType) x, (int UpazilaId, string Ward, AreaType AreaType) y)
            {
                return x.UpazilaId == y.UpazilaId && 
                       string.Equals(x.Ward, y.Ward, StringComparison.OrdinalIgnoreCase) &&
                       x.AreaType == y.AreaType;
            }

            public int GetHashCode((int UpazilaId, string Ward, AreaType AreaType) obj)
            {
                return HashCode.Combine(obj.UpazilaId, obj.Ward.ToLower(), obj.AreaType);
            }
        }

        private class VillageComparer : IEqualityComparer<(int WardId, string Village)>
        {
            public bool Equals((int WardId, string Village) x, (int WardId, string Village) y)
            {
                return x.WardId == y.WardId && 
                       string.Equals(x.Village, y.Village, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode((int WardId, string Village) obj)
            {
                return HashCode.Combine(obj.WardId, obj.Village.ToLower());
            }
        }
    }
} 