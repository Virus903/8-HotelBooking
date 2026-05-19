namespace HotelBooking.Models
{
    /// <summary>
    /// Результат расчёта бронирования
    /// </summary>
    public class BookingResult
    {
        public decimal Total { get; set; }           // Итоговая сумма
        public decimal Discount { get; set; }        // Скидка
        public decimal RoomPrice { get; set; }       // Цена за сутки с учётом сезона
        public decimal ExtraGuestFee { get; set; }   // Доплата за гостей
        public decimal BreakfastFee { get; set; }    // Доплата за завтрак
        public decimal SeaViewFee { get; set; }      // Доплата за вид на море
        public bool IsError { get; set; }            // Флаг ошибки
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Преобразует результат в строку для вывода
        /// </summary>
        public override string ToString()
        {
            if (IsError)
            {
                return "ERROR";
            }

            return $"TOTAL={Total:F2};DISCOUNT={Discount:F2};ROOMPRICE={RoomPrice:F2};" +
                   $"EXTRAGUEST={ExtraGuestFee:F2};BREAKFAST={BreakfastFee:F2};SEAVIEW={SeaViewFee:F2}";
        }
    }
}