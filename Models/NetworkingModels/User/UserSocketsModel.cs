using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing a sockets version of the user model
    /// </summary>
    public class UserSocketsModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }
        
        [JsonPropertyName("score")]
        public int Score { get; set; }
        
        [JsonPropertyName("address")]
        public Address Address { get; set; }
        
        [JsonIgnore]
        [JsonPropertyName("avatar")]
        public byte[] Avatar { get; set; }
        
        [JsonIgnore]
        [JsonPropertyName("profileBackground")]
        public byte[] ProfileBackground { get; set; }
        
        // 0: isFriend, 1: isYouSentHimFriendRequest, 2: isYouReportedHim, 3: isHeSharedTrainingsWithYou,
        // 4: isHeSharedDietsWithYou,5: isYouFollowPage, 6: isHeSentYouFriendRequest, 7:hasUserRatedPage
        [JsonPropertyName("userStatus")]
        public bool[] UserStatus { get; set; }
        
        [JsonPropertyName("relevantFriendsNumber")]
        public int RelevantFriendsNumber { get; set; }
        
        [JsonPropertyName("rating")]
        public int Rating { get; set; }
    }
}