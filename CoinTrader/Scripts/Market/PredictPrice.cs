using System;

public struct PredictPrice
{
    /// <summary>
    /// 날짜
    /// </summary>
    public DateTime dateTime;
    /// <summary>
    /// 예측값 (평균)
    /// </summary>
    public double forecasted;
    /// <summary>
    /// 최소 예측값
    /// </summary>
    public double lowerBound;
    /// <summary>
    /// 최대 예측값
    /// </summary>
    public double upperBound;
}