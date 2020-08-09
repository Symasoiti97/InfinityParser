using System;
using System.Collections.Generic;

namespace Helper.Extensions
{
    public static class TypeExtension
    {
        public static Type ToEnumerableType(this Type type)
        {
            return typeof(IEnumerable<>).MakeGenericType(type);
        }
    }
}