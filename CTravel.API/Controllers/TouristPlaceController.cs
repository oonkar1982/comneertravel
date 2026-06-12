using CTravel.API.DTO;
using CTravel.API.Filters;
using CTravel.API.Helpers;
using CTravel.API.Models;
using CTravel.API.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CTravel.API.Controllers
{
    //[JwtAuthFilter]
    [ApiKeyAuthFilter]
    [RoutePrefix("api/Places")]
    public class TouristPlaceController : ApiController
    {

        private readonly PlaceRepository _repo = new PlaceRepository();

        [HttpGet]
        [Route("api/place/{StateId}/{CityId}")]
        public HttpResponseMessage GetPlace(int StateId, int CityId)
        {
            var obj = _repo.GetPlace(StateId, CityId); // obj is Response<List<TouristPlaceDTO>>

            if (obj.Data == null)
                return Request.CreateResponse(HttpStatusCode.OK, Response<List<TouristPlaceDTO>>.Fail(obj.MessageID, obj.MessageDesc));
               return Request.CreateResponse(HttpStatusCode.OK, Response<List<TouristPlaceDTO>>.Ok(obj.Data, obj.MessageID, obj.MessageDesc));
        }


        [HttpPost]
        [Route("api/place")]
        public HttpResponseMessage CreatePlace([FromBody] CreateTouristPlaceRequest req)
        {
            var obj = _repo.CreatePlace(req);
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }
        [HttpPut]
        [Route("api/place")]
        public HttpResponseMessage UpdatePlace([FromBody] UpdateTouristPlaceRequest req)
        {
            var obj = _repo.UpdatePlace(req);
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }

        [HttpDelete]
        [Route("api/place/{placeId}/{modifiedBy}")]
        public HttpResponseMessage DeletePlace(int placeId, int modifiedBy)
        {
            var obj = _repo.DeletePlace(placeId, modifiedBy);
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }
    }
}
