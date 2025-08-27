using System;
using System.IO;
using ViscoveryDemoPOS.Domain;

namespace ViscoveryDemoPOS.DAL
{
    public class RecognitionLogRepository : IRecognitionLogRepository
    {
        private readonly string _file;

        public RecognitionLogRepository(string file = "recognition_log.csv")
        {
            _file = file;
            if (!File.Exists(_file))
            {
                File.WriteAllText(_file, "Time,OrderId,Code,Name,Status\n");
            }
        }

        public void Log(string orderId, ProductItem item)
        {
            var line = string.Format("{0},{1},{2},{3},{4}\n", DateTime.UtcNow.ToString("o"), orderId, item.Code, item.Name, item.Status);
            File.AppendAllText(_file, line);
        }
    }
}
