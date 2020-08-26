using System;

namespace Core.Domain.DTO.MaskInstitution {

    public class MaskData {

        public MaskData(string maskDataStr) {
            var maskDataArr = maskDataStr.Split(',');
            if (!Int32.TryParse(maskDataArr[4], out int adultMasks)) {
                Console.WriteLine($"Ex: Cannot parse {maskDataArr[4]} to Int.");
                adultMasks = Int32.MaxValue;
            }
            if (!Int32.TryParse(maskDataArr[5], out int childMasks)) {
                Console.WriteLine($"Ex: Cannot parse {maskDataArr[5]} to Int.");
                childMasks = Int32.MaxValue;
            }
            if (!DateTime.TryParse(maskDataArr[6], out DateTime updateTime)) {
                Console.WriteLine($"Ex: Cannot parse {maskDataArr[6]} to Int.");
                updateTime = DateTime.MinValue;
            }
            Id = maskDataArr[0];
            Name = maskDataArr[1];
            Address = maskDataArr[2];
            PhoneNumber = maskDataArr[3];
            AdultMasks = adultMasks;
            ChildMasks = childMasks;
            UpdateTime = updateTime;
        }

        public string Id { get; set; }

        public string Name { set; get; }

        public string Address { get; set; }

        public string PhoneNumber { set; get; }

        public int AdultMasks { get; set; }

        public int ChildMasks { set; get; }

        public DateTime UpdateTime { get; set; }
    }
}