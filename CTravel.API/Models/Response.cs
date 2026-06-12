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
        public T Data { get; set; }

        public static Response<T> Ok(T data, int messageid , string message = "Success")
        {
            return new Response<T> { MessageID = messageid, MessageDesc = message, Data = data };
        }

        public static Response<T> Fail(T data, int messageid, string message)
        {
            return new Response<T> { MessageID = messageid, MessageDesc = message, Data = default(T) };
        }

        public static Response<object> Fail(int messageid, string message)
        {
            return new Response<object> { MessageID = messageid, MessageDesc = message, Data = default(T) };
        }

        public static Response<List<T>> Ok(List<T> list, int messageid, string message)
        {
            return new Response<List<T>> { MessageID = messageid, MessageDesc = message, Data = default(List<T>) };
        }

    

}
}