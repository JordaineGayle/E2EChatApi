using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E2ECHATAPI.Helpers
{
    public static class Contracts
    {
        /// <summary>
        /// Requires that the specified value is not null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <param name="msg"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void RequiresNotNull<T>(this T val, string msg)
        {
            if (val == null)
                throw new ArgumentNullException(msg ?? $"{typeof(T)} is required.");
        }

        /// <summary>
        /// Checks if a string null, empty or contains only whitespace and throws an exception if that's the case.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static string EnsureNotNullOrEmpty(this string val, string msg = null)
        {
            if (string.IsNullOrWhiteSpace(val))
                throw new InvalidOperationException(msg ?? "item is required.");
            return val;
        }

        /// <summary>
        /// Checks if an entity is null and throws an exception if that's the case
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static T EnsureNotNull<T>(this T val, string msg = null)
        {
            if (val == null)
                throw new InvalidOperationException(msg ?? $"{typeof(T)} is required.");
            return val;
        }


    }
}
