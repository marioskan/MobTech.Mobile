using System;
using System.Collections.Generic;
using System.Text;

namespace MobTech.Mobile.Entities
{
    public class UpdatedAED
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Photograph { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int SearchedID { get; set; }
    }
}
