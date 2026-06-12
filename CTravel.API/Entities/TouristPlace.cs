using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTravel.API.Entities
{
    public class TouristPlace
    {
            public int PlaceID { get; set; }

            public int StateID { get; set; }

            public int DistrictID { get; set; }

            public int? CityID { get; set; }

            public int? CategoryID { get; set; }

            public string TouristPlaceName { get; set; }

            public string AboutPlace { get; set; }

            public string BestTime { get; set; }

            public string Timings { get; set; }

            public bool CommoneerPick { get; set; }

            public bool OffbeatHiddenGem { get; set; }

            public bool SeasonalPick { get; set; }

            public decimal? CommoneerIndex { get; set; }

            public string OfficialWebsiteLink { get; set; }

            public bool IsActive { get; set; }

            public int? CreatedBy { get; set; }

            public DateTime CreatedOn { get; set; }

            public int? ModifiedBy { get; set; }

            public DateTime? ModifiedOn { get; set; }
        }

    
}