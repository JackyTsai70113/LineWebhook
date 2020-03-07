using System;

namespace Models.MaskDatas
{
    public class MaskData
    {
        public string Id { get; set; }

        public string Name {set; get;}

        public string Address { get; set; }

        public string PhoneNumber {set; get;}

        public int AdultMasks { get; set; }

        public int ChildMasks {set; get;}

        public DateTime UpdateTime { get; set; }
    }
}