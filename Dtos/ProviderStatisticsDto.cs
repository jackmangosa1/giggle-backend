namespace ServiceManagementAPI.Dtos
{
    public class ProviderStatisticsDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
        public double RevenueGrowthPercentage { get; set; }
        public List<RevenueData> RevenueData { get; set; } = new List<RevenueData>();
    }
}
