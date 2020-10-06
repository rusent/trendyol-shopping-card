using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrendyolShoppingCard.Business.Abstract;
using TrendyolShoppingCard.Model;

namespace TrendyolShoppingCard.Business
{
    public class ShoppingCardService : BaseService, IShoppingCardService
    {
        #region ctor

        internal List<Campaign> Campaigns { get; set; }

        internal List<Card> Cards { get; set; }

        internal Coupon Coupon { get; set; }

        readonly IDeliveryCostCalculator deliveryCostCalculator;

        public ShoppingCardService(IDeliveryCostCalculator deliveryCostCalculator)
        {
            this.Campaigns = new List<Campaign>();
            this.Cards = new List<Card>();
            this.deliveryCostCalculator = deliveryCostCalculator;
        }

        #endregion

        #region  total amount

        public decimal CalculateTotalAmount(List<Card> cards)
        {
            if (CardListExists(cards))
            {
                throw new ArgumentNullException($"{SetMethodNameForMessage(nameof(CalculateTotalAmount))} is Null");
            }
            else
            {
                return GetTotalAmount(cards);
            }
        }

        static bool CardListExists(List<Card> cards)
        {
            return cards == null;
        }

        static decimal GetTotalAmount(List<Card> cards)
        {
            return cards.Sum(x => x.Quantity * x.Product.Price);
        }

        #endregion

        #region apply campaign

        public decimal ApplyCampaign(decimal totalAmount)
        {
            decimal discountAmount = 0;
            foreach (Campaign campaign in Campaigns)
            {
                var product = GetProductsByCategory(campaign.Category);
                if (product.Values.Sum() >= campaign.MinimumAmount)
                {
                    return GetDiscountAmount(totalAmount, discountAmount, campaign);
                }
            }

            return discountAmount;
        }

        Dictionary<Product, int> GetProductsByCategory(Category category)
        {
            return Cards.Where(x => x.Product.Category == category || IsSubCategory(category, x.Product.Category)).ToDictionary(x => x.Product, x => x.Quantity);
        }

        static bool IsSubCategory(Category category, Category subCategory)
        {
            if (CategoryOrSubCategoryExists(category, subCategory))
            {
                return false;
            }

            var parentCategory = subCategory.ParentCategory;
            while (ParentCategoryExists(parentCategory))
            {
                if (IsCategoryAndSubCategoryEqual(category, parentCategory))
                {
                    return true;
                }

                parentCategory = parentCategory.ParentCategory;
            }

            return false;
        }

        static decimal GetDiscountAmount(decimal totalAmount, decimal discountAmount, Campaign campaign)
        {
            switch (campaign.DiscountType)
            {
                case DiscountType.Rate:
                    {
                        var calculatedDiscountAmount = totalAmount * (campaign.DiscountAmount / 100);
                        if (calculatedDiscountAmount > discountAmount)
                        {
                            discountAmount = calculatedDiscountAmount;
                        }
                        break;
                    }
                case DiscountType.Amount:
                    {
                        if (campaign.DiscountAmount > discountAmount)
                        {
                            discountAmount = campaign.DiscountAmount;
                        }
                        break;
                    }

                default:
                    break;
            }

            return discountAmount;
        }

        static bool CategoryOrSubCategoryExists(Category category, Category subCategory)
        {
            return category == null || subCategory == null;
        }

        static bool ParentCategoryExists(Category parentCategory)
        {
            return parentCategory != null;
        }

        static bool IsCategoryAndSubCategoryEqual(Category category, Category parentCategory)
        {
            return parentCategory == category;
        }

        #endregion

        #region apply coupon

        public decimal ApplyCoupon(decimal totalAmount)
        {
            decimal discountAmount = 0;

            if (CouponExistAndGreatherThanTotalAmountCouponMinimumAmount(totalAmount))
            {
                switch (Coupon.DiscountType)
                {
                    case DiscountType.Rate:
                        return totalAmount * (Coupon.DiscountAmount / 100);

                    case DiscountType.Amount:
                        return Coupon.DiscountAmount;

                    default:
                        throw new Exception("Unexpected Case");
                }
            }
            return discountAmount;
        }

        bool CouponExistAndGreatherThanTotalAmountCouponMinimumAmount(decimal totalAmount)
        {
            return Coupon != null && totalAmount >= Coupon.MinimumAmount;
        }

        #endregion

        #region print

        public string Print()
        {
            var builder = new StringBuilder();
            var products = Cards.GroupBy(p => p.Product.Category.Title).ToDictionary(e => e.Key, e => e.ToList());

            builder.AppendLine($"{"Category Name",20}  " + $"{"Product Name",20}  " + $"{"Quantity",20}  " + $"{"Unit Price",20}  " + $"{"Total Price",20}");

            foreach (var item in products)
            {
                foreach (var p in item.Value)
                {
                    builder.AppendLine($"{item.Key,20} {p.Product.Title,20} {p.Quantity,20} {p.Product.Price,20} {(p.Product.Price * p.Quantity),20}\t");
                }
            }

            builder.AppendLine(
                $"\nTotal Amount: {GetTotalAmount(Cards)}" +
                $"\nTotal Amount After Discounts: {GetTotalAmountAfterDiscounts()}" +
                $"\nTotal Discount: {GetTotalDiscount()}" +
                $"\nDelivery Cost: {GetDeliveryCost()}");

            return builder.ToString();
        }

        #endregion

        #region get number of deliveries

        public int GetNumberOfDeliveries()
        {
            return Cards.GroupBy(x => x.Product.Category.Title).Count();
        }

        #endregion

        #region get number of products

        public int GetNumberOfProducts()
        {
            return Cards.GroupBy(x => x.Product).Count();
        }

        #endregion

        #region get total amount after discounts

        public decimal GetTotalAmountAfterDiscounts()
        {
            var amount = GetTotalAmount(Cards);
            amount -= ApplyCampaign(amount);
            amount -= ApplyCoupon(amount);
            return amount;
        }

        #endregion

        #region get delivery cost

        public decimal GetDeliveryCost()
        {
            return deliveryCostCalculator.Calculate(this);
        }

        #endregion

        #region get total discount

        decimal GetTotalDiscount()
        {
            return GetTotalAmount(Cards) - GetTotalAmountAfterDiscounts();
        }

        #endregion

        #region set methods

        public void SetCoupon(Coupon coupon)
        {
            if (coupon == null)
            {
                throw new ArgumentNullException($"{SetMethodNameForMessage(nameof(SetCoupon))} is Null");
            }
            else
            {
                Coupon = coupon;
            }
        }

        public void SetCards(List<Card> cards)
        {
            if (cards == null)
            {
                throw new ArgumentNullException($"{SetMethodNameForMessage(nameof(SetCards))} is Null");
            }
            else
            {
                Cards = cards;
            }
        }

        public void SetCampaigns(List<Campaign> campaigns)
        {
            if (campaigns == null)
            {
                throw new ArgumentNullException($"{SetMethodNameForMessage(nameof(SetCampaigns))} is Null");
            }
            else
            {
                Campaigns = campaigns;
            }
        }

        #endregion
    }
}