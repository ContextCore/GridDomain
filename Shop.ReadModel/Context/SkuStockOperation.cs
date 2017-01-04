namespace Shop.ReadModel.Context
{
    public enum SkuStockOperation
    {
        Created,
        Taken,
        Reserved,
        ResereveTaken,
        ReserveRenewed, 
        ReserveExpired,
        Added
    }
}