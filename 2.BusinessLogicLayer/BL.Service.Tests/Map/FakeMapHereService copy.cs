using BL.Service.Map;
using BL.Service.MapQuest;
using Core.Domain.DTO.Map;

namespace BL.Service.Tests.Map
{
    /// <summary>
    /// here服務的 API [Project Name: Freemium 2020-09-19]
    /// 25000 transactions per month for free
    /// 2.5GB data hub data tranfer per month for free
    /// 5GB studio and data hub database storage per month for free
    /// https://developer.here.com/documentation
    /// </summary>
    public class FakeMapQuestService : IMapQuestService
    {
        public List<string> GetAddressInOrder(string sourceAddress, List<string> targetAddresses)
        {
            return new List<string>();
        }

        public Task<int> GetDuration(Core.Domain.DTO.Map.LatLng l1, Core.Domain.DTO.Map.LatLng l2)
        {
            return new Task<int>(() => 1);
        }

        public Core.Domain.DTO.Map.LatLng GetLatLngFromAddress(string address)
        {
            return new Core.Domain.DTO.Map.LatLng
            {
                Lat = 0,
                Lng = 0
            };
        }
    }
}