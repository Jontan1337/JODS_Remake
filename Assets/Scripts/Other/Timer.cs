using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

enum TimerType
{
    tickContinuously,
    tickOnFinish,
    continuousAndFinish
}
public abstract class Timer : NetworkBehaviour
{
    [Header("Network Settings")]
    [SerializeField] private bool onlyServer = false;
    [Header("Timer Settings")]
    [SerializeField] private TimerType timerType = TimerType.continuousAndFinish;
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
        if (onlyServer && !isServer) return;

        if (startTimerOnAwake)
        {
            StartTimer(true, stopTime, startOnAwakeDelay);
        }
    }

    public virtual void StartTimer(bool start, float _stopTime = 5f, float delay = 0)
    {
        timerEnabled = start;

        if (start == true)
        {
            stopTime = _stopTime == 0 ? stopTime : _stopTime;
            
            switch (timerType)
            {
                case TimerType.tickContinuously:
                    TickEn = StartCoroutine(TickEnumerator(delay));
                    break;
                case TimerType.tickOnFinish:
                    FinishEn = StartCoroutine(FinishEnumerator(delay));
                    break;
                case TimerType.continuousAndFinish:
                    TickEn = StartCoroutine(TickEnumerator(delay));
                    FinishEn = StartCoroutine(FinishEnumerator(delay));
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

    public virtual void StartTimer(bool start, float _stopTime = 5f, float delay = 0, Material[] mats = null)
    {
        StartTimer(start, _stopTime, delay);
    }

    #region Coroutines
    private Coroutine TickEn;
    private IEnumerator TickEnumerator(float delay = 0)
    {
        yield return new WaitForSeconds(delay);

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
    private IEnumerator FinishEnumerator(float delay = 0)
    {
        yield return new WaitForSeconds(delay);

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
