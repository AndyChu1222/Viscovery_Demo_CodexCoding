using ViscoveryDemoPOS.Domain;

namespace ViscoveryDemoPOS.DAL
{
    /// <summary>
    /// Abstraction for persisting recognition results to an underlying store.
    /// </summary>
    public interface IRecognitionLogRepository
    {
        /// <summary>
        /// Writes a log entry for the specified product in an order.
        /// </summary>
        /// <param name="orderId">Identifier of the order.</param>
        /// <param name="item">Product item to log.</param>
        void Log(string orderId, ProductItem item);
    }
}
