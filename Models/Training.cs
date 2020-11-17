using System;

namespace SEP3_Tier3.Models
{
    public class Training
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsCompleted { get; set; }
    }
}