using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SEP3_Tier3.Models
{
    public class Training
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public DateTime TimeStamp { get; set; }
        
        [DefaultValue(false)]
        public bool IsCompleted { get; set; }
    }
}