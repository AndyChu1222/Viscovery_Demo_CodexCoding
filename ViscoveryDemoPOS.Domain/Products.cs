using System.Collections.Generic;

namespace ViscoveryDemoPOS.Domain
{
    /// <summary>
    /// Indicates the recognition state of a product item.
    /// </summary>
    public enum RecognizeStatus
    {
        /// <summary>Item has not yet been processed.</summary>
        Waiting,
        /// <summary>Item was correctly recognized.</summary>
        Confirm,
        /// <summary>Item was expected but not recognized.</summary>
        Undetected,
        /// <summary>Item was recognized but not part of the expected order.</summary>
        Extra
    }

    /// <summary>
    /// Represents a single product and its recognition status.
    /// </summary>
    public class ProductItem
    {
        /// <summary>Unique code identifying the product.</summary>
        public string Code { get; set; }

        /// <summary>Display name of the product.</summary>
        public string Name { get; set; }

        /// <summary>Expected price of the product.</summary>
        public decimal Price { get; set; }

        /// <summary>Current recognition status of the product.</summary>
        public RecognizeStatus Status { get; set; }
    }

    /// <summary>
    /// Simplified order representation used by the demo application.  The order
    /// is based on a QR code and contains a list of expected items.
    /// </summary>
    public class QrOrder
    {
        /// <summary>Identifier read from the QR code.</summary>
        public string OrderId { get; set; }

        /// <summary>Collection of products expected to appear in the order.</summary>
        public List<ProductItem> ExpectedItems { get; set; } = new List<ProductItem>();
    }
}
