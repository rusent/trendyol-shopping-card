using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TrendyolShoppingCard.Business;
using TrendyolShoppingCard.Business.Abstract;
using TrendyolShoppingCard.Model;

namespace TrendyolShoppingCard.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Fixtures)]
    public class ShoppingCardServiceTests : BaseTestFixture
    {
        #region members & setup

        StrictMock<IDeliveryCostCalculator> deliveryCostCalculator;

        ShoppingCardService service;

        [SetUp]
        public void Initialize()
        {
            deliveryCostCalculator = new StrictMock<IDeliveryCostCalculator>();

            service = new ShoppingCardService(deliveryCostCalculator.Object);
        }

        #endregion

        #region verify mocks

        protected override void VerifyMocks()
        {
            deliveryCostCalculator.VerifyAll();
        }

        #endregion

        #region calculate total amount

        [Test]
        public void CalculateTotalAmount_CardListNull_ReturnArgumentNullException()
        {
            //Arrange
            List<Card> cards = null;

            //Act

            //Assert
            Assert.Throws(typeof(ArgumentNullException), delegate () { service.CalculateTotalAmount(cards); });
        }

        [Test]
        public void CalculateTotalAmount_CardListNotNull_ReturnTotalAmount()
        {
            //Arrange
            Product macbookPro, pillow, table;
            CreateProduct(out macbookPro, out pillow, out table);
            var cards = CreateCards(macbookPro, pillow, table);

            //Act
            var result = service.CalculateTotalAmount(cards);

            //Assert
            result.Should().Be(6390.00M);
        }

        #endregion

        #region apply campaign

        [Test]
        public void ApplyCampaign_NullCampaign_ReturnDiscountAmount()
        {
            //Arrange
            var totalAmount = 1000;

            //Act
            var result = service.ApplyCampaign(totalAmount);

            //Assert
            result.Should().Be(0);
        }

        [Test]
        public void ApplyCampaign_NotNullCampaign_ReturnDiscountAmount()
        {
            //Arrange
            var totalAmount = 1000;

            var electronicsCategory = new Category(CategoryNames.Electronics);
            var homeCategory = new Category(CategoryNames.Home);
            var gardencategory = new Category(CategoryNames.Garden, homeCategory);

            var campaigns = CreateCampaignList(electronicsCategory, homeCategory, gardencategory);

            service.SetCampaigns(campaigns);

            var macbookPro = new Product(ProductNames.MacbookPro, 1200.00M, electronicsCategory);
            var pillow = new Product(ProductNames.Pillow, 20.00M, homeCategory);
            var table = new Product(ProductNames.Table, 350.00M, gardencategory);

            var cards = CreateCards(macbookPro, pillow, table);
            service.SetCards(cards);

            //Act
            var result = service.ApplyCampaign(totalAmount);

            //Assert
            result.Should().Be(100.0M);
        }

        #endregion

        #region apply coupon

        [Test]
        public void ApplyCoupon_CouponNull_ReturnDiscountAmount()
        {
            //Arrange
            var totalAmount = 1000;

            //Act
            var result = service.ApplyCoupon(totalAmount);

            //Assert
            result.Should().Be(0);
        }

        [Test]
        public void ApplyCoupon_CouponNotNullAndTotalAmountLessThanCouponMinimumAmount_ReturnDiscountAmount()
        {
            //Arrange
            var totalAmount = 0;

            //Act
            var result = service.ApplyCoupon(totalAmount);

            //Assert
            result.Should().Be(0);
        }

        [Test]
        [TestCase(DiscountType.Rate, 60.00)]
        [TestCase(DiscountType.Amount, 6)]
        public void ApplyCoupon_CouponNotNullAndTotalAmountGreatherThanCouponMinimumAmount_ReturnTestClassData(DiscountType discountType, decimal expectedResult)
        {
            //Arrange
            var totalAmount = 1000;
            Product macbookPro, pillow, table;
            CreateProduct(out macbookPro, out pillow, out table);
            CreateCards(macbookPro, pillow, table);

            var coupon = new Coupon(minAmount: 2, discountAmount: 6, discountType);
            service.SetCoupon(coupon);

            //Act
            var result = service.ApplyCoupon(totalAmount);

            //Assert
            result.Should().Be(expectedResult);
        }

        #endregion

        #region get number of deliveries

        [Test]
        public void GetNumberOfDeliveries_NoCondition_ReturnsZero()
        {
            //Arrange

            //Act
            var result = service.GetNumberOfDeliveries();

            //Assert
            result.Should().Be(0);
        }

        #endregion

        #region get number of products

        [Test]
        public void GetNumberOfProduct_EmptyList_ReturnsZero()
        {
            //Arrange

            //Act
            var result = service.GetNumberOfProducts();

            //Assert
            result.Should().Be(0);
        }

        #endregion

        #region get total amount after discounts

        [Test]
        public void GetTotalAmountAfterDiscounts_NoCondition_ReturnTotalAmount()
        {
            //Arrange
            Product macbookPro, pillow, table;
            CreateProduct(out macbookPro, out pillow, out table);
            var cards = CreateCards(macbookPro, pillow, table);
            service.SetCards(cards);

            //Act
            var result = service.GetTotalAmountAfterDiscounts();

            //Assert
            result.Should().Be(6390.00M);
        }

        #endregion

        #region get delivery cost

        [Test]
        public void GetDeliveryCost_NoCondition_ReturnsOne()
        {
            //Arrange
            deliveryCostCalculator.Setup(x => x.Calculate(service)).Returns(1M);

            //Act
            var result = service.GetDeliveryCost();

            //Assert
            result.Should().Be(1M);
        }

        #endregion

        #region set coupon

        [Test]
        public void SetCoupon_NullCoupon_ReturnArgumentNullException()
        {
            //Arrange
            Coupon coupon = null;

            //Act

            //Assert
            Assert.Throws(typeof(ArgumentNullException), delegate () { service.SetCoupon(coupon); });
        }

        [Test]
        public void SetCoupon_NotNullCoupon_SetCoupon()
        {
            //Arrange
            var coupon = new Coupon(1, 2M, DiscountType.Amount);

            //Act

            //Assert
        }

        # endregion

        #region set cards

        [Test]
        public void SetCards_NullCards_ReturnArgumentNullException()
        {
            //Arrange
            List<Card> cards = null;

            //Act

            //Assert
            Assert.Throws(typeof(ArgumentNullException), delegate () { service.SetCards(cards); });
        }

        [Test]
        public void SetCards_NotNullCards_ReturnArgumentNullException()
        {
            //Arrange
            var cards = new List<Card>();

            //Act

            //Assert
        }

        #endregion

        #region set campaigns

        [Test]
        public void SetCampaigns_NullCampaigns_ReturnArgumentNullException()
        {
            //Arrange
            List<Campaign> campaigns = null;

            //Act

            //Assert
            Assert.Throws(typeof(ArgumentNullException), delegate () { service.SetCampaigns(campaigns); });
        }

        #endregion

        #region helper methods

        static void CreateProduct(out Product macbookPro, out Product pillow, out Product table)
        {
            var electronicsCategory = new Category(CategoryNames.Electronics);
            var homeCategory = new Category(CategoryNames.Home);
            var gardencategory = new Category(CategoryNames.Garden, homeCategory);

            macbookPro = new Product(ProductNames.MacbookPro, 1200.00M, electronicsCategory);
            pillow = new Product(ProductNames.Pillow, 20.00M, homeCategory);
            table = new Product(ProductNames.Table, 350.00M, gardencategory);
        }

        static List<Card> CreateCards(Product macbookPro, Product pillow, Product table)
        {
            return new List<Card>
            {
                 new Card(macbookPro,5),
                 new Card(pillow,2),
                 new Card(table,1)
            };
        }

        static List<Category> CreateCategories()
        {
            var electronicsCategory = new Category(CategoryNames.Electronics);
            var homeCategory = new Category(CategoryNames.Home);
            var gardencategory = new Category(CategoryNames.Garden, homeCategory);

            return new List<Category>
            {
                electronicsCategory,
                homeCategory,
                gardencategory
            };
        }

        static List<Campaign> CreateCampaignList(Category electronicsCategory, Category homeCategory, Category gardencategory)
        {
            return new List<Campaign>
            {
                new Campaign(electronicsCategory, minAmount: 5, discountAmount: 10, DiscountType.Rate),
                new Campaign(homeCategory, minAmount: 5, discountAmount: 1, DiscountType.Amount),
                new Campaign(gardencategory, minAmount: 5, discountAmount: 1, DiscountType.Amount),
            };
        }

        #endregion
    }
}