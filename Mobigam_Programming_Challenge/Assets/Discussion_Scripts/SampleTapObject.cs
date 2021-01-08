using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleTapObject : MonoBehaviour, ITapped,ISwiped,IDragged,IPinchSpread,IRotate
{
    public Vector3 TargetPosition;
    public float speed = 10f;

    public void OnEnable() {

        TargetPosition = transform.position;
    
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, TargetPosition, speed * Time.deltaTime);
    }

    public void OnTap()
    {

        Destroy(gameObject);
    }

    public void OnSwipe(SwipeEventArgs args)
    {
        Vector3 move_dir = Vector3.zero;
        
        move_dir = args.RawDirection.normalized;

        TargetPosition = TargetPosition + (move_dir * 5);
    }

    public void OnDrag(DragEventArgs args)
    {
        Ray r = Camera.main.ScreenPointToRay(args.TargetFinger.position);
        Vector3 point = r.GetPoint(10);

        transform.position = point;
        TargetPosition = point;
    }

    public float scaleSpeed = 5f;
    public void OnPinchSpread(SpreadEventArgs args)
    {
        if(args.HitObject == gameObject)
        {
            float scale = (args.DistanceDiff / Screen.dpi) *scaleSpeed;
            Vector3 scaleVector = new Vector3(scale, scale, scale);

            transform.localScale += scaleVector;
        }    
        
    }

    public void onRotate(RotateEventArgs args)
    {
        float angle = args.Angle * speed;
        if(args.RotationDirection == RotationDirections.CW)
        {
            angle *= -1;
        }

        Vector3 rot = new Vector3(0, 0, angle);
        transform.Rotate(rot);
    }
}
