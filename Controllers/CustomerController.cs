using Microsoft.AspNetCore.Mvc;
using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Enums;
using ServiceManagementAPI.Services.CustomerService;

namespace ServiceManagementAPI.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _userService;

        public CustomerController(ICustomerService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile/{customerId}")]
        public async Task<IActionResult> GetCustomerProfile(string customerId)
        {
            var customerProfile = await _userService.GetCustomerProfileAsync(customerId);

            if (customerProfile == null)
            {
                return NotFound(new { message = "Customer not found" });
            }

            return Ok(customerProfile);
        }

        [HttpPut("profile/{customerId}")]
        public async Task<IActionResult> UpdateCustomerProfile(string customerId, [FromForm] UpdateCustomerProfileDto updateCustomerProfileDto, IFormFile? imageFile = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Stream? imageStream = null;

            if (imageFile != null)
            {
                imageStream = imageFile.OpenReadStream();
            }

            var result = await _userService.UpdateCustomerProfileAsync(customerId, updateCustomerProfileDto, imageStream!);

            if (!result)
            {
                return NotFound(new { message = "Customer not found" });
            }

            return Ok(new { message = "Profile updated successfully" });
        }

        [HttpPost("bookings")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDto bookingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bookingSuccess = await _userService.CreateBookingAsync(bookingDto);
            return CreatedAtAction(nameof(CreateBooking), new { message = "Booking created successfully." });
        }

        [HttpGet("search-providers")]
        public async Task<IActionResult> SearchProviders([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(new { message = "Search term cannot be empty" });
            }

            var providers = await _userService.SearchProvidersAsync(searchTerm);

            if (providers == null || !providers.Any())
            {
                return NotFound(new { message = "No providers found matching the search term" });
            }

            return Ok(providers);
        }

        [HttpGet("notifications/{userId}")]
        public async Task<IActionResult> GetNotificationsByUserId(string userId)
        {
            var notifications = await _userService.GetNotificationsByUserIdAsync(userId);

            if (notifications == null || !notifications.Any())
            {
                return NotFound(new { message = "No notifications found" });
            }

            return Ok(notifications);
        }

        [HttpPost("payments")]
        public async Task<IActionResult> SavePaymentAsync([FromBody] SavePaymentDto savePaymentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var paymentSaved = await _userService.ProcessPaymentAsync(savePaymentDto);

            if (!paymentSaved)
            {
                return BadRequest(new { message = "Payment processing failed" });
            }

            return Ok(new { message = "Payment saved successfully" });
        }

        [HttpPut("bookings/{bookingId}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int bookingId, [FromBody] BookingStatus bookingStatus)
        {
            if (!Enum.IsDefined(typeof(BookingStatus), bookingStatus))
            {
                return BadRequest(new { message = "Invalid booking status" });
            }

            var result = await _userService.UpdateBookingStatusAsync(bookingId, bookingStatus);

            if (!result)
            {
                return NotFound(new { message = "Booking not found or status update failed" });
            }

            return Ok(new { message = "Booking status updated successfully" });
        }

        [HttpGet("reviews/{reviewId}")]
        public async Task<IActionResult> GetReviewById(int reviewId)
        {
            var review = await _userService.GetReviewByIdAsync(reviewId);

            if (review == null)
            {
                return NotFound(new { message = "Review not found" });
            }

            return Ok(review);
        }

        [HttpPost("reviews")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto createReviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var review = await _userService.CreateReviewAsync(
                createReviewDto.UserId,
                createReviewDto.CompletedServiceId,
                createReviewDto.Rating,
                createReviewDto.Comment
            );

            var responseDto = new
            {
                Id = review.Id,
                UserId = review.UserId,
                CompletedServiceId = review.CompletedServiceId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                CompletedService = new
                {
                    Id = review.CompletedService.Id,
                    Description = review.CompletedService.Description,
                    CompletedAt = review.CompletedService.CompletedAt
                }
            };

            return CreatedAtAction(nameof(GetReviewById), new { reviewId = review.Id }, responseDto);
        }

        [HttpPut("reviews/{reviewId}")]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] UpdateReviewDto updateReviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.UpdateReviewAsync(reviewId, updateReviewDto.Rating, updateReviewDto.Comment);

            if (!result)
            {
                return NotFound(new { message = "Review not found or update failed" });
            }

            return Ok(new { message = "Review updated successfully" });
        }

        [HttpDelete("reviews/{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var result = await _userService.DeleteReviewAsync(reviewId);

            if (!result)
            {
                return NotFound(new { message = "Review not found" });
            }

            return Ok(new { message = "Review deleted successfully" });
        }

        [HttpPost("messages")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto sendMessageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var message = await _userService.SendMessageAsync(sendMessageDto.SenderId, sendMessageDto.ReceiverId, sendMessageDto.MessageContent);

            if (message == null)
            {
                return BadRequest(new { message = "Failed to send message." });
            }

            return Ok(message);
        }

        [HttpGet("messages/history")]
        public async Task<IActionResult> GetChatHistory([FromQuery] string userId1, [FromQuery] string userId2)
        {
            if (string.IsNullOrWhiteSpace(userId1) || string.IsNullOrWhiteSpace(userId2))
            {
                return BadRequest(new { message = "Both user IDs are required." });
            }

            var chatHistory = await _userService.GetChatHistoryAsync(userId1, userId2);

            if (chatHistory == null || !chatHistory.Any())
            {
                return NotFound(new { message = "No chat history found." });
            }

            return Ok(chatHistory);
        }

        [HttpPut("messages/mark-as-read")]
        public async Task<IActionResult> MarkMessagesAsRead([FromQuery] string senderId, [FromQuery] string receiverId)
        {
            if (string.IsNullOrWhiteSpace(senderId) || string.IsNullOrWhiteSpace(receiverId))
            {
                return BadRequest(new { message = "Both senderId and receiverId are required." });
            }

            await _userService.MarkMessagesAsReadAsync(senderId, receiverId);

            return Ok(new { message = "Messages marked as read." });
        }

        [HttpGet("messages/unread-count/{userId}")]
        public async Task<IActionResult> GetUnreadMessageCount(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(new { message = "User ID is required." });
            }

            var unreadCount = await _userService.GetUnreadMessageCountAsync(userId);

            return Ok(new { unreadCount });
        }
    }
}
