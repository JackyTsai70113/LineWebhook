using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Core.Domain.Enums {

    /// <summary>
    /// 漲跌Enum
    /// </summary>
    public enum StockDirectionEnum {
        Down = -1,
        Not = 0,
        Up = 1,
    }

    public static class StockDirection {

        public static StockDirectionEnum ToStockDirectionEnum(this string str) {
            switch (str) {
                case "+":
                    return StockDirectionEnum.Up;

                case "-":
                    return StockDirectionEnum.Down;

                case " ":
                    return StockDirectionEnum.Not;

                default:
                    throw new ValidationException("不符合 漲跌Enum 的預期(+/-/ )");
            }
        }
    }
}