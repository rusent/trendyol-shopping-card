using System;
using System.Collections.Generic;
using TrendyolShoppingCard.Business;
using TrendyolShoppingCard.Model;

namespace TrendyolShoppingCard
{
    class Program
    {
        static void Main()
        {
            var electronicsCategory = new Category(CategoryNames.Electronics);
            var homeCategory = new Category(CategoryNames.Home);
            var gardencategory = new Category(CategoryNames.Garden, homeCategory);

            var campaigns = SetCampaignList(electronicsCategory, homeCategory, gardencategory);

            var shoppingCardService = new ShoppingCardService(new DeliveryCostCalculator(2M, 6M));
            shoppingCardService.SetCampaigns(campaigns);

            var macbookPro = new Product(ProductNames.MacbookPro, 1300.00M, electronicsCategory);
            var pillow = new Product(ProductNames.Pillow, 30.00M, homeCategory);
            var table = new Product(ProductNames.Table, 450.00M, gardencategory);

            var cards = SetCardList(macbookPro, pillow, table);
            shoppingCardService.SetCards(cards);

            var coupon = new Coupon(minAmount: 1, discountAmount: 5, DiscountType.Amount);
            shoppingCardService.SetCoupon(coupon);

            Console.WriteLine(shoppingCardService.Print());
            Console.ReadKey();
        }

        static List<Campaign> SetCampaignList(Category electronicsCategory, Category homeCategory, Category gardencategory)
        {
            return new List<Campaign>
            {
                new Campaign(electronicsCategory, minAmount: 5, discountAmount: 10, DiscountType.Rate),
                new Campaign(homeCategory, minAmount: 1, discountAmount: 30, DiscountType.Rate),
                new Campaign(gardencategory, minAmount: 1, discountAmount: 30, DiscountType.Rate),
            };
        }

        static List<Card> SetCardList(Product macbookPro, Product pillow, Product table)
        {
            return new List<Card>
            {
                new Card(macbookPro,4),
                new Card(pillow,3),
                new Card(table,1)
            };
        }
    }
}