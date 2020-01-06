using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Lean.Touch;
using UnityEngine.Rendering;

[RequireComponent(typeof(LineRenderer))]
public class DragIndicator : MonoBehaviour
{
    public float cutOffAngle;
    public float dragResistance = 50f;
    private bool canHook;
    private Vector3 startPos;
    private Vector3 endPos;
    private Camera camera;
    private LineRenderer lineRenderer;
    private readonly Vector3 camOffset = new Vector3(0f, 0f, 10f);

    [SerializeField] private AnimationCurve widthCurve;

    public delegate void DragCallBack(Vector2 power);
    public static event DragCallBack OnDrag;

    private void Start()
    {
        camera = Camera.main;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
    }

    private void Update()
    {
        if (lineRenderer.positionCount > 0)
            lineRenderer.SetPosition(0, transform.position);

        if (Input.GetMouseButtonDown(0))
        {
            if (lineRenderer == null)
                lineRenderer = gameObject.AddComponent<LineRenderer>();

            canHook = true;
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;

            //startPos = camera.ScreenToWorldPoint(Input.mousePosition) + camOffset;
            startPos = LeanTouch.Fingers[0].StartScreenPosition;

            lineRenderer.useWorldSpace = true;
            lineRenderer.widthCurve = widthCurve;
            lineRenderer.numCapVertices = 5;
        }

        if (Input.GetMouseButton(0))
        {
            //endPos = camera.ScreenToWorldPoint(Input.mousePosition) + camOffset;
            endPos = LeanTouch.Fingers[0].LastScreenPosition;

            //only allow hooking when player is dragging downwards. Decimal represents % angled upwards or downwards. 
            lineRenderer.enabled = canHook = ((startPos - endPos).normalized.y > cutOffAngle); 

            lineRenderer.SetPosition(1, transform.position + ((startPos - endPos) / dragResistance));
        }

        if (Input.GetMouseButtonUp(0))
        {
            lineRenderer.enabled = false;

            if (canHook)
                RaiseDrag(new Vector2(startPos.x - endPos.x, startPos.y - endPos.y));
        }
    }

    private void RaiseDrag(Vector2 power)
    {
        OnDrag?.Invoke(power);
        DragDone();
    }

    private void DragDone()
    {
        // Do something when drag is done like animations/particles
    }
}
