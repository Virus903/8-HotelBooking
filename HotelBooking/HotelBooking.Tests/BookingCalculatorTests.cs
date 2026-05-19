using Xunit;
using HotelBooking.Models;
using HotelBooking.Services;

namespace HotelBooking.Tests
{
    public class BookingCalculatorTests
    {
        private readonly BookingCalculator _calculator;

        public BookingCalculatorTests()
        {
            _calculator = new BookingCalculator();
        }

        // 1. ТЕСТЫ ВАЛИДАЦИИ (4 теста)

        [Fact]
        public void Test01_Calculate_ZeroDays_ReturnsError()
        {
            // Arrange
            var request = new BookingRequest
            {
                RoomType = RoomType.Standard,
                Days = 0,
                Guests = 2,
                Season = Season.Low,
                Breakfast = false,
                SeaView = false,
                PromoCode = null
            };

            // Act
            var result = _calculator.Calculate(request);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public void Test02_Calculate_ZeroGuests_ReturnsError()
        {
            // Arrange
            var request = new BookingRequest
            {
                RoomType = RoomType.Standard,
                Days = 3,
                Guests = 0,
                Season = Season.Low,
                Breakfast = false,
                SeaView = false,
                PromoCode = null
            };

            // Act
            var result = _calculator.Calculate(request);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public void Test03_Calculate_TooManyGuestsForStandard_ReturnsError()
        {
            // Arrange
            var request = new BookingRequest
            {
                RoomType = RoomType.Standard,
                Days = 3,
                Guests = 3,
                Season = Season.Low,
                Breakfast = false,
                SeaView = false,
                PromoCode = null
            };

            // Act
            var result = _calculator.Calculate(request);

            // Assert
            Assert.True(result.IsError);
        }

        [Fact]
        public void Test04_Calculate_TooManyGuestsForLux_ReturnsError()
        {
            // Arrange
            var request = new BookingRequest
            {
                RoomType = RoomType.Lux,
                Days = 3,
                Guests = 5,
                Season = Season.Low,
                Breakfast = false,
                SeaView = false,
                PromoCode = null
            };

            // Act
            var result = _calculator.Calculate(request);

            // Assert
            Assert.True(result.IsError);
        }

        // 2. ТЕСТЫ РАСЧЁТА СТОИМОСТИ (7 тестов)

        [Fact]
        public void Test05_Calculate_StandardRoomNoExtras_ReturnsCorrectTotal()
        {
            // Arrange
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

            // Act
            var result = _calculator.Calculate(request);

            // Assert
            Assert.Equal(7500m, result.Total);
            Assert.Equal(2500m, result.RoomPrice);
            Assert.Equal(0, result.Discount);
            Assert.Equal(0, result.ExtraGuestFee);
            Assert.Equal(0, result.BreakfastFee);
            Assert.Equal(0, result.SeaViewFee);
            Assert.False(result.IsError);
        }

        [Fact]
        public void Test06_Calculate_ComfortRoomWithThreeGuests_ReturnsCorrectTotal()
        {
            // Arrange
            var request = new BookingRequest
            {
                RoomType = RoomType.Comfort,
                Days = 2,
                Guests = 3,
                Season = Season.Low,
                Breakfast = false,
                SeaView = false,
                PromoCode = null
            };

            // Act
            var result = _calculator.Calculate(request);

            // Assert
            // 4000*2 = 8000 (проживание) + 500*2 = 1000 (доплата за 3-го гостя) = 9000
            Assert.Equal(9000m, result.Total);
            Assert.False(result.IsError);
        }

        [Fact]
        public void Test07_Calculate_LuxRoomWithFourGuests_ReturnsCorrectTotal()
        {
            // Arrange
            var request = new BookingRequest
            {
                RoomType = RoomType.Lux,
                Days = 2,
                Guests = 4,
                Season = Season.Low,
                Breakfast = false,
                SeaView = false,
                PromoCode = null
            };

            // Act
            var result = _calculator.Calculate(request);

            // Assert
            // 7000*2 = 14000 (проживание) + (4-2)*1000*2 = 4000 (доплата за гостей) = 18000
            Assert.Equal(18000m, result.Total);
            Assert.False(result.IsError);
        }

        [Fact]
        public void Test08_Calculate_WithBreakfast_ReturnsCorrectTotal()
        {
            // Arrange
            var request = new BookingRequest
            {
                RoomType = RoomType.Standard,
                Days = 2,
                Guests = 2,
                Season = Season.Low,
                Breakfast = true,
                SeaView = false,
                PromoCode = null
            };

            // Act
            var result = _calculator.Calculate(request);

            // Assert
            // 2500*2 = 5000 (проживание) + 2*500*2 = 2000 (завтрак) = 7000
            Assert.Equal(7000m, result.Total);
            Assert.Equal(2000m, result.BreakfastFee);
            Assert.False(result.IsError);
        }

        [Fact]
        public void Test09_Calculate_WithSeaView_ReturnsCorrectTotal()
        {
            // Arrange
            var request = new BookingRequest
            {
                RoomType = RoomType.Standard,
                Days = 2,
                Guests = 2,
                Season = Season.Low,
                Breakfast = false,
                SeaView = true,
                PromoCode = null
            };

            // Act
            var result = _calculator.Calculate(request);

            // Assert
            // 2500*2 = 5000 (проживание) + 1000*2 = 2000 (вид на море) = 7000
            Assert.Equal(7000m, result.Total);
            Assert.Equal(2000m, result.SeaViewFee);
            Assert.False(result.IsError);
        }

        [Fact]
        public void Test10_Calculate_HighSeason_ReturnsCorrectTotal()
        {
            // Arrange
            var request = new BookingRequest
            {
                RoomType = RoomType.Standard,
                Days = 2,
                Guests = 2,
                Season = Season.High,
                Breakfast = false,
                SeaView = false,
                PromoCode = null
            };

            // Act
            var result = _calculator.Calculate(request);

            // Assert
            // 2500*1.25 = 3125 (цена за сутки) * 2 = 6250
            Assert.Equal(6250m, result.Total);
            Assert.False(result.IsError);
        }

        [Fact]
        public void Test11_Calculate_MiddleSeason_ReturnsCorrectTotal()
        {
            // Arrange
            var request = new BookingRequest
            {
                RoomType = RoomType.Standard,
                Days = 2,
                Guests = 2,
                Season = Season.Middle,
                Breakfast = false,
                SeaView = false,
                PromoCode = null
            };

            // Act
            var result = _calculator.Calculate(request);

            // Assert
            // 2500*1.10 = 2750 (цена за сутки) * 2 = 5500
            Assert.Equal(5500m, result.Total);
            Assert.False(result.IsError);
        }


        // 3. ТЕСТЫ ПРОМОКОДОВ (4 теста)

        [Fact]
        public void Test12_Calculate_NoPromoCode_ReturnsNoDiscount()
        {
            // Arrange
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

            // Act
            var result = _calculator.Calculate(request);

            // Assert
            Assert.Equal(0, result.Discount);
            Assert.Equal(7500m, result.Total);
            Assert.False(result.IsError);
        }

        [Fact]
        public void Test13_Calculate_Sale10PromoCode_Returns10PercentDiscount()
        {
            // Arrange
            var request = new BookingRequest
            {
                RoomType = RoomType.Standard,
                Days = 3,
                Guests = 2,
                Season = Season.Low,
                Breakfast = false,
                SeaView = false,
                PromoCode = "SALE10"
            };

            // Act
            var result = _calculator.Calculate(request);

            // Assert
            // 7500 * 0.10 = 750 скидка, итого 6750
            Assert.Equal(750m, result.Discount);
            Assert.Equal(6750m, result.Total);
            Assert.False(result.IsError);
        }

        [Fact]
        public void Test14_Calculate_WelcomePromoCode_Returns1000Discount()
        {
            // Arrange
            var request = new BookingRequest
            {
                RoomType = RoomType.Standard,
                Days = 3,
                Guests = 2,
                Season = Season.Low,
                Breakfast = false,
                SeaView = false,
                PromoCode = "WELCOME"
            };

            // Act
            var result = _calculator.Calculate(request);

            // Assert
            // 7500 - 1000 = 6500
            Assert.Equal(1000m, result.Discount);
            Assert.Equal(6500m, result.Total);
            Assert.False(result.IsError);
        }

        [Fact]
        public void Test15_Calculate_InvalidPromoCode_ReturnsError()
        {
            // Arrange
            var request = new BookingRequest
            {
                RoomType = RoomType.Standard,
                Days = 3,
                Guests = 2,
                Season = Season.Low,
                Breakfast = false,
                SeaView = false,
                PromoCode = "INVALID"
            };

            // Act
            var result = _calculator.Calculate(request);

            // Assert
            Assert.True(result.IsError);
        }

        // 4. КОНТРОЛЬНЫЕ ПРИМЕРЫ ИЗ ЗАДАНИЯ (3 теста)

        [Fact]
        public void Test16_Calculate_Example1_ReturnsExpectedResult()
        {
            // Arrange
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

            // Act
            var result = _calculator.Calculate(request);

            // Assert
            Assert.Equal(7500m, result.Total);
            Assert.Equal(0, result.Discount);
            Assert.Equal(2500m, result.RoomPrice);
            Assert.Equal(0, result.ExtraGuestFee);
            Assert.Equal(0, result.BreakfastFee);
            Assert.Equal(0, result.SeaViewFee);
            Assert.False(result.IsError);
        }

        [Fact]
        public void Test17_Calculate_Example2_ReturnsExpectedResult()
        {
            // Arrange
            var request = new BookingRequest
            {
                RoomType = RoomType.Comfort,
                Days = 2,
                Guests = 3,
                Season = Season.Middle,
                Breakfast = true,
                SeaView = true,
                PromoCode = "SALE10"
            };

            // Act
            var result = _calculator.Calculate(request);

            // Assert
            // Ожидания из задания:
            // TOTAL=14220.00;DISCOUNT=1580.00;ROOMPRICE=4400.00;EXTRAGUEST=1000.00;BREAKFAST=3000.00;SEAVIEW=3000.00
            Assert.Equal(14220m, result.Total);
            Assert.Equal(1580m, result.Discount);
            Assert.Equal(4400m, result.RoomPrice);
            Assert.Equal(1000m, result.ExtraGuestFee);
            Assert.Equal(3000m, result.BreakfastFee);
            Assert.Equal(3000m, result.SeaViewFee);
            Assert.False(result.IsError);
        }

        [Fact]
        public void Test18_Calculate_Example3_ReturnsExpectedResult()
        {
            // Arrange
            var request = new BookingRequest
            {
                RoomType = RoomType.Lux,
                Days = 4,
                Guests = 4,
                Season = Season.High,
                Breakfast = false,
                SeaView = true,
                PromoCode = "WELCOME"
            };

            // Act
            var result = _calculator.Calculate(request);

            // Assert
            // Ожидания из задания:
            // TOTAL=50000.00;DISCOUNT=1000.00;ROOMPRICE=8750.00;EXTRAGUEST=8000.00;BREAKFAST=0.00;SEAVIEW=8000.00
            Assert.Equal(50000m, result.Total);
            Assert.Equal(1000m, result.Discount);
            Assert.Equal(8750m, result.RoomPrice);
            Assert.Equal(8000m, result.ExtraGuestFee);
            Assert.Equal(0, result.BreakfastFee);
            Assert.Equal(8000m, result.SeaViewFee);
            Assert.False(result.IsError);
        }
    }
}