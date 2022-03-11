using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

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
    public static async Task WaitFrame()
    {
        await Task.Yield();
    }
}
