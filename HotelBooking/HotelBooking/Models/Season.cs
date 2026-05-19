namespace HotelBooking.Models
{
    /// <summary>
    /// Сезонность (влияет на наценку)
    /// </summary>
    public enum Season
    {
        Low = 0,      // 0% наценка
        Middle = 1,   // 10% наценка
        High = 2      // 25% наценка
    }
}