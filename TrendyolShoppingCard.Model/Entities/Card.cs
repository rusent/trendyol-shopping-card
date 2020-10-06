namespace TrendyolShoppingCard.Model
{
    public class Card
    {
        #region factory methods

        public Card(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        #endregion

        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
}