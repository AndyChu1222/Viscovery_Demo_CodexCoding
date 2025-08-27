using System.Collections.Generic;

namespace ViscoveryDemoPOS.Domain
{
    /// <summary>
    /// Root object returned by the VisAgent unified recognition endpoint.
    /// </summary>
    public class UnifiedRecognitionResponse
    {
        /// <summary>HTTP-like status code returned by VisAgent.</summary>
        public int code { get; set; }

        /// <summary>Description of the status.</summary>
        public string message { get; set; }

        /// <summary>Actual recognition data.</summary>
        public UnifiedData data { get; set; }
    }

    /// <summary>
    /// Container for recognition data returned by VisAgent.
    /// </summary>
    public class UnifiedData
    {
        /// <summary>Order information including recognized plates.</summary>
        public UnifiedOrder order { get; set; }
    }

    /// <summary>
    /// Represents a VisAgent order that may contain multiple plates.
    /// </summary>
    public class UnifiedOrder
    {
        /// <summary>Collection of plates detected in the image.</summary>
        public List<Plate> plates { get; set; }
    }

    /// <summary>
    /// A plate is a container for recognized product instances.
    /// </summary>
    public class Plate
    {
        /// <summary>Items detected on the plate.</summary>
        public List<Instance> instances { get; set; }
    }

    /// <summary>
    /// Represents a single detection instance returned by VisAgent.
    /// </summary>
    public class Instance
    {
        /// <summary>Product information associated with the detection.</summary>
        public Product product { get; set; }
    }

    /// <summary>
    /// Minimal product information returned by VisAgent.
    /// </summary>
    public class Product
    {
        /// <summary>Product code provided by VisAgent.</summary>
        public string product_code { get; set; }

        /// <summary>Product name provided by VisAgent.</summary>
        public string product_name { get; set; }
    }

    /// <summary>
    /// Data structure for items received from the POS system checkout callback.
    /// </summary>
    public class CheckoutItem
    {
        /// <summary>Product code returned by the POS system.</summary>
        public string product_code { get; set; }

        /// <summary>Product name returned by the POS system.</summary>
        public string product_name { get; set; }

        /// <summary>When the item is a combo, additional nested items may be provided.</summary>
        public List<CheckoutItem> combo_data { get; set; }
    }
}
