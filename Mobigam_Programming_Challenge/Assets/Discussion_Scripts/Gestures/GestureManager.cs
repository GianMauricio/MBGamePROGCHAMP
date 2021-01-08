using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GestureManager : MonoBehaviour
{
    public static GestureManager Instance;
    
    public TapProperty _tapProperty;
    public SwipeProperty _swipeProperty;
    public DragProperty _dragProperty;
    public TwoFingerPanProperty _twoFingerPan;
    public SpreadProperty _spreadProperty;
    public RotateProperty _rotateProperty;

    public event EventHandler<TapEventArgs> OnTap;
    public event EventHandler<DragEventArgs> OnDrag;
    public event EventHandler<SwipeEventArgs> OnSwipe;
    public event EventHandler<TwoFingerPanEventArgs> OnTwoFingerPan;
    public event EventHandler<SpreadEventArgs> OnPinchSpread;
    public event EventHandler<RotateEventArgs> OnRotate;
    private Touch gesture_finger;
    private Touch gesture_finger1;
    private Touch gesture_finger2;

    private Vector2 start_pos;
    private Vector2 end_pos;

    private float gesture_time;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    void start()
    {
    }

    void Update()
    {

        if (Input.touchCount > 0)
        {
            if (Input.touchCount == 1) {
                //CheckSingleFingerGesture();        
              }
            else
            {
                gesture_finger1 = Input.GetTouch(0);
                gesture_finger2 = Input.GetTouch(1);

                /*if(gesture_finger1.phase == TouchPhase.Moved && gesture_finger2.phase == TouchPhase.Moved
                && Vector2.Distance(gesture_finger1.position,gesture_finger2.position) <= (_twoFingerPan.MaxDistance * Screen.dpi));
                {
                    FireTwoFingerPan();
                } 

                if (gesture_finger1.phase == TouchPhase.Moved || gesture_finger2.phase == TouchPhase.Moved)
                {
                    Vector2 prevPoint1 = GetPreviousPoint(gesture_finger1);
                    Vector2 prevPoint2 = GetPreviousPoint(gesture_finger2);

                    float currDistance = Vector2.Distance(gesture_finger1.position, gesture_finger2.position);
                    float prevDistance = Vector2.Distance(prevPoint1, prevPoint2);

                    if (Mathf.Abs(currDistance - prevDistance) >= (_spreadProperty.MinDistanceChange * Screen.dpi))
                    {
                        FireSpreadEvent(currDistance - prevDistance);
                    }
                }*/

                if((gesture_finger1.phase == TouchPhase.Moved || gesture_finger2.phase == TouchPhase.Moved)&&
                    Vector2.Distance(gesture_finger1.position, gesture_finger2.position) >= (_rotateProperty.MinDistance * Screen.dpi))
                {
                    Vector2 prevPoint1 = GetPreviousPoint(gesture_finger1);
                    Vector2 prevPoint2 = GetPreviousPoint(gesture_finger2);

                    Vector2 diff_vector = gesture_finger1.position - gesture_finger2.position;
                    Vector2 prev_diff_vector = prevPoint1 - prevPoint2;

                    float angle = Vector2.Angle(prev_diff_vector, diff_vector);
                    if(angle >= _rotateProperty.MinChange)
                    {
                        Vector3 cross = Vector3.Cross(prev_diff_vector, diff_vector);

                        if (cross.z > 0)
                        {
                            FireRotateEvent(angle, RotationDirections.CCW);
                            Debug.Log($"Rotate Counter Cw{angle}");
                        }

                        else if (cross.z < 0)
                        {
                            FireRotateEvent(angle, RotationDirections.CW);
                            Debug.Log($"Rotate CW {angle}");
                        }

                        
                    }
                }

            }
        }
    }

    private GameObject GetHit(Vector2 screenPos)
    {
        Ray r = Camera.main.ScreenPointToRay(start_pos);
        RaycastHit hit = new RaycastHit();
        GameObject hitObj = null;

        if (Physics.Raycast(r, out hit, Mathf.Infinity))
        {
            hitObj = hit.collider.gameObject;
        }

        return hitObj;
    }

    private void CheckSingleFingerGesture() {

        gesture_finger = Input.GetTouch(0);

        if (gesture_finger.phase == TouchPhase.Began)
        {
            start_pos = gesture_finger.position;
            gesture_time = 0;
        }

        if (gesture_finger.phase == TouchPhase.Ended)
        {
            end_pos = gesture_finger.position;

            if (gesture_time <= _swipeProperty.MaxGestureTime && Vector2.Distance(start_pos, end_pos) >=
                (_swipeProperty.MinSwipeDistance * Screen.dpi))
            {
                FireSwipeFunction();
            }
        }

        else
        {
            Debug.Log("DRAGGING");
            gesture_time += Time.deltaTime;
            if (gesture_time >= _dragProperty.MinGesture)
            {
                //FireDragFunction();
            }
        }
    }


    private Vector2 GetPreviousPoint(Touch t)
    {
        return t.position - t.deltaPosition;
    }

    private Vector2 GetMidPoint (Vector2 p1, Vector2 p2)
    {
        Vector2 ret = new Vector2((p1.x + p2.x) / 2, (p1.y + p2.y) / 2);
        return ret;
    }
    private void FireRotateEvent(float angle, RotationDirections dir)
    {
        Vector2 midPoint = GetMidPoint(gesture_finger1.position, gesture_finger2.position);

        GameObject hitObj = GetHit(midPoint);

        RotateEventArgs args = new RotateEventArgs(gesture_finger1, gesture_finger2, angle, dir, hitObj);

        if(OnRotate != null)
        {
            OnRotate(this, args);
        }

        if(hitObj != null)
        {
            IRotate rot = hitObj.GetComponent<IRotate>();
            if(rot != null)
            {
                rot.onRotate(args);
            }
        }
    }
    private void FireSpreadEvent(float dist_diff)
    {
        Debug.Log("Spread");

        if(dist_diff > 0)
        {
            Debug.Log("Spread");
        }

        else
        {
            Debug.Log("Pinch");
        }

        Vector2 midPoint = GetMidPoint(gesture_finger1.position, gesture_finger2.position);

        GameObject hitObj = GetHit(midPoint);

        SpreadEventArgs args = new SpreadEventArgs(gesture_finger1, gesture_finger2, dist_diff, hitObj);

        if(OnPinchSpread != null)
        {
            OnPinchSpread(this, args);
        }

        if(hitObj != null)
        {
            IPinchSpread pinchSpread = hitObj.GetComponent<IPinchSpread>();
            if(pinchSpread != null)
            {
                pinchSpread.OnPinchSpread(args);
            }
        }
    }
    private void FireTwoFingerPan()
    {
        Debug.Log("Panning");

        TwoFingerPanEventArgs args = new TwoFingerPanEventArgs(gesture_finger1, gesture_finger2);

        if(OnTwoFingerPan != null)
        {
            OnTwoFingerPan(this, args);
        }
    }

    private void FireDragFunction() {

        Debug.Log($"Drag{ gesture_finger.position.ToString()}");

        GameObject hitObject = GetHit(gesture_finger.position);
        DragEventArgs args = new DragEventArgs(gesture_finger, hitObject);

        if (OnDrag != null) {
            OnDrag(this, args);
        }

        if(hitObject != null)
        {

            IDragged drag = hitObject.GetComponent<IDragged>();
            if (drag != null)
            {
                drag.OnDrag(args);

            }
        }

    }

    private void FireSwipeFunction() {
        Debug.Log("SWIPE");

        Vector2 diff = end_pos - start_pos;

        GameObject hitObject = GetHit(start_pos);
        Directions dir;

        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            if(diff.x > 0)
            {
                Debug.Log("Right");
                dir = Directions.RIGHT;
            }
            else
            {
                Debug.Log("Left");
                dir = Directions.LEFT;
            }
        }
        else
        {
            if (diff.y > 0) {
                Debug.Log("up");
                dir = Directions.UP;
            }

            else {
                Debug.Log("down");
                dir = Directions.DOWN;
            }
        }

        SwipeEventArgs args = new SwipeEventArgs(start_pos, diff, dir, hitObject);

        if(OnSwipe != null)
        {
            OnSwipe(this, args);

        }

        if(hitObject != null)
        {
            ISwiped iSwipe = hitObject.GetComponent<ISwiped>();
            if(iSwipe != null)
            {
                iSwipe.OnSwipe(args);
            }
        }
    }


    private void FireTapEvent()
    {
        Debug.Log("Tap-FireTapEvent");
        if (OnTap != null)
        {
            Ray r = Camera.main.ScreenPointToRay(start_pos);
            RaycastHit hit = new RaycastHit();
            GameObject hitObj = null;

            if(Physics.Raycast(r,out hit, Mathf.Infinity))
            {
                hitObj = hit.collider.gameObject;
            }
            TapEventArgs args = new TapEventArgs(start_pos,hitObj);
            OnTap(this, args);

            if (hitObj != null) {
                ITapped tapped = hitObj.GetComponent<ITapped>();
                if (tapped != null) {

                    tapped.OnTap();
                
                }
            }
        }
    }

    private void onDrawGizmos()
    {
        if (Input.touchCount > 0)
        {
            Ray r = Camera.main.ScreenPointToRay(gesture_finger.position);
            Gizmos.DrawIcon(r.GetPoint(5), "HarunaChibi");
            
            if(Input.touchCount > 1)
            {
                r = Camera.main.ScreenPointToRay(gesture_finger2.position);
                Gizmos.DrawIcon(r.GetPoint(5), "KongouChibi");
            }
        }


    }
}