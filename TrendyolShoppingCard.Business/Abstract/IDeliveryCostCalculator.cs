namespace TrendyolShoppingCard.Business.Abstract
{
    public interface IDeliveryCostCalculator
    {
        decimal Calculate(IShoppingCardService shoppingCartService);
    }
}