using NUnit.Framework;
using System;
using TrendyolShoppingCard.Business;
using TrendyolShoppingCard.Business.Abstract;

namespace TrendyolShoppingCard.Tests.Business
{
    [TestFixture]
    [Parallelizable(ParallelScope.Fixtures)]
    public class DeliveryCostCalculatorTests : BaseTestFixture
    {
        #region members & setup

        DeliveryCostCalculator deliveryCostCalculator;

        StrictMock<IShoppingCardService> shoppingCardService;

        [SetUp]
        public void Initialize()
        {
            shoppingCardService = new StrictMock<IShoppingCardService>();
        }

        #endregion

        #region verify mocks

        protected override void VerifyMocks()
        {
            shoppingCardService.VerifyAll();
        }

        #endregion

        [Test]
        public void Calculate_NullCard_ReturnsArgumentNullException()
        {
            //Arrange
            deliveryCostCalculator = new DeliveryCostCalculator(2M, 6M);

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => deliveryCostCalculator.Calculate(null));
        }

        [Test]
        public void Calculate_NoProduct_ReturnsFixedCost()
        {
            //Arrange
            deliveryCostCalculator = new DeliveryCostCalculator(2M, 6M);
            shoppingCardService.Setup(m => m.GetNumberOfDeliveries()).Returns(0);
            shoppingCardService.Setup(m => m.GetNumberOfProducts()).Returns(0);

            //Act

            //Assert
            var expectedResult = deliveryCostCalculator.Calculate(shoppingCardService.Object) == 6.99M;
            Assert.That(expectedResult);
        }

        [Test]
        public void Calculate_CardWithOneDeliveryOneProduct_ReturnsValidCalculation()
        {
            //Arrange
            deliveryCostCalculator = new DeliveryCostCalculator(2M, 6M);
            shoppingCardService.Setup(m => m.GetNumberOfDeliveries()).Returns(1);
            shoppingCardService.Setup(m => m.GetNumberOfProducts()).Returns(1);

            //Act

            //Assert
            var expectedResult = (2M * 1M) + (6M * 1M) + 6.99M;
            Assert.That(deliveryCostCalculator.Calculate(shoppingCardService.Object) == expectedResult);
        }
    }
}