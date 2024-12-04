namespace backend.Domain
{
    public class OrderModel
    {
        public List<IdentifierAndQuantityModel> coffees { get; set; }
        public List<IdentifierAndQuantityModel> moneyAdded { get; set; }
    }
}
