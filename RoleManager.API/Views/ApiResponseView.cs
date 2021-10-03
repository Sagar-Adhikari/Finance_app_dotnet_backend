using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoleManager.API.Views
{
    public class ApiError
    {

        public static JsonSerializerSettings ApiErrorJsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };

        private string _message;

        public ApiError(string msg)
        {
            if (msg == null) throw new ArgumentNullException("Error message can not be null.");
            this._message = msg;
        }

        public string Error { get { return this._message; } }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, ApiErrorJsonSerializerSettings);
        }
    }

    public class ApiResult
    {
        private object _data;

        public ApiResult(object data)
        {
            this._data = data;
        }

        public object Data { get { return this._data; } }
    }
}
