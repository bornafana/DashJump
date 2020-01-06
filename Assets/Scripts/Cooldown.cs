using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooldown : MonoBehaviour
{
    public static void CountDown(float value, float from, float to)
    {
        //StartCoroutine(CountDownRoutine(value, from, to));
    }

    public static IEnumerator CountDownRoutine(float value, float from, float to)
    {
        value = from;
        while (value > to)
        {
            value -= Time.deltaTime;
            
            yield return new WaitForEndOfFrame();
        }
    }

    public static IEnumerator CountUpRoutine(float value, float from, float to)
    {
        value = from;
        while (value <= to)
        {
            value += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
