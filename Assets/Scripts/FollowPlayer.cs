using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    public float lerpSpeed = 5f;
    private Vector3 offset;

    private void Start()
    {
        offset = transform.position - player.transform.position;
    }

    private void LateUpdate()
    {
        //transform.position = player.transform.position + offset;

       
            transform.position = Vector3.Lerp(
                transform.position,                         // current camera position
                player.transform.position + offset,   // new position plus our original offset
                lerpSpeed * Time.deltaTime);                                 // the speed of smoothing
        
    }
}