using CTravel.API.DTO;
using CTravel.API.Helpers;
using CTravel.API.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CTravel.API.Repository
{
    public class PlaceRepository
    {
        public Response<List<TouristPlaceDTO>> GetPlace(int stateId, int cityId)
        {
            var list = new List<TouristPlaceDTO>();
            var response = new Response<List<TouristPlaceDTO>>();

            try
            {
                using (var con = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("GetTouristPlacesByStateCity", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StateID", stateId);
                    cmd.Parameters.AddWithValue("@CityID", cityId); // fixed double @@

                    var statusCodeParam = new SqlParameter("@StatusCode", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(statusCodeParam);

                    var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 500)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(messageParam);

                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                          
                    var dto = new TouristPlaceDTO
                    {
                        PlaceID = Convert.ToInt32(dr["PlaceID"]),
                        StateName = dr["PlaceName"].ToString(),
                        DistrictName = dr["Description"].ToString(),
                        CityName = dr["Description"].ToString(),
                        CategoryName = dr["Description"].ToString(),
                        TouristPlaceName = dr["Description"].ToString()
                    };

                            list.Add(dto);
                        }
                    }
                }

                response.MessageID = 100;
                response.Data = list;
                response.MessageDesc = "Tourist places fetched successfully.";
            }
            catch (Exception ex)
            {
                response.MessageID = 100;
                response.Data = null;
                response.MessageDesc = ex.Message;
                
            }

            return response;
        }
    }
    
}