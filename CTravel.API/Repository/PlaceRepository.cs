using CTravel.API.DTO;
using CTravel.API.Helpers;
using CTravel.API.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
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
                        StateName = dr["StateName"].ToString(),
                        DistrictName = dr["DistrictName"].ToString(),
                        CityName = dr["CityName"].ToString(),
                        AboutPlace = dr["TouristPlaceName"].ToString(),
                        TouristPlaceName = dr["TouristPlaceName"].ToString()
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

        public Response<TouristPlaceRequest> GetCreateTouristPlace(int PlaceID)
        {
            var response = new Response<TouristPlaceRequest>();
            try
            {
                // 1. Always initialize vm first
                TouristPlaceRequest vm = new TouristPlaceRequest();

                if (PlaceID > 0)
                {
                    using (var con = DbHelper.GetConnection())
                    using (var cmd = new SqlCommand("GetTouristPlaceByID", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PlaceID", PlaceID);

                        var statusCodeParam = new SqlParameter("@StatusCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(statusCodeParam);
                        cmd.Parameters.Add(messageParam);

                        con.Open();
                        using (var dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                // 2. Map into vm (not response.Data directly)
                                vm.PlaceID = Convert.ToInt32(dr["PlaceID"]);
                                vm.StateID = Convert.ToInt32(dr["StateID"]);
                                vm.DistrictID = Convert.ToInt32(dr["DistrictID"]);
                                vm.CityID = dr["CityID"] != DBNull.Value ? Convert.ToInt32(dr["CityID"]) : (int?)null;
                                vm.CategoryID = dr["CategoryID"] != DBNull.Value ? Convert.ToInt32(dr["CategoryID"]) : (int?)null;
                                vm.TouristPlaceName = dr["TouristPlaceName"].ToString();
                                vm.AboutPlace = dr["AboutPlace"].ToString();
                                vm.BestTime = dr["BestTime"].ToString();
                                vm.Timings = dr["Timings"].ToString();
                                vm.CommoneerPick = Convert.ToBoolean(dr["CommoneerPick"]);
                                vm.OffbeatHiddenGem = Convert.ToBoolean(dr["OffbeatHiddenGem"]);
                                vm.SeasonalPick = Convert.ToBoolean(dr["SeasonalPick"]);
                                vm.CommoneerIndex = dr["CommoneerIndex"] != DBNull.Value ? Convert.ToDecimal(dr["CommoneerIndex"]) : (decimal?)null;
                                vm.OfficialWebsiteLink = dr["OfficialWebsiteLink"].ToString();
                                vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                                vm.ModifiedBy = dr["ModifiedBy"] != DBNull.Value ? Convert.ToInt32(dr["ModifiedBy"]) : (int?)null;
                                vm.ModifiedOn = dr["ModifiedOn"] != DBNull.Value ? Convert.ToDateTime(dr["ModifiedOn"]) : (DateTime?)null;
                            }
                        }

                        response.MessageID = (int)statusCodeParam.Value;
                        response.MessageDesc = messageParam.Value?.ToString();
                    }
                }

                // 3. Populate dropdowns AFTER vm is initialized and data is mapped
                //    so StateID and DistrictID are already set for cascading dropdowns
                PopulateDropdowns(vm);

                response.Data = vm;
                response.MessageID = 1;
                response.MessageDesc = "Tourist place fetched successfully.";
            }
            catch (Exception ex)
            {
                response.MessageID = -1;
                response.MessageDesc = ex.Message;
                response.Data = null;
            }

            return response;
        }


        private void PopulateDropdowns(TouristPlaceRequest vm)
        {
            if (vm == null) vm = new TouristPlaceRequest();

            try
            {
                var users = GetUsersAsync();
                var states = GetStatesAsync();

                vm.GetUsers = users ?? new List<SelectDTO>();
                vm.GetStates = states ?? new List<SelectDTO>();
                vm.GetDistricts = vm.StateID > 0 ? GetDistrictsAsync(vm.StateID) ?? new List<SelectDTO>() : new List<SelectDTO>();
                vm.GetCity = vm.DistrictID > 0 ? GetCitiesAsync(vm.DistrictID) ?? new List<SelectDTO>() : new List<SelectDTO>();
            }
            catch (Exception ex)
            {
                // see exactly where it fails
                System.Diagnostics.Debug.WriteLine("PopulateDropdowns error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("StackTrace: " + ex.StackTrace);
            }
        }

        public Response<object> GetDistricts(int stateId)
        {
            const string sql = "select DistrictID,DistrictName from [dbo].[sysDistrict]  where [StateID]=@StateID";

            var parameters = new[]
            {
            new SqlParameter("@StateID", SqlDbType.Int) { Value = stateId }
           };

            List<SelectDTO> res = FetchAsync(sql, parameters);

            return new Response<object>
            {
                 Data= res, MessageID=100, MessageDesc="Ok"
            };
        }

        public Response<object>  GetCities(int DistrictID)
        {
            const string sql = " SELECT CityID AS Value, CityName AS Text    FROM dbo.Cities  WHERE  IsActive = 1    AND DistrictID = @DistrictID       ORDER BY CityName";
            var parameters = new[]
            {
            new SqlParameter("@DistrictID", SqlDbType.Int) { Value = DistrictID }
           };

            List<SelectDTO> res = FetchAsync(sql, parameters);

            return new Response<object>
            {
                Data = res,
                MessageID = 100,
                MessageDesc = "Ok"
            };
        }

        public Response<object> CreatePlace(TouristPlaceRequest req)
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
                    cmd.Parameters.AddWithValue("@CityID", req.CityID );
                    cmd.Parameters.AddWithValue("@CategoryID", req.CategoryID );
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
                    cmd.Parameters.AddWithValue("@CreatedBy", req.ModifiedBy);


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
        public Response<object> UpdatePlace(TouristPlaceRequest req)
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
                    cmd.Parameters.AddWithValue("@CityID", req.CityID );
                    cmd.Parameters.AddWithValue("@CategoryID", req.CategoryID );
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

        private List<SelectDTO> FetchAsync(string sql, SqlParameter[] parameters = null)
        {
            var list = new List<SelectDTO>();

            SqlConnection sqlConnection = DbHelper.GetConnection();
            using (SqlConnection conn = sqlConnection)
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new SelectDTO
                        {
                            ID = dr.GetInt32(0),
                            Name = dr.GetString(1)
                        });
                    }

                }

            }
            return list;
        }

        // ── GET USERS ──────────────────────────────────────────────────────────────
        public List<SelectDTO> GetUsersAsync()
        {
            string sql = "select  [UserID] , CONCAT( [FirstName],'',LastName ) fullname from tblUser  WHERE  IsActive = 1";
            return FetchAsync(sql);
        }

        // ── GET STATES ─────────────────────────────────────────────────────────────
        public List<SelectDTO> GetStatesAsync()
        {
            const string sql = "select StateID,StateName from sysState order by  StateName";

            return FetchAsync(sql);
        }

        // ── GET DISTRICTS (filtered by StateID) ───────────────────────────────────
        public List<SelectDTO> GetDistrictsAsync(int stateId)
        {
            const string sql = "select DistrictID,DistrictName from [dbo].[sysDistrict]  where [StateID]=@@StateID";

            var parameters = new[]
            {
            new SqlParameter("@StateID", SqlDbType.Int) { Value = stateId }
        };

            return FetchAsync(sql, parameters);
        }

        // ── GET CITIES (filtered by DistrictID) ───────────────────────────────────
        public List<SelectDTO> GetCitiesAsync(int districtId)
        {
          
            
            const string sql = " SELECT CityID AS Value, CityName AS Text    FROM dbo.Cities  WHERE  IsActive = 1    AND DistrictID = @DistrictID       ORDER BY CityName";
            var parameters = new[]
            {
            new SqlParameter("@DistrictID", SqlDbType.Int) { Value = districtId }
        };

            return FetchAsync(sql, parameters);
        }


        public Response<List<PlaceTicketFeeDTO>> GetTicketFeesByPlace(int placeId)
        {
            var list = new List<PlaceTicketFeeDTO>();
            var response = new Response<List<PlaceTicketFeeDTO>>();

            try
            {
                using (var con = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("GetPlaceTicketFees", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PlaceID", placeId);

                    var statusCodeParam = new SqlParameter("@StatusCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(statusCodeParam);
                    cmd.Parameters.Add(messageParam);

                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new PlaceTicketFeeDTO
                            {
                                TicketFeeID = Convert.ToInt32(dr["TicketFeeID"]),
                                PlaceID = Convert.ToInt32(dr["PlaceID"]),
                                EntryFeeIndianAdult = dr["EntryFeeIndianAdult"] != DBNull.Value ? Convert.ToDecimal(dr["EntryFeeIndianAdult"]) : (decimal?)null,
                                EntryFeeIndianChild = dr["EntryFeeIndianChild"] != DBNull.Value ? Convert.ToDecimal(dr["EntryFeeIndianChild"]) : (decimal?)null,
                                EntryFeeNonIndianAdult = dr["EntryFeeNonIndianAdult"] != DBNull.Value ? Convert.ToDecimal(dr["EntryFeeNonIndianAdult"]) : (decimal?)null,
                                EntryFeeNonIndianChild = dr["EntryFeeNonIndianChild"] != DBNull.Value ? Convert.ToDecimal(dr["EntryFeeNonIndianChild"]) : (decimal?)null,
                                TicketingPlatform = dr["TicketingPlatform"] != DBNull.Value ? dr["TicketingPlatform"].ToString() : null,
                                TicketingUrl = dr["TicketingUrl"] != DBNull.Value ? dr["TicketingUrl"].ToString() : null
                            });
                        }
                    }

                    response.MessageID = (int)statusCodeParam.Value;
                    response.MessageDesc = messageParam.Value?.ToString();
                    response.Data = list;
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

        public Response<object> UpsertTicketFee(UpsertPlaceTicketFeeRequest req)
        {
            var response = new Response<object>();

            try
            {
                using (var con = DbHelper.GetConnection())
                using (var cmd = new SqlCommand("UpsertPlaceTicketFees", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TicketFeeID", req.TicketFeeID);
                    cmd.Parameters.AddWithValue("@PlaceID", req.PlaceID);
                    cmd.Parameters.AddWithValue("@EntryFeeIndianAdult", req.EntryFeeIndianAdult ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EntryFeeIndianChild", req.EntryFeeIndianChild ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EntryFeeNonIndianAdult", req.EntryFeeNonIndianAdult ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EntryFeeNonIndianChild", req.EntryFeeNonIndianChild ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TicketingPlatform", req.TicketingPlatform ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TicketingUrl", req.TicketingUrl ?? (object)DBNull.Value);

                    var statusCodeParam = new SqlParameter("@StatusCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(statusCodeParam);
                    cmd.Parameters.Add(messageParam);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    response.MessageID = (int)statusCodeParam.Value;
                    response.MessageDesc = messageParam.Value?.ToString();
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