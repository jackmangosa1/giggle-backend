namespace ServiceManagementAPI.Utils
{
    public static class EnumConverter
    {
        public static string GetPaymentStatus(int paymentStatus)
        {
            return paymentStatus switch
            {
                0 => "Pending",
                1 => "Completed",
                2 => "Failed",
                _ => "Unknown"
            };
        }

        public static string GetBookingStatus(int bookingStatus)
        {
            return bookingStatus switch
            {
                0 => "Pending",
                1 => "Approved",
                2 => "Rejected",
                3 => "Completed",
                4 => "Confirmed",
                _ => "Unknown"
            };
        }
    }
}
