using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeMi.Core
{
    /// <summary>
    /// HttpUtility class
    /// </summary>
    static class HttpUtility
    {
        /// <summary>
        /// UrlDecodes a string without requiring System.Web
        /// </summary>
        /// <param name="text">String to decode.</param>
        /// <returns>decoded string</returns>
        public static string UrlDecode(string text)
        {
            // pre-process for + sign space formatting since System.Uri doesn't handle it
            // plus literals are encoded as %2b normally so this should be safe
            return Uri.UnescapeDataString(text.Replace("+", " "));
        }
    }
}
