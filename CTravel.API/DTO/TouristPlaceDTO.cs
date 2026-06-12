using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTravel.API.DTO
{
    public class TouristPlaceDTO
    {
        public int PlaceID { get; set; }
        public string StateName { get; set; }

        public string DistrictName { get; set; }

        public string CityName { get; set; }

        public string CategoryName { get; set; }

        public string TouristPlaceName { get; set; }

        public string AboutPlace { get; set; }

        public string BestTime { get; set; }

        public string Timings { get; set; }

        public bool CommoneerPick { get; set; }

        public bool OffbeatHiddenGem { get; set; }

        public bool SeasonalPick { get; set; }
        public string OfficialWebsiteLink { get; set; }

        public bool IsActive { get; set; }
    }
}