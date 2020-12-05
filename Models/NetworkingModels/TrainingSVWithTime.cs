using System;
using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class TrainingSVWithTime : TrainingShortVersion
    {
        [JsonPropertyName("timeStamp")]
        public DateTime TimeStamp { get; set; }
        
        [JsonPropertyName("duration")]
        public int Duration { get; set; }
    }
}