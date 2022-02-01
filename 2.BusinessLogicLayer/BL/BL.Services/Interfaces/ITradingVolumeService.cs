using System;
using System.Collections.Generic;
using Core.Domain.Enums;
using isRock.LineBot;

namespace BL.Services.Interfaces
{
    public interface ITradingVolumeService
    {
        List<MessageBase> GetTradingVolumeStrOverDays(QuerySortTypeEnum querySortType, int days);

        List<MessageBase> GetTradingVolumeStr(DateTime date, QuerySortTypeEnum querySortType);
    }
}