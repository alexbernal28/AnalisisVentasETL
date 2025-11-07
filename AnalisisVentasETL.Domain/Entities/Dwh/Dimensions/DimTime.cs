namespace AnalisisVentasETL.Domain.Entities.Dwh.Dimensions
{
    public class DimTime
    {
        public int TimeKey { get; set; }
        public DateTime Date { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Quarter { get; set; }
        public string MonthName { get; set; } = string.Empty;
    }
}
