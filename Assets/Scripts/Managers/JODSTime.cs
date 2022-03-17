using UnityEngine;
using System.Threading.Tasks;
using System;

public class JODSTime : MonoBehaviour
{
    public static async Task WaitTime(float duration)
    { 
        float targetTime = Time.time + duration;
        while (Time.time < targetTime)
        {
            await Task.Yield();
        }
    }
    public static void WaitTimeEvent(float duration, Action action)
    {
        WaitTime(duration).GetAwaiter().OnCompleted(action);
    }

    public static async Task WaitFrame()
    {
        await Task.Yield();
    }
}
