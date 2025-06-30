using System.Text.Json.Serialization;
using CartonCaps.Domain.Entities;

namespace CartonCaps.WebAPI.ResponseModels
{
    public class ReferralListResponse : BaseResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferralListResponse"/> class.
        /// Sets the default status to "Success".
        /// </summary>
        public ReferralListResponse()
        {
            Status = "Success";
        }        

        /// <summary>
        /// Gets or sets the total count of referrals.
        /// </summary>
        [JsonPropertyOrder(1)]
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the list of referrals.
        /// </summary>
        [JsonPropertyOrder(2)]
        public required List<Referral> Referrals { get; set; } 
    }
}