namespace Sklep_z_towarami.Models
{
    public class Order
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string PaymentMethod {  get; set; }

        public Order() { }

    }
}
