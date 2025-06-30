using CartonCaps.Domain.Enums;
using CartonCaps.Domain.Entities;
using CartonCaps.Application.Services;
using CartonCaps.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CartonCaps.Application.Repositories;
using CartonCaps.WebAPI.ResponseModels;

namespace CartonCaps.WebAPI.Controllers
{

    /// <summary>
    /// API Controller for managing referrals.
    /// </summary>
    /// <remarks> This controller provides endpoints to retrieve, create and update referrals.</remarks>    
    /// <example> 
    /// GET /api/referral/[id]
    /// POST /api/referral
    /// PATCH /api/referral/[referralId]/status/[status]
    /// </example>
    [ApiController]
    [Route("api/referral/{userId:guid}")]
    public class ReferralController : ControllerBase
    {
        private readonly IReferralRepository _referralRepository;

        public ReferralController(IReferralRepository referralRepository)
        {
            _referralRepository = referralRepository;
        }

        [HttpGet]
        /// <summary> Gets a list of referrals.</summary>
        public async Task<IActionResult> GetReferrals([FromRoute] Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "User ID cannot be empty."
                });
            }

            // Get referrals for the specified user
            try
            {
                var referrals = await _referralRepository.GetReferralsByUserIdAsync(userId);

                if (referrals == null || referrals.Count == 0)
                {
                    return NotFound(new BadRequestResponse() {
                        Message = "No referrals found for this user."
                    });
                }
                else
                {
                    return Ok(new ReferralListResponse()
                    {
                        TotalCount = referrals.Count,
                        Referrals = referrals
                    });
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new BadRequestResponse()
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse() {
                    Details = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateReferral([FromRoute] Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new BadRequestResponse()
                {
                    Message = "User ID cannot be empty."
                });
            }

            try
            {
                var referral = await _referralRepository.AddReferralForUserAsync(userId);

                return Ok(new NewReferralResponse()
                {
                    ReferralId = referral.ReferralId,
                    ShareLink = referral.ReferredDeepLink
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new BadRequestResponse()
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse() {
                    Details = ex.Message
                });
            }
        }

        [Route("~/api/referral/{referralId:guid}/status/{status}")]
        [HttpPatch]
        public async Task<IActionResult> UpdateReferralStatus([FromRoute] Guid referralId, [FromRoute] ReferralStatus status)
        {
            if (referralId == Guid.Empty)
            {
                return BadRequest(new BadRequestResponse()
                {
                    Message = "Referral ID cannot be empty."
                });
            }

            try
            {
                var updatedReferral = await _referralRepository.UpdateReferralStatusAsync(referralId, status);

                if (updatedReferral == null)
                {
                    return NotFound(new BadRequestResponse() {
                        Message = "Referral not found."
                    });
                }

                return Accepted(new ReferralUpdatedResponse()
                {
                    Message = "Referral status updated successfully.",
                    ReferralId = updatedReferral.ReferralId,
                    NewStatus = updatedReferral.Status
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new BadRequestResponse()
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse() {
                    Details = ex.Message
                });
            }
        }

        [Route("~/api/referral/{referralId:guid}/claim/{claimedByUserId:guid}")]
        [HttpPost]
        public async Task<IActionResult> ClaimReferral([FromRoute] Guid referralId, [FromRoute] Guid claimedByUserId)
        {
            if (referralId == Guid.Empty || claimedByUserId == Guid.Empty)
            {
                return BadRequest(new BadRequestResponse()
                {
                    Message = "Referral ID and User ID cannot be empty."
                });
            }

            try
            {
                var updatedReferral = await _referralRepository.ClaimReferralAsync(referralId, claimedByUserId);

                if (updatedReferral == null)
                {
                    return NotFound(new BadRequestResponse() {
                        Message = "Referral not found."
                    });
                }

                return Accepted(new ReferralUpdatedResponse()
                {
                    Message = "Referral claimed successfully.",
                    ReferralId = updatedReferral.ReferralId,
                    NewStatus = updatedReferral.Status
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new BadRequestResponse()
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse() {
                    Details = ex.Message
                });
            }
        }


        [Route("~/api/referral/statuses")]
        [HttpGet]
        public IActionResult GetReferralStatuses()
        {
            // Return all possible referral statuses
            var statuses = Enum.GetValues(typeof(ReferralStatus))
                .Cast<ReferralStatus>()
                .Select(s => new { Name = s.ToString(), Value = (int)s })
                .ToList();

            return Ok(statuses);
        }
    }
}