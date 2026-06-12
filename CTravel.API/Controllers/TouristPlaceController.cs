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

    }
}
