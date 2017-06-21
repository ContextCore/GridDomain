namespace Shop.ReadModel.Context
{
    public enum StockOperation
    {
        Created,
        Taken,
        Reserved,
        ReserveTaken,
        ReserveRenewed,
        ReserveExpired,
        ReserveCanceled,
        Added
    }
}