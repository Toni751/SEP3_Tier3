using System.Collections.Generic;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class used for sockets communication, containing a request instance along with it's images, if any
    /// </summary>
    public class ActualRequest
    {
        public Request Request { get; set; }
        public List<byte[]> Images { get; set; }
    }
}