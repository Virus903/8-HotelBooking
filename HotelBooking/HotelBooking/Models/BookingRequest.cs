namespace HotelBooking.Models
{
    /// <summary>
    /// Запрос на бронирование номера
    /// </summary>
    public class BookingRequest
    {
        public RoomType RoomType { get; set; }
        public int Days { get; set; }
        public int Guests { get; set; }
        public Season Season { get; set; }
        public bool Breakfast { get; set; }
        public bool SeaView { get; set; }
        public string? PromoCode { get; set; }
    }
}