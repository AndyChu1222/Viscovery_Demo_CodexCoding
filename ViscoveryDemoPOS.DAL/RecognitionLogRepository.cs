using System;
using System.IO;
using ViscoveryDemoPOS.Domain;

namespace ViscoveryDemoPOS.DAL
{
    /// <summary>
    /// Simple file-based implementation of <see cref="IRecognitionLogRepository"/>
    /// that appends recognition results to a CSV file.
    /// </summary>
    public class RecognitionLogRepository : IRecognitionLogRepository
    {
        /// <summary>Path to the log file.</summary>
        private readonly string _file;

        /// <summary>
        /// Creates the repository and ensures the log file exists with headers.
        /// </summary>
        /// <param name="file">Destination CSV file.</param>
        public RecognitionLogRepository(string file = "recognition_log.csv")
        {
            _file = file;
            if (!File.Exists(_file))
            {
                File.WriteAllText(_file, "Time,OrderId,Code,Name,Status\n");
            }
        }

        /// <summary>
        /// Appends a recognition result to the CSV log with a timestamp.
        /// </summary>
        /// <param name="orderId">Identifier of the processed order.</param>
        /// <param name="item">Product item and its recognition status.</param>
        public void Log(string orderId, ProductItem item)
        {
            var line = string.Format("{0},{1},{2},{3},{4}\n", DateTime.UtcNow.ToString("o"), orderId, item.Code, item.Name, item.Status);
            File.AppendAllText(_file, line);
        }
    }
}
