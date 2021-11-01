using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointSlotRayManager : MonoBehaviour
{
    public delegate void TouchFunc(Point point);
    public TouchFunc callback_on_touch = null;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject obj = hit.transform.gameObject;

                if (!obj.CompareTag("point_slot"))
                {
                    Debug.Log("is not point slot");
                    return;
                }

                PointSlot point = obj.transform.parent.GetComponent<PointSlot>();
                if (point == null)
                {
                    Debug.Log("point slot is null");
                    return;
                }

                if (callback_on_touch != null)
                {
                    this.callback_on_touch(point.get_point());
                }
            }
            else
            {
                Debug.Log("is cant hit Physics Raycas");
            }
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        if(results.Count > 0)
        {
            Debug.Log("IsPointerOverUIObject");
        }
        return results.Count > 0;
    }
}
