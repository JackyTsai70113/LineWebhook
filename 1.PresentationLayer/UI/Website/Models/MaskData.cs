using System;

namespace Website.Models
{
    public class InstitutionMaskData
    {
        public int Id { get; set; }

        public string Name {set; get;}

        public string Address { get; set; }

        public string PhoneNumber {set; get;}

        public int AdultMasks { get; set; }

        public int ChildMasks {set; get;}

        public string UpdateTime { get; set; }
    }
}