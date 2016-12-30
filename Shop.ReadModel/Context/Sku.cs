using System;

namespace Shop.ReadModel.Context
{
    public class Sku
    {
        public Guid Id { get; set; }
        public string Article { get; set; }
        public int Number { get; set; }
        public string Name  { get; set; }

        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
    }
}