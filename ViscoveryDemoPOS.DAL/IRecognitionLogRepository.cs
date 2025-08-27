using ViscoveryDemoPOS.Domain;

namespace ViscoveryDemoPOS.DAL
{
    public interface IRecognitionLogRepository
    {
        void Log(string orderId, ProductItem item);
    }
}
