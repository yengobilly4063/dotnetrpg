using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetrpg.models
{
  public class ServiceResponse<T>
  {
    public T Data { get; set; }
    public bool Success { get; set; } = true;
    public string Message { get; set; } = null;

    public ServiceResponse(T data)
    {
      this.Data = data;
    }

    public ServiceResponse(T data, bool success)
    {
      this.Data = data;
      this.Success = success;
    }

    public ServiceResponse(T data, bool success, string message)
    {
      this.Data = data;
      this.Success = success;
      this.Message = message;
    }

  }

}