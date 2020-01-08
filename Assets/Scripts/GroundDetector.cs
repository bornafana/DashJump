using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroundDetector : MonoBehaviour
{
    public UnityEvent groundEvent;

    private void OnCollisionEnter2D(Collision2D col)
    {
        groundEvent.Invoke();
    }
}
