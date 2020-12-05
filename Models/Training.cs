using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class Training
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Type { get; set; }
        
        [Required]
        public DateTime TimeStamp { get; set; }
        
        [Required]
        public int Duration { get; set; }

        [DefaultValue(false)]
        public bool IsCompleted { get; set; }
        
        public bool IsPublic { get; set; }
        
        public User Owner { get; set; }
    }
}