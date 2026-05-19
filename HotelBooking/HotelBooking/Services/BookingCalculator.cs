using System;
using System.Collections.Generic;
using HotelBooking.Models;

namespace HotelBooking.Services
{
 
    /// Калькулятор стоимости бронирования номера
    
    public class BookingCalculator : IBookingCalculator
    {
        // Базовые цены за сутки по типам номеров
        private readonly Dictionary<RoomType, decimal> _basePrices = new()
        {
            { RoomType.Standard, 2500m },
            { RoomType.Comfort, 4000m },
            { RoomType.Lux, 7000m }
        };

        // Максимальное количество гостей по типам номеров
        private readonly Dictionary<RoomType, int> _maxGuests = new()
        {
            { RoomType.Standard, 2 },
            { RoomType.Comfort, 3 },
            { RoomType.Lux, 4 }
        };

        // Сезонные коэффициенты
        private readonly Dictionary<Season, decimal> _seasonCoefficients = new()
        {
            { Season.Low, 0.00m },    // 0% наценка
            { Season.Middle, 0.10m }, // 10% наценка
            { Season.High, 0.25m }    // 25% наценка
        };

        // Стоимость завтрака за одного гостя в сутки
        private const decimal BreakfastPricePerPersonPerDay = 500m;

        // Стоимость вида на море за сутки по типам номеров
        private readonly Dictionary<RoomType, decimal> _seaViewPrices = new()
        {
            { RoomType.Standard, 1000m },
            { RoomType.Comfort, 1500m },
            { RoomType.Lux, 2000m }
        };

        // Доплата за дополнительного гостя в сутки (для Comfort)
        private const decimal ExtraGuestFeePerDayForComfort = 500m;

        // Доплата за дополнительного гостя в сутки (для Lux)
        private const decimal ExtraGuestFeePerDayForLuxPerGuest = 1000m;

        // Лимит гостей после которого начинается доплата (для Lux)
        private const int ExtraGuestThreshold = 2;

        public BookingResult Calculate(BookingRequest request)
        {
            // ========== 1. ВАЛИДАЦИЯ ==========
            var validationError = ValidateRequest(request);
            if (validationError != null)
            {
                return validationError;
            }

            // ========== 2. РАСЧЁТ ЦЕНЫ ЗА СУТКИ С УЧЁТОМ СЕЗОНА ==========
            decimal basePrice = _basePrices[request.RoomType];
            decimal seasonCoefficient = _seasonCoefficients[request.Season];
            decimal roomPrice = basePrice * (1 + seasonCoefficient);

            // ========== 3. РАСЧЁТ ПРОЖИВАНИЯ ==========
            decimal stayTotal = roomPrice * request.Days;

            // ========== 4. РАСЧЁТ ДОПЛАТЫ ЗА ГОСТЕЙ ==========
            decimal extraGuestFee = CalculateExtraGuestFee(request);

            // ========== 5. РАСЧЁТ ДОПЛАТЫ ЗА ЗАВТРАК ==========
            decimal breakfastFee = CalculateBreakfastFee(request);

            // ========== 6. РАСЧЁТ ДОПЛАТЫ ЗА ВИД НА МОРЕ ==========
            decimal seaViewFee = CalculateSeaViewFee(request);

            // ========== 7. СУММА ДО СКИДКИ ==========
            decimal totalBeforeDiscount = stayTotal + extraGuestFee + breakfastFee + seaViewFee;

            // ========== 8. РАСЧЁТ СКИДКИ ==========
            decimal discount = CalculateDiscount(request.PromoCode, totalBeforeDiscount);

            // ========== 9. ПРОВЕРКА НА НЕВЕРНЫЙ ПРОМОКОД ==========
            if (discount < 0)
            {
                return new BookingResult { IsError = true, ErrorMessage = "Invalid promo code" };
            }

            // ========== 10. ПРИМЕНЕНИЕ СКИДКИ ==========
            discount = Math.Min(discount, totalBeforeDiscount);
            decimal total = totalBeforeDiscount - discount;

            // ========== 11. ФОРМИРОВАНИЕ РЕЗУЛЬТАТА ==========
            return new BookingResult
            {
                Total = total,
                Discount = discount,
                RoomPrice = roomPrice,
                ExtraGuestFee = extraGuestFee,
                BreakfastFee = breakfastFee,
                SeaViewFee = seaViewFee,
                IsError = false
            };
        }

        
        /// Проверка корректности входных данных
        
        private BookingResult? ValidateRequest(BookingRequest request)
        {
            // Проверка количества дней
            if (request.Days <= 0)
            {
                return new BookingResult { IsError = true, ErrorMessage = "Days must be greater than 0" };
            }

            // Проверка количества гостей
            if (request.Guests <= 0)
            {
                return new BookingResult { IsError = true, ErrorMessage = "Guests must be greater than 0" };
            }

            // Проверка типа номера (автоматически через enum)
            if (!Enum.IsDefined(typeof(RoomType), request.RoomType))
            {
                return new BookingResult { IsError = true, ErrorMessage = "Invalid room type" };
            }

            // Проверка сезона (автоматически через enum)
            if (!Enum.IsDefined(typeof(Season), request.Season))
            {
                return new BookingResult { IsError = true, ErrorMessage = "Invalid season" };
            }

            // Проверка максимального количества гостей для типа номера
            int maxGuests = _maxGuests[request.RoomType];
            if (request.Guests > maxGuests)
            {
                return new BookingResult { IsError = true, ErrorMessage = $"Room {request.RoomType} can accommodate max {maxGuests} guests" };
            }

            return null;
        }

        
        /// Расчёт доплаты за дополнительных гостей
        
        private decimal CalculateExtraGuestFee(BookingRequest request)
        {
            // Standard: дополнительные гости запрещены (уже проверено в валидации)
            if (request.RoomType == RoomType.Standard)
            {
                return 0;
            }

            // Comfort: если гостей больше 2, доплата 500 руб/сутки
            if (request.RoomType == RoomType.Comfort && request.Guests > 2)
            {
                return ExtraGuestFeePerDayForComfort * request.Days;
            }

            // Lux: за каждого гостя сверх 2 доплата 1000 руб/сутки
            if (request.RoomType == RoomType.Lux && request.Guests > ExtraGuestThreshold)
            {
                int extraGuests = request.Guests - ExtraGuestThreshold;
                return extraGuests * ExtraGuestFeePerDayForLuxPerGuest * request.Days;
            }

            return 0;
        }

        
        /// Расчёт доплаты за завтрак
       
        private decimal CalculateBreakfastFee(BookingRequest request)
        {
            if (!request.Breakfast)
            {
                return 0;
            }

            return request.Guests * BreakfastPricePerPersonPerDay * request.Days;
        }

        
        /// Расчёт доплаты за вид на море
        
        private decimal CalculateSeaViewFee(BookingRequest request)
        {
            if (!request.SeaView)
            {
                return 0;
            }

            return _seaViewPrices[request.RoomType] * request.Days;
        }

       
        /// Расчёт скидки по промокоду
        
        private decimal CalculateDiscount(string? promoCode, decimal totalBeforeDiscount)
        {
            // Без промокода
            if (string.IsNullOrEmpty(promoCode))
            {
                return 0;
            }

            // Проверка промокодов
            if (promoCode == "SALE10")
            {
                return totalBeforeDiscount * 0.10m;  // 10% скидка
            }

            if (promoCode == "WELCOME")
            {
                return 1000m;  // 1000 руб скидка
            }

            // Неверный промокод - возвращаем -1 (означает ошибку)
            return -1;
        }
    }
}