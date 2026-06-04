using UnityEngine;
using System;
using Unity.Mathematics;

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
        CheckMarketState();

        OnTimeChanged?.Invoke(hour, minute);
        if (hour >= 24)
        {
            NextDay();
        }
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
        if (hour == 9 && minute == 0)
        {
            IsMarketOpen = true;
            OnMarketOpen?.Invoke();
        }
        if (hour == 15 && minute == 30)
        {
            IsMarketOpen = false;
            OnMarketClose?.Invoke();
        }
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
}
