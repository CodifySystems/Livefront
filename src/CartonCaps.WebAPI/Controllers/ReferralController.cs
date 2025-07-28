using CartonCaps.Domain.Enums;
using CartonCaps.Domain.Entities;
using CartonCaps.Application.Services;
using CartonCaps.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CartonCaps.Application.Repositories;
using CartonCaps.WebAPI.ResponseModels;
using CartonCaps.Application.Common.Exceptions;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferralController"/> class.
        /// </summary>
        /// <param name="referralRepository"></param>
        public ReferralController(IReferralRepository referralRepository)
        {
            _referralRepository = referralRepository;
        }

        /// <summary> Gets a list of referrals.</summary>
        [HttpGet]
        [ProducesDefaultResponseType(typeof(ReferralListResponse))]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(typeof(BadRequestResponse), 404)]
        public async Task<IActionResult> GetReferrals([FromRoute] Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new BadRequestResponse()
                {
                    Message = "User ID cannot be empty."
                });
            }

            // Get referrals for the specified user
            try
            {
                var referrals = await _referralRepository.GetReferralsByUserIdAsync(userId);

                if (referrals == null || referrals.Count == 0)
                {
                    return NotFound(new BadRequestResponse()
                    {
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
            catch (NotFoundException ex)
            {
                return NotFound(new BadRequestResponse()
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse()
                {
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Creates a new referral for a user.
        /// </summary>
        /// <param name="userId">User ID to create referral for</param>
        /// <returns>IActionResult</returns>
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

                if (referral == null)
                {
                    return NotFound(new BadRequestResponse()
                    {
                        Message = "Referral could not be created."
                    });
                }

                return Ok(new NewReferralResponse()
                {
                    ReferralId = referral.ReferralId,
                    ShareLink = referral.ReferredDeepLink
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse()
                {
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Updates the status of a referral.
        /// </summary>
        /// <param name="referralId"></param>
        /// <param name="status"></param>
        /// <returns>IActionResult for the API call.</returns>
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
            catch (BadRequestException ex)
            {
                return BadRequest(new BadRequestResponse()
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse()
                {
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Claims a referral for a user, marking it as claimed.
        /// </summary>
        /// <param name="referralId"></param>
        /// <param name="claimedByUserId"></param>
        /// <returns>IActionResult with the API result data.</returns>
        [Route("~/api/referral/{referralId:guid}/claim/{claimedByUserId:guid}")]
        [HttpPost]
        public async Task<IActionResult> ClaimReferral([FromRoute] Guid referralId, [FromRoute] Guid claimedByUserId)
        {
            if (referralId == Guid.Empty || claimedByUserId == Guid.Empty)
            {
                return BadRequest(new BadRequestResponse()
                {
                    Message = "Referral ID and Claimed by User ID cannot be empty."
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
            catch (BadRequestException ex)
            {
                return BadRequest(new BadRequestResponse()
                {
                    Message = ex.Message
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new BadRequestResponse()
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse()
                {
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Gets all possible referral statuses.
        /// </summary>
        /// <returns>IActionResult with the list of referral statuses.</returns>
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