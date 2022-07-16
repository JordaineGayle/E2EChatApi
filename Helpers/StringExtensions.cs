using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E2ECHATAPI.Helpers
{
    public static class StringExtensions
    {
        /// <summary>
        /// Check is a string is null, empty or contains only whitespaces
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsNullOrEmptyWhitespace(this string val) => string.IsNullOrWhiteSpace(val);

        /// <summary>
        /// Checkes if a string is null or empty
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string val) => string.IsNullOrEmpty(val);

        /// <summary>
        /// Checks the equality of two strings irrespective of the case
        /// </summary>
        /// <param name="val"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string val, string other) 
            => string.Equals(val, other, StringComparison.OrdinalIgnoreCase);
    }
}
