namespace ServiceManagementAPI.Dtos
{
    public class RevenueData
    {
        public string Date { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int Bookings { get; set; }
    }
}
