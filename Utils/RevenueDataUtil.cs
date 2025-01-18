using ServiceManagementAPI.Entities;
using ServiceManagementAPI.Enums;

namespace ServiceManagementAPI.Utils
{
    public static class RevenueDataUtil
    {
        public static List<Dtos.RevenueData> GetMonthlyRevenueData(List<Service> services)
        {
            var months = new List<Dtos.RevenueData>();
            for (int i = 5; i >= 0; i--)
            {
                var monthStart = DateTime.UtcNow.AddMonths(-i).ToString("yyyy-MM");
                var monthEnd = DateTime.UtcNow.AddMonths(-i + 1).ToString("yyyy-MM");

                var monthRevenue = services
                    .SelectMany(s => s.Bookings)
                    .Where(b => b.BookingStatus == (int)BookingStatus.Confirmed &&
                                b.CreatedAt.HasValue &&
                                b.CreatedAt.Value >= DateTime.Parse(monthStart) &&
                                b.CreatedAt.Value < DateTime.Parse(monthEnd))
                    .Sum(b => b.Payments.Sum(p => p.ReleasedAmount ?? 0));

                var monthBookings = services
                    .SelectMany(s => s.Bookings)
                    .Count(b => b.BookingStatus == (int)BookingStatus.Confirmed &&
                                b.CreatedAt.HasValue &&
                                b.CreatedAt.Value >= DateTime.Parse(monthStart) &&
                                b.CreatedAt.Value < DateTime.Parse(monthEnd));

                months.Add(new Dtos.RevenueData
                {
                    Date = monthStart,
                    Revenue = monthRevenue,
                    Bookings = monthBookings
                });
            }

            return months;
        }

    }
}
