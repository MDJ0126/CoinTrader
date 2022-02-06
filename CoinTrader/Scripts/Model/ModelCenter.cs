public static class ModelCenter
{
    public static MarketModel MarketModel { get; private set; } = new MarketModel();

    public static void Release()
    {
        MarketModel = new MarketModel();
    }
}