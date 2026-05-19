using HotelBooking.Models;

namespace HotelBooking.Services
{
   
    /// Интерфейс калькулятора бронирования
    
    public interface IBookingCalculator
    {
        BookingResult Calculate(BookingRequest request);
    }
}