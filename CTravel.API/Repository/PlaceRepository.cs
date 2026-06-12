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

        public Response<object> CreatePlace(CreateTouristPlaceRequest req)
        {
            var response = new Response<object>();

            try
            {
                using (var con = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("CreateTouristPlace", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@StateID", req.StateID);
                    cmd.Parameters.AddWithValue("@DistrictID", req.DistrictID);
                    cmd.Parameters.AddWithValue("@CityID", req.CityID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CategoryID", req.CategoryID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TouristPlaceName", req.TouristPlaceName);
                    cmd.Parameters.AddWithValue("@AboutPlace", req.AboutPlace);
                    cmd.Parameters.AddWithValue("@BestTime", req.BestTime);
                    cmd.Parameters.AddWithValue("@Timings", req.Timings);
                    cmd.Parameters.AddWithValue("@CommoneerPick", req.CommoneerPick);
                    cmd.Parameters.AddWithValue("@OffbeatHiddenGem", req.OffbeatHiddenGem);
                    cmd.Parameters.AddWithValue("@SeasonalPick", req.SeasonalPick);
                    cmd.Parameters.AddWithValue("@CommoneerIndex", req.CommoneerIndex ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@OfficialWebsiteLink", req.OfficialWebsiteLink ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", req.IsActive);
                    cmd.Parameters.AddWithValue("@CreatedBy", req.CreatedBy);

                    var statusCodeParam = new SqlParameter("@StatusCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(statusCodeParam);
                    cmd.Parameters.Add(messageParam);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    response.MessageID = (int)statusCodeParam.Value;
                    response.MessageDesc = messageParam.Value?.ToString();
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                response.MessageID = -1;
                response.MessageDesc = ex.Message;
                response.Data = null;
            }

            return response;
        }
        public Response<object> UpdatePlace(UpdateTouristPlaceRequest req)
        {
            var response = new Response<object>();

            try
            {
                using (var con = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("UpdateTouristPlace", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PlaceID", req.PlaceID);
                    cmd.Parameters.AddWithValue("@StateID", req.StateID);
                    cmd.Parameters.AddWithValue("@DistrictID", req.DistrictID);
                    cmd.Parameters.AddWithValue("@CityID", req.CityID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CategoryID", req.CategoryID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TouristPlaceName", req.TouristPlaceName);
                    cmd.Parameters.AddWithValue("@AboutPlace", req.AboutPlace);
                    cmd.Parameters.AddWithValue("@BestTime", req.BestTime);
                    cmd.Parameters.AddWithValue("@Timings", req.Timings);
                    cmd.Parameters.AddWithValue("@CommoneerPick", req.CommoneerPick);
                    cmd.Parameters.AddWithValue("@OffbeatHiddenGem", req.OffbeatHiddenGem);
                    cmd.Parameters.AddWithValue("@SeasonalPick", req.SeasonalPick);
                    cmd.Parameters.AddWithValue("@CommoneerIndex", req.CommoneerIndex ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@OfficialWebsiteLink", req.OfficialWebsiteLink ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", req.IsActive);
                    cmd.Parameters.AddWithValue("@ModifiedBy", req.ModifiedBy);

                    var statusCodeParam = new SqlParameter("@StatusCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(statusCodeParam);
                    cmd.Parameters.Add(messageParam);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    response.MessageID = (int)statusCodeParam.Value;
                    response.MessageDesc = messageParam.Value?.ToString();
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                response.MessageID = -1;
                response.MessageDesc = ex.Message;
                response.Data = null;
            }

            return response;
        }

        public Response<object> DeletePlace(int placeId, int modifiedBy)
        {
            var response = new Response<object>();

            try
            {
                using (var con = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("DeleteTouristPlace", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PlaceID", placeId);
                    cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                    var statusCodeParam = new SqlParameter("@StatusCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(statusCodeParam);
                    cmd.Parameters.Add(messageParam);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    response.MessageID = (int)statusCodeParam.Value;
                    response.MessageDesc = messageParam.Value?.ToString();
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                response.MessageID = -1;
                response.MessageDesc = ex.Message;
                response.Data = null;
            }

            return response;
        }
    }
    
}