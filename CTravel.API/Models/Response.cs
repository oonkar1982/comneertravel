using CTravel.API.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTravel.API.Models
{


    public class Response<T>
    {
        public int MessageID { get; set; }
        public string MessageDesc { get; set; }
        public T Data { get; set; } = default(T);

        public static Response<T> Ok(T data, int messageid , string message = "Success")
        {
            return new Response<T> { MessageID = messageid, MessageDesc = message, Data = data };
        }

       

        public static Response<T> Fail(int messageID, string messageDesc)
        {
            return new Response<T> { MessageID = messageID, MessageDesc = messageDesc  };
        }
    }
}