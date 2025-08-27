using System.Collections.Generic;

namespace ViscoveryDemoPOS.Domain
{
    public enum RecognizeStatus { Waiting, Confirm, Undetected, Extra }

    public class ProductItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public RecognizeStatus Status { get; set; }
    }

    public class QrOrder
    {
        public string OrderId { get; set; }
        public List<ProductItem> ExpectedItems { get; set; } = new List<ProductItem>();
    }
}
