using System.Collections.Generic;
using System.Linq;
using ViscoveryDemoPOS.Domain;

namespace ViscoveryDemoPOS.BLL
{
    /// <summary>
    /// Utility methods for comparing recognition results with an expected order
    /// and producing a merged list of items annotated with their recognition
    /// status.
    /// </summary>
    public static class RecognitionComparer
    {
        /// <summary>
        /// Combines the expected items from a QR order with the list of products
        /// recognized by VisAgent.  Items that appear in both lists are marked as
        /// <see cref="RecognizeStatus.Confirm"/>.  Recognized items that do not
        /// belong to the order are appended and marked as <see cref="RecognizeStatus.Extra"/>.
        /// Any expected items not recognized remain with status
        /// <see cref="RecognizeStatus.Undetected"/>.
        /// </summary>
        /// <param name="order">Original order containing expected products.</param>
        /// <param name="recognized">Products returned from the recognition engine.</param>
        /// <returns>A merged list of products with recognition status filled in.</returns>
        public static List<ProductItem> MergeAndMark(QrOrder order, IEnumerable<ProductItem> recognized)
        {
            // Start with a copy of all expected items and mark them as undetected.
            var result = order.ExpectedItems.Select(x => new ProductItem
            {
                Code = x.Code,
                Name = x.Name,
                Price = x.Price,
                Status = RecognizeStatus.Undetected
            }).ToList();

            // Compare each recognized item to the expected list and update the
            // status accordingly.  Unmatched items are treated as extras.
            foreach (var r in recognized)
            {
                var match = result.FirstOrDefault(i => i.Code == r.Code || i.Name == r.Name);
                if (match != null)
                    match.Status = RecognizeStatus.Confirm;
                else
                    result.Add(new ProductItem { Code = r.Code, Name = r.Name, Status = RecognizeStatus.Extra });
            }

            return result;
        }
    }
}
