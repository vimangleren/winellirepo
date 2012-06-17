using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarmPenguin.StorageBroker;

namespace PenguinDeals
{
    public class DealBasket
    {
        public double BasketTotalPrice { get; set; }
        private List<Deal> _deals = new List<Deal>();

        public async Task AddToBasket(string dealId)
        {
            var deal = await DealData.GetDealAsync(dealId);
            _deals.Add(deal);

            CalculatePrice();
        }

        public List<Deal> GetDealsInBasket()
        {
            return _deals;
        }

        public void RemoveFromBasket(string dealId)
        {
            var deal = _deals.Where(x => x.DealId == dealId).First();
            _deals.Remove(deal);

            if (_deals.Count == 0)
                BasketTotalPrice = 0.0;
            else
                CalculatePrice();
        }

        public void EmptyBasket()
        {
            _deals.Clear();
            CalculatePrice();
        }

        private void CalculatePrice()
        {
            var price = 0.0;
            foreach (var item in _deals)
            {
                price = price + item.Price;
            }

            this.BasketTotalPrice = price;
        }
    }
}
