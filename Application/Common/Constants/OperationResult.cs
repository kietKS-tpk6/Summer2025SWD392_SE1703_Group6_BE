using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Constants
{
    public class OperationResult<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public static OperationResult<T> Ok(T data, string? message = null) =>
            new() { Success = true, Data = data, Message = message };

        public static OperationResult<T> Fail(string message) =>
            new() { Success = false, Message = message };

        //public static OperationResult<T> Fail(string message)
        //{
        //    Console.WriteLine($"Fail called with message:{message}"); // debug
        //    return new OperationResult<T> { Success = false, Message = message };
        //}
    }

}
