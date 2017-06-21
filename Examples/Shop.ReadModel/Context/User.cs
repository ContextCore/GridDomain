using System;

namespace Shop.ReadModel.Context
{
    public class User
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public DateTime Created { get; set; }
    }
}