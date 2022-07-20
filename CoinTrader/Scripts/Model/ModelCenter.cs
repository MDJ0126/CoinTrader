public static class ModelCenter
{
    public static AccountModel Account { get; private set; } = new AccountModel();
    public static MarketModel Market { get; private set; } = new MarketModel();

    public static void Release()
    {
        Account = new AccountModel();
        Market = new MarketModel();
    }
}