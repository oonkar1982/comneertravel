using CTravel.API.DTO;
using CTravel.API.Filters;
using CTravel.API.Helpers;
using CTravel.API.Models;
using CTravel.API.Repository;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CTravel.API.Controllers
{
    [ApiKeyAuthFilter]
    [RoutePrefix("api/Places")]
    public class TouristPlaceController : ApiController
    {
        private readonly PlaceRepository _repo;

        public TouristPlaceController()
        {
            _repo = new PlaceRepository(); // direct instantiation
        }

        [HttpPost]
        [Route("Filter")]
        public HttpResponseMessage GetFilter([FromBody] TouristPlaceFilterRequest filter)
        {
            if (filter == null) filter = new TouristPlaceFilterRequest();

            var obj = _repo.GetTouristPlacesFilter(filter);
            if (obj.Data == null)
                return Request.CreateResponse(HttpStatusCode.OK,
                    Response<object>.Fail(obj.MessageID, obj.MessageDesc));

            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }
        // GET: api/Places/{StateId}/{CityId}
        [HttpGet]
        [Route("{StateId}/{CityId}")]
        public HttpResponseMessage GetPlace(int StateId, int CityId)
        {
            var obj = _repo.GetPlace(StateId, CityId);
            if (obj.Data == null || obj.Data.Count == 0)
                return Request.CreateResponse(HttpStatusCode.OK,
                    Response<List<TouristPlaceDTO>>.Fail(obj.MessageID, obj.MessageDesc));

            return Request.CreateResponse(HttpStatusCode.OK,
                Response<List<TouristPlaceDTO>>.Ok(obj.Data, obj.MessageID, obj.MessageDesc));
        }

        // GET: api/Places/GetByPlaceID?PlaceID=1
        [HttpGet]
        [Route("GetByPlaceID")]
        public HttpResponseMessage GetByPlaceID(int PlaceID)
        {
            var obj = _repo.GetCreateTouristPlace(PlaceID);
            if (obj.Data == null)
                return Request.CreateResponse(HttpStatusCode.OK,
                    Response<TouristPlaceRequest>.Fail(obj.MessageID, obj.MessageDesc));

            return Request.CreateResponse(HttpStatusCode.OK,
                Response<TouristPlaceRequest>.Ok(obj.Data, obj.MessageID, obj.MessageDesc));
        }

        // POST: api/Places/CreatePlace
        [HttpPost]
        [Route("CreatePlace")]
        public HttpResponseMessage CreatePlace([FromBody] TouristPlaceRequest req)
        {
            if (req == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    Response<object>.Fail(0, "Request body is null."));

            var obj = _repo.CreatePlace(req);
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }

        // PUT: api/Places/UpdatePlace
        [HttpPut]
        [Route("UpdatePlace")]
        public HttpResponseMessage UpdatePlace([FromBody] TouristPlaceRequest req)
        {
            if (req == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    Response<object>.Fail(0, "Request body is null."));

            var obj = _repo.UpdatePlace(req);
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }

        // DELETE: api/Places/{placeId}/{modifiedBy}
        [HttpDelete]
        [Route("{placeId}/{modifiedBy}")]
        public HttpResponseMessage DeletePlace(int placeId, int modifiedBy)
        {
            var obj = _repo.DeletePlace(placeId, modifiedBy);
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }

        //GET: api/Places/GetDistricts/{stateId

        [HttpGet]
        [Route("GetDistricts/{stateId}")]
        public HttpResponseMessage GetDistricts(int stateId)

        {
            var obj = _repo.GetDistricts(stateId);
            if (obj.Data == null )
                return Request.CreateResponse(HttpStatusCode.OK,
                    Response<List<SelectDTO>>.Fail(obj.MessageID, obj.MessageDesc));
          
            return Request.CreateResponse(HttpStatusCode.OK,
                Response<List<SelectDTO>>.Ok((List<SelectDTO>)obj.Data, obj.MessageID, obj.MessageDesc));
        }
        [HttpGet]
        [Route("GetCity/{Districtid}")]
        public HttpResponseMessage GetCities(int Districtid)
        {

            var obj = _repo.GetCities(Districtid);
            if (obj.Data == null )
                return Request.CreateResponse(HttpStatusCode.OK,
                    Response<List<SelectDTO>>.Fail(obj.MessageID, obj.MessageDesc));

            return Request.CreateResponse(HttpStatusCode.OK,
                Response<List<SelectDTO>>.Ok((List<SelectDTO>)obj.Data, obj.MessageID, obj.MessageDesc));
        }

        // GET: api/TicketFees/{placeId}
        [HttpGet]
        [Route("GetTicketFees/{placeId}")]
        public HttpResponseMessage GetTicketFees(int placeId)
        {
            var obj = _repo.GetTicketFeesByPlace(placeId);
            if (obj.Data == null || obj.Data.Count == 0)
                return Request.CreateResponse(HttpStatusCode.OK,
                    Response<List<PlaceTicketFeeDTO>>.Fail(obj.MessageID, obj.MessageDesc));

            return Request.CreateResponse(HttpStatusCode.OK,
                Response<List<PlaceTicketFeeDTO>>.Ok(obj.Data, obj.MessageID, obj.MessageDesc));
        }

        // POST: api/TicketFees/Upsert
        [HttpPost]        
        [Route("UpsertTicketFee")]
        public HttpResponseMessage UpsertTicketFee([FromBody] UpsertPlaceTicketFeeRequest req)
        {
            if (req == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    Response<object>.Fail(0, "Request body is null."));

            var obj = _repo.UpsertTicketFee(req);
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }
    }
}