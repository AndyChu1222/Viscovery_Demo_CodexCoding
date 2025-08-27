using System.Collections.Generic;
using System.Linq;
using ViscoveryDemoPOS.Domain;

namespace ViscoveryDemoPOS.BLL
{
    public static class RecognitionComparer
    {
        public static List<ProductItem> MergeAndMark(QrOrder order, IEnumerable<ProductItem> recognized)
        {
            var result = order.ExpectedItems.Select(x => new ProductItem
            {
                Code = x.Code, Name = x.Name, Price = x.Price, Status = RecognizeStatus.Undetected
            }).ToList();

            foreach (var r in recognized)
            {
                var match = result.FirstOrDefault(i => i.Code == r.Code || i.Name == r.Name);
                if (match != null) match.Status = RecognizeStatus.Confirm;
                else result.Add(new ProductItem { Code = r.Code, Name = r.Name, Status = RecognizeStatus.Extra });
            }

            return result;
        }
    }
}
