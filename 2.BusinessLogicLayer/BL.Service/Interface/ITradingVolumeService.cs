using Core.Domain.Enums;
using isRock.LineBot;

namespace BL.Service.Interface {
    public interface ITradingVolumeService {
        List<MessageBase> GetTradingVolumeStrOverDays(QuerySortTypeEnum querySortType, int days);

        List<MessageBase> GetTradingVolumeStr(DateTime date, QuerySortTypeEnum querySortType);
    }
}