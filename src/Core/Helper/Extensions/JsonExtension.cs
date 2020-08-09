using System;
using Newtonsoft.Json;

namespace Helper.Extensions
{
    public static class JsonExtension
    {
        public static string ToJson(this object obj, JsonSerializerSettings settings = null)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return JsonConvert.SerializeObject(obj, settings);
        }

        public static T FromJson<T>(this string obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return JsonConvert.DeserializeObject<T>(obj);
        }
    }
}