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
    public class TouristPlaceRequest
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
        public int? ModifiedBy { get; set; }      // ← add
        public DateTime? ModifiedOn { get; set; } // ← add

        public List<SelectDTO> GetUsers { get; set; } = new List<SelectDTO>();
        public List<SelectDTO> GetStates { get; set; } = new List<SelectDTO>();
        public List<SelectDTO> GetDistricts { get; set; } = new List<SelectDTO>();
        public List<SelectDTO> GetCity { get; set; } = new List<SelectDTO>();


    }

    public class SelectDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class PlaceTicketFeeDTO
    {
        public int TicketFeeID { get; set; }
        public int PlaceID { get; set; }
        public decimal? EntryFeeIndianAdult { get; set; }
        public decimal? EntryFeeIndianChild { get; set; }
        public decimal? EntryFeeNonIndianAdult { get; set; }
        public decimal? EntryFeeNonIndianChild { get; set; }
        public string TicketingPlatform { get; set; }
        public string TicketingUrl { get; set; }
    }

    public class UpsertPlaceTicketFeeRequest
    {
        public int TicketFeeID { get; set; } // 0 = create, >0 = update
        public int PlaceID { get; set; }
        public decimal? EntryFeeIndianAdult { get; set; }
        public decimal? EntryFeeIndianChild { get; set; }
        public decimal? EntryFeeNonIndianAdult { get; set; }
        public decimal? EntryFeeNonIndianChild { get; set; }
        public string TicketingPlatform { get; set; }
        public string TicketingUrl { get; set; }
    }

}