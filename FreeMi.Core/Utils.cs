using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FreeMi.Core
{
    /// <summary>
    /// Utils class
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Hash an input string and return the hash as 32 character hexadecimal string.
        /// </summary>
        /// <param name="input">string</param>
        /// <returns>the hash as 32 character hexadecimal strin</returns>
        public static string GetMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// Gets an exception message
        /// </summary>
        /// <param name="ex">exception</param>
        /// <returns>the exception message</returns>
        public static string GetExceptionMessage(Exception ex)
        {
            string message = ex.Message;
            var innerException = ex.InnerException;
            while (innerException != null)
            {
                message = String.Format("{0} - {1}", message, innerException.Message);
                innerException = innerException.InnerException;
            }
            return message;
        }

        /// <summary>
        /// Write an exception to the trace
        /// </summary>
        /// <param name="ex">exception</param>
        public static void WriteException(Exception ex)
        {
            if (ex != null)
            {
                Trace.WriteLine(GetExceptionMessage(ex));
            }
        }
    }
}