using Network;
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
    /// 변동성 타겟 가격 = 전일 종가 + ((어제의 고가 - 어제의 저가) * k)
    /// </summary>
    public double buy_target_price = 0f;
    /// <summary>
    /// 최근 30일 캔들 데이터
    /// </summary>
    public List<CandlesDaysRes> candlesDaysLatest30 = new List<CandlesDaysRes>();

    /// <summary>
    /// 예상 종가 세팅
    /// </summary>
    /// <param name="modelOutput"></param>
    public void SetPredictPrices(DateTime dateTime, CoinTrader.ML.ModelOutput modelOutput)
    {
        if (modelOutput != null)
        {
            dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
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
                predictPrices.Add(predictPrice);
            }
        }
    }

    /// <summary>
    /// 전일 대비
    /// </summary>
    /// <returns></returns>
    public double GetVariabilityNormalize()
    {
        return (trade_price - prev_closing_price) / prev_closing_price;
    }

    /// <summary>
    /// 캔들 데이터 세팅
    /// </summary>
    /// <param name="res"></param>
    public void SetCandleDaysRes(List<CandlesDaysRes> res)
    {
        candlesDaysLatest30.Clear();
        candlesDaysLatest30.AddRange(res);

        prev_closing_price = candlesDaysLatest30[candlesDaysLatest30.Count - 1].prev_closing_price;
        movingAverage_15 = GetMovingAverage(15);
        movingAverage_30 = GetMovingAverage(30);
        buy_target_price = GetTargetPrice(0.3f);
    }

    /// <summary>
    /// 이동평균값 가져오기
    /// </summary>
    private double GetMovingAverage(int days)
    {
        double average = 0f;
        if (candlesDaysLatest30 != null && candlesDaysLatest30.Count > 0)
        {
            var candleData = candlesDaysLatest30[candlesDaysLatest30.Count - 1];
            double total = 0f;
            for (int i = 0; i < days; i++)
            {
                total += candlesDaysLatest30[candlesDaysLatest30.Count - 1 - i].trade_price;
            }
            average = total / days;
        }
        return average;
    }

    /// <summary>
    /// 전날 변동성 k 배수로 가져오기
    /// </summary>
    /// <param name="date"></param>
    /// <param name="k">배수 세팅 0f ~ 1f</param>
    /// <returns></returns>
    private double GetTargetPrice(float k)
    {
        k = Utils.Clamp(k, 0f, 1f);
        if (candlesDaysLatest30 != null && candlesDaysLatest30.Count > 0)
        {
            var candleData = candlesDaysLatest30[candlesDaysLatest30.Count - 1];
            return prev_closing_price + (candleData.high_price - candleData.low_price) * k;
        }
        return double.MaxValue;
    }

    public override string ToString()
    {
        return $"{name}, {korean_name}, 현재가: {trade_price:N0}";
    }

    internal void ResetDay()
    {
        throw new NotImplementedException();
    }
}