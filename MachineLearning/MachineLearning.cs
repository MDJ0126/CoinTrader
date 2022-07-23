using System;
using System.IO;

namespace CoinTrader.ML
{
    public static class MachineLearning
    {
        static void CreateCSV<T>(System.Collections.Generic.ICollection<T> collection)
        {
            Utils.CreateCSVFile(collection, Path.Combine(Environment.CurrentDirectory, "Data", "taxi-fare-train.csv"));
        }
    }
}
