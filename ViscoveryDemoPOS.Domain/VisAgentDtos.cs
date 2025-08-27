using System.Collections.Generic;

namespace ViscoveryDemoPOS.Domain
{
    public class UnifiedRecognitionResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public UnifiedData data { get; set; }
    }

    public class UnifiedData
    {
        public UnifiedOrder order { get; set; }
    }

    public class UnifiedOrder
    {
        public List<Plate> plates { get; set; }
    }

    public class Plate
    {
        public List<Instance> instances { get; set; }
    }

    public class Instance
    {
        public Product product { get; set; }
    }

    public class Product
    {
        public string product_code { get; set; }
        public string product_name { get; set; }
    }

    public class CheckoutItem
    {
        public string product_code { get; set; }
        public string product_name { get; set; }
        public List<CheckoutItem> combo_data { get; set; }
    }
}
