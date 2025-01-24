using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Entities;
using ServiceManagementAPI.Enums;

namespace ServiceManagementAPI.Repositories.CustomerRepository
{
    public interface ICustomerRepository
    {
        Task<bool> UpdateCustomerProfileAsync(string customerId, UpdateCustomerProfileDto updateCustomerProfileDto, Stream imageStream = null!);
        Task<CustomerProfileDto?> GetCustomerProfileAsync(string customerId);
        Task<bool> CreateBookingAsync(BookingDto bookingDto);
        Task<List<ProviderDto>> SearchProvidersAsync(string searchTerm);
        Task<List<NotificationDto>> GetNotificationsByUserIdAsync(string userId);
        Task<bool> ProcessPaymentAsync(SavePaymentDto savePaymentDto);
        Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus status);
        Task<Review> CreateReviewAsync(string userId, int completedServiceId, int rating, string? comment);
        Task<Review?> GetReviewByIdAsync(int reviewId);
        Task<bool> UpdateReviewAsync(int reviewId, int? rating = null, string? comment = null);
        Task<bool> DeleteReviewAsync(int reviewId);
        Task<MessageDto> SendMessageAsync(string senderId, string receiverId, string messageContent);
        Task<List<MessageDto>> GetChatHistoryAsync(string userId1, string userId2);
        Task MarkMessagesAsReadAsync(string senderId, string receiverId);
    }
}
