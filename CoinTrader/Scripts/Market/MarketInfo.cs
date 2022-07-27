using System;
using System.Collections.Generic;

public class MarketInfo
{
    /// <summary>
    /// 종목 구분 코드
    /// </summary>
    public string name;
    /// <summary>
    /// 한글 이름
    /// </summary>
    public string korean_name;
    /// <summary>
    /// 종가(현재가)
    /// </summary>
    public double trade_price;
    /// <summary>
    /// 전일 종가
    /// </summary>
    public double? yesterday_trade_price;
    /// <summary>
    /// 예상 종가
    /// </summary>
    public List<PredictPrice> predictPrices = new List<PredictPrice>();

    /// <summary>
    /// 과거 캔들 데이터 모두 받았는지 여부
    /// </summary>
    public bool isCandleOldDataSuccess = false;

    /// <summary>
    /// 최신 캔들 데이터 모두 받았는지 여부
    /// </summary>
    public bool isCandleNewDataSuccess = false;

    /// <summary>
    /// 예상 종가 세팅
    /// </summary>
    /// <param name="modelOutput"></param>
    public void SetPredictPrices(DateTime dateTime, CoinTrader.ML.ModelOutput modelOutput)
    {
        if (modelOutput != null)
        {
            dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
            predictPrices.Clear();
            int length = modelOutput.Forecasted.Length;
            for (int i = 0; i < length; i++)
            {
                var predictPrice = new PredictPrice
                {
                    dateTime = dateTime.AddHours(i),
                    forecasted = modelOutput.Forecasted[i],
                    lowerBound = modelOutput.LowerBound[i],
                    upperBound = modelOutput.UpperBound[i],
                };

                if (predictPrice.dateTime.Hour == 0)
                {
                    predictPrices.Add(predictPrice);
                }
            }
        }
    }

    /// <summary>
    /// 변동성 퍼센테이지 Normalize (-1f ~ 1f)
    /// </summary>
    /// <returns></returns>
    public double GetVariabilityNormalize()
    {
        return (trade_price - yesterday_trade_price.Value) / yesterday_trade_price.Value;
    }

    /// <summary>
    /// 금일 예상 종가 퍼센테이지 Normalize (-1f ~ 1f)
    /// </summary>
    /// <returns></returns>
    public double GetTodayPredicteNormalize()
    {
        if (predictPrices != null && predictPrices.Count > 0)
            return (predictPrices[0].forecasted - yesterday_trade_price.Value) / yesterday_trade_price.Value;
        return 0d;
    }

    public override string ToString()
    {
        return $"{name}, {korean_name}, 현재가: {trade_price:N0}";
    }
}