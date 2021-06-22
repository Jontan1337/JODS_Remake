using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum TimerType
{
    tickContinuously,
    tickOnFinish,
    continuousAndFinish
}
public abstract class Timer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private TimerType timerType = TimerType.tickContinuously;
    [Space]
    [SerializeField] private bool startTimerOnAwake = false;
    [SerializeField] private float startOnAwakeDelay = 1.5f;
    [Space]
    [SerializeField] private float stopTime = 5f;
    private readonly float startTime = 0f;
    private float currentTime = 0f;
    [Space]
    [SerializeField, Range(0.01f, 10f)] private float tickRate = 0.1f;
    [Space]
    [SerializeField] private bool timerEnabled = false;
    [SerializeField, Range(0,100)] protected float timerProgress = 0f;

    public virtual void Start()
    {
        if (startTimerOnAwake)
        {
            StartCoroutine(StartDelay(startOnAwakeDelay));
        }
    }

    private IEnumerator StartDelay(float time)
    {
        yield return new WaitForSeconds(time);

        StartTimer(true, stopTime);
    }

    public virtual void StartTimer(bool start, float _stopTime = 5f)
    {
        timerEnabled = start;

        if (start == true)
        {
            stopTime = _stopTime == 0 ? stopTime : _stopTime;
            
            switch (timerType)
            {
                case TimerType.tickContinuously:
                    TickEn = StartCoroutine(TickEnumerator());
                    break;
                case TimerType.tickOnFinish:
                    FinishEn = StartCoroutine(FinishEnumerator());
                    break;
                case TimerType.continuousAndFinish:
                    TickEn = StartCoroutine(TickEnumerator());
                    FinishEn = StartCoroutine(FinishEnumerator());
                    break;
            }
        }
        else if (start == false)
        {
            switch (timerType)
            {
                case TimerType.tickContinuously:
                    StopCoroutine(TickEn);
                    break;
                case TimerType.tickOnFinish:
                    StopCoroutine(FinishEn);
                    break;
                case TimerType.continuousAndFinish:
                    StopCoroutine(FinishEn);
                    StopCoroutine(TickEn);
                    break;
            }
        }
    }

    public virtual void StartTimer(bool start, float _stopTime = 5f, Material[] mats = null)
    {
        StartTimer(start, _stopTime);
    }

    #region Coroutines
    private Coroutine TickEn;
    private IEnumerator TickEnumerator()
    {
        currentTime = startTime;
        timerProgress = 0f;

        while (timerProgress < 100)
        {
            currentTime += tickRate;
            timerProgress = currentTime / stopTime * 100;

            Tick();

            yield return new WaitForSeconds(tickRate);
        }
        timerProgress = 100;
    }

    private Coroutine FinishEn;
    private IEnumerator FinishEnumerator()
    {
        currentTime = startTime;
        timerProgress = 0f;

        while (timerProgress < 100)
        {
            currentTime += tickRate;
            timerProgress = currentTime / stopTime * 100;
            yield return new WaitForSeconds(tickRate);
        }
        timerProgress = 100;

        Finish();
    }
    #endregion
    protected abstract void Tick();
    protected abstract void Finish();
}
