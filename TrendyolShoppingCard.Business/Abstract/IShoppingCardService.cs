using System.Collections.Generic;
using TrendyolShoppingCard.Model;

namespace TrendyolShoppingCard.Business.Abstract
{
    public interface IShoppingCardService
    {
        decimal CalculateTotalAmount(List<Card> baskets);

        decimal ApplyCampaign(decimal totalAmount);

        decimal ApplyCoupon(decimal totalAmount);

        int GetNumberOfDeliveries();

        int GetNumberOfProducts();

        decimal GetDeliveryCost();

        void SetCampaigns(List<Campaign> campaigns);

        void SetCards(List<Card> cards);

        void SetCoupon(Coupon coupon);
    }
}