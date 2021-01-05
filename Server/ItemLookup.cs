using System;
using System.Collections.Generic;
using System.Linq;
using dev;

namespace hypixel
{
    public partial class ItemPrices
    {
        private class ItemLookup
        {
            public int ItemId => (int)(Prices.FirstOrDefault() == null ? 0 : Prices.FirstOrDefault().ItemId);
            public AveragePrice Oldest => Prices.FirstOrDefault();
            public List<AveragePrice> Prices = new List<AveragePrice>();

            public ItemLookup()
            {

            }

            public ItemLookup(IEnumerable<SaveAuction> auctions)
            {
                Prices = auctions.Select(a => AverageFromAuction(a)).ToList();
            }

            public void AddNew(AveragePrice price)
            {
                Prices.Add(price);
            }

            public void AddNew(SaveAuction auction)
            {
                AddNew(AverageFromAuction(auction));
            }

            private static AveragePrice AverageFromAuction(SaveAuction auction)
            {
                return new AveragePrice()
                {
                    Avg = auction.HighestBidAmount,
                    Date = auction.End,
                    Volume = auction.Count,
                    Min = auction.HighestBidAmount,
                    Max = auction.HighestBidAmount,
                    ItemId = auction.ItemId
                };
            }

            public AveragePrice CombineIntoOne(DateTime start, DateTime end)
            {
                var complete = new AveragePrice();
                var matchingSelection = Prices
                    .Where(p => p.Date >= start && p.Date <= end)
                    .OrderBy(p => p.Date);
                if (matchingSelection.Count() == 0 || matchingSelection.First().Date.Ticks == 0)
                    return complete;
                foreach (var item in matchingSelection)
                {
                    complete.Avg += item.Avg;
                    complete.Volume += item.Volume;
                    if (complete.Max < item.Max)
                        complete.Max = item.Max;
                    if (complete.Min > item.Min)
                        complete.Min = item.Min;

                }
                complete.Avg /= Prices.Count();
                complete.Date = matchingSelection.Where(p=>p.Date.Ticks > 0).First().Date;
                return complete;
            }

            internal void AddNew(ProductInfo item, DateTime time)
            {
                AddNew(new AveragePrice()
                {
                    ItemId = ItemDetails.Instance.GetItemIdForName(item.ProductId, false),
                    Max = (float)item.QuickStatus.BuyPrice,
                    Min = (float)item.QuickStatus.SellPrice,
                    Avg = (float)(item.QuickStatus.BuyPrice + item.QuickStatus.SellPrice) / 2,
                    Date = time,
                    Volume = (int)item.QuickStatus.SellMovingWeek
                });
            }

            internal void Discard(DateTime allBefore)
            {
                this.Prices = this.Prices.Where(p => p.Date >= allBefore).ToList();
            }
        }
    }
}
