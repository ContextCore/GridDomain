using System;

namespace Shop.ReadModel.Context
{
    public class Good
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public DateTime Created { get; set; }
    }
}