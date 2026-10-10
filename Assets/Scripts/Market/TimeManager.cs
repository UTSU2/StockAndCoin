using UnityEngine;
using System;
using Unity.Mathematics;
using Data;

public class TimeManager : MonoBehaviour
{
    public int year = 2026;
    public int month = 1;
    public int day = 1;

    public int hour = 9;
    public int minute = 0;
    public float realSecondsPerGameMinute = 1f;
    private float timer;

    public event Action OnDayChanged;
    public event Action OnMarketOpen;
    public event Action OnMarketClose;
    public event Action<int, int> OnTimeChanged;
    public bool IsMarketOpen { get; private set; }

    private void Start()
    {
        CheckMarketState();
    }
    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= realSecondsPerGameMinute)
        {
            timer = 0f;
            AddMinute();
        }
    }
    private void AddMinute()
    {
        minute++;

        if (minute >= 60)
        {
            minute = 0;
            hour++;
        }
        if (hour >= 24)
        {
            NextDay();
        }
        CheckMarketState();

        OnTimeChanged?.Invoke(hour, minute);
    }
    private void NextDay()
    {
        hour = 0;
        minute = 0;

        DateTime date = new DateTime(year, month, day);
        date = date.AddDays(1);

        year = date.Year;
        month = date.Month;
        day = date.Day;

        OnDayChanged?.Invoke();
    }
    private void CheckMarketState()
    {
        bool shouldOpen = hour >= 9 && (hour < 15 || (hour == 15 && minute < 30));

        if (IsMarketOpen == shouldOpen)
            return;

        IsMarketOpen = shouldOpen;

        if (IsMarketOpen)
            OnMarketOpen?.Invoke();
        else
            OnMarketClose?.Invoke();
    }
    public void ForceCloseMarket()
    {
        IsMarketOpen = false;
    }

    public string GetDateText()
    {
        return $"{year:D4}-{month:D2}-{day:D2}";
    }
    public string GetTimeText()
    {
        return $"{hour:D2}:{minute:D2}";
    }

    public bool CanTrade(MarketType marketType)
    {
        if (marketType == MarketType.Coin)
            return true;
        else
            return false;
    }
}
