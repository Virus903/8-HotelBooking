using System;
using HotelBooking.Models;
using HotelBooking.Services;

namespace Program.cs
{
    class Program
    {
        static void Main(string[] args)
        {
            var calculator = new BookingCalculator();

            var request = new BookingRequest
            {
                RoomType = RoomType.Standard,
                Days = 3,
                Guests = 2,
                Season = Season.Low,
                Breakfast = false,
                SeaView = false,
                PromoCode = null
            };

            var result = calculator.Calculate(request);
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}