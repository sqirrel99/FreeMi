using OpenSource.UPnP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FreeMi.Core
{
    static class RangeSets
    {
        /// <summary>
        /// Parses 'rangeStr' for HTTP range sets, and adds the sets into
        /// 'rangeSets'... should an overlapping range set be provided
        /// or if an otherwise invalid range is requested, then we clear
        /// the 'rangeSets'... behavior is taken from http://www.freesoft.org/CIE/RFC/2068/178.htm.
        ///
        /// <para>
        /// If the server ignores a byte-range-spec because it is invalid, 
        /// the server should treat the request as if the invalid Range header 
        /// field did not exist. 
        /// (Normally, this means return a 200 response containing the full entity). 
        /// The reason is that the only time a client will make such an invalid 
        /// request is when the entity is smaller than the entity retrieved by a prior request.
        /// [source: http://www.freesoft.org/CIE/RFC/2068/178.htm]
        /// </para>
        /// </summary>
        /// <param name="rangeSets">this ArrayList has range sets added to it</param>
        /// <param name="rangeStr">
        /// This is the HTTP header with the desired ranges.
        /// Text is assumed to be all lower case and trimmed.
        /// </param>
        /// <param name="contentLength">
        /// The entire length of the content, from byte 0.
        /// </param>
        public static void AddRange(this List<HTTPSession.Range> rangeSets, string rangeStr, long contentLength)
        {
            if (String.IsNullOrEmpty(rangeStr))
            {
                return;
            }

            bool errorEncountered = true;

            errorEncountered = false;
            DText dt = new DText();
            dt.ATTRMARK = "=";
            dt.MULTMARK = ",";
            dt.SUBVMARK = "-";
            dt[0] = rangeStr;

            int numSets = dt.DCOUNT(2);

            for (int i = 1; i <= numSets; i++)
            {
                string sOffset = dt[2, i, 1].Trim();
                string sEnd = dt[2, i, 2].Trim();
                long offset = -1, length = -1, end = -1;

                if ((sOffset == "") && (sEnd == ""))
                {
                    // royally screwed up request
                    errorEncountered = true;
                    break;
                }
                else if ((sOffset == "") && (sEnd != ""))
                {
                    // retrieve the last set of bytes identified by sEnd
                    try
                    {
                        offset = 0;
                        end = long.Parse(sEnd);
                        length = end + 1;
                    }
                    catch
                    {
                        errorEncountered = true;
                        break;
                    }
                }
                else if ((sOffset != "") && (sEnd == ""))
                {
                    // retrieve all bytes starting from sOffset
                    try
                    {
                        offset = long.Parse(sOffset);
                        end = contentLength - 1;
                        length = contentLength - offset;
                    }
                    catch
                    {
                        errorEncountered = true;
                        break;
                    }
                }
                else
                {
                    // retrieve bytes from sOffset through sEnd, 
                    // inclusive so be sure to add 1 to difference
                    try
                    {
                        offset = long.Parse(sOffset);
                        end = long.Parse(sEnd);

                        if (offset <= end)
                        {
                            length = end - offset + 1;
                        }
                        else
                        {
                            errorEncountered = true;
                        }
                    }
                    catch
                    {
                        errorEncountered = true;
                        break;
                    }
                }

                if (errorEncountered == false)
                {
                    Debug.Assert(offset >= 0);
                    Debug.Assert(length >= 0);
                    Debug.Assert(end >= 0);

                    HTTPSession.Range newRange = new HTTPSession.Range(offset, length);
                    rangeSets.Add(newRange);
                }
            }

            if (errorEncountered)
            {
                // error parsing value, this is invalid so clear and return
                rangeSets.Clear();
            }
        }
    }
}
