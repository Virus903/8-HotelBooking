using HotelBooking.Models;

namespace HotelBooking.Services
{
    /// <summary>
    /// Интерфейс калькулятора бронирования
    /// </summary>
    public interface IBookingCalculator
    {
        BookingResult Calculate(BookingRequest request);
    }
}