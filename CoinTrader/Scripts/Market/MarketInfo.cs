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
    public double prev_closing_price;
    /// <summary>
    /// 예상 종가
    /// </summary>
    public List<PredictPrice> predictPrices = new List<PredictPrice>();
    /// <summary>
    /// 이동평균 15일
    /// </summary>
    public double movingAverage_15 = 0f;
    /// <summary>
    /// 이동평균 30일
    /// </summary>
    public double movingAverage_30 = 0f;
    /// <summary>
    /// 골든크로스 (단기이동평균선이 장기이동평균선보다 높을때, 반의어: 데드크로스)
    /// </summary>
    public bool IsGoldenCross => movingAverage_30 < movingAverage_15;
    /// <summary>
    /// 변동성 타겟 가격 (어제의 고가 - 어제의 저가) * k)
    /// </summary>
    public double buy_target_price = 0f;

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
        return (trade_price - prev_closing_price) / prev_closing_price;
    }

    /// <summary>
    /// 변동성 타겟 가격 세팅
    /// </summary>
    /// <param name="targetPrice"></param>
    public void SetTargetPrice(double targetPrice)
    {
        buy_target_price = targetPrice;
    }

    public override string ToString()
    {
        return $"{name}, {korean_name}, 현재가: {trade_price:N0}";
    }
}