using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Host.Model
{
    public class HttpResultModelJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HttpResultModel);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            while (true)
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Comment:
                        if (!reader.Read()) return null;
                        else continue;
                    case JsonToken.None:
                    case JsonToken.Null:
                    case JsonToken.Undefined:
                        return null;
                }
                if (reader.TokenType == JsonToken.StartObject) break;
                else throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Unexpected token when converting HttpResultModel: {0}", reader.TokenType));
            }

            var r = new HttpResultModel();
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Comment:
                        break;
                    case JsonToken.PropertyName:
                        read_prop();
                        if (r.IsSuccess == true) return r;
                        break;
                    case JsonToken.EndObject:
                        return r;
                }
            }
            throw new JsonSerializationException("Unexpected end when reading.");

            void read_prop()
            {
                var propertyName = reader.Value.ToString();
                switch (1)
                {
                    case 1 when (r.IsSuccess == null && string.Equals(propertyName, "issuccess", StringComparison.OrdinalIgnoreCase)):
                    case 1 when (r.IsSuccess == null && string.Equals(propertyName, "isok", StringComparison.OrdinalIgnoreCase)):
                    case 1 when (r.IsSuccess == null && string.Equals(propertyName, "succeed", StringComparison.OrdinalIgnoreCase)):
                        r.IsSuccess = reader.ReadAsBoolean();
                        return;
                    case 1 when (r.IsSuccess == null && string.Equals(propertyName, "status", StringComparison.OrdinalIgnoreCase)):
                        {
                            var code = reader.ReadAsInt32();
                            r.IsSuccess = code == 200;
                        }
                        return;
                    case 1 when (r.IsSuccess == null && string.Equals(propertyName, "code", StringComparison.OrdinalIgnoreCase)):
                        {
                            var code = reader.ReadAsInt32();
                            r.IsSuccess = code == 0 || code == 200;
                        }
                        return;
                    case 1 when (r.IsSuccess == null && string.Equals(propertyName, "errcode", StringComparison.OrdinalIgnoreCase)):
                    case 1 when (r.IsSuccess == null && string.Equals(propertyName, "errno", StringComparison.OrdinalIgnoreCase)):
                        {
                            var code = reader.ReadAsInt32();
                            r.IsSuccess = code == 0;
                        }
                        return;
                    case 1 when (string.Equals(propertyName, "msg", StringComparison.OrdinalIgnoreCase)):
                    case 1 when (string.Equals(propertyName, "errormsg", StringComparison.OrdinalIgnoreCase)):
                    case 1 when (string.Equals(propertyName, "errmsg", StringComparison.OrdinalIgnoreCase)):
                    case 1 when (string.Equals(propertyName, "errormessage", StringComparison.OrdinalIgnoreCase)):
                        r.ErrorMsg = reader.ReadAsString();
                        return;
                }
                if (!reader.Read())
                {
                    throw new JsonSerializationException("Unexpected end when reading.");
                }
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}
