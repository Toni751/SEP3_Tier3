using System.Collections.Generic;

namespace SEP3_Tier3.Models
{
    public class ActualRequest
    {
        public Request Request { get; set; }
        public List<byte[]> Images { get; set; }
    }
}