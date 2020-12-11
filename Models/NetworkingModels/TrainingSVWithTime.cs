using System;
using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing a training short version with the date, time and duration of it
    /// </summary>
    public class TrainingSVWithTime : TrainingShortVersion
    {
        [JsonPropertyName("timeStamp")]
        public DateTime TimeStamp { get; set; }
        
        [JsonPropertyName("duration")]
        public int Duration { get; set; }
    }
}