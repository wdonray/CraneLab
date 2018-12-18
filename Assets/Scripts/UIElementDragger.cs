using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR;

public class UIElementDragger : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] private Vector3 origin;

    public void OnDrag(PointerEventData eventData)
    {
        if (origin == Vector3.zero)
        {
            origin = transform.localPosition;
        }
        transform.SetAsLastSibling();
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform, eventData.position,
            Camera.main, out pos);
        transform.position = transform.parent.transform.TransformPoint(pos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetAsFirstSibling();
        transform.localPosition = origin;
    }

    public void CycleActive(Transform transform)
    {
        transform.gameObject.SetActive(!transform.gameObject.activeInHierarchy);
    }
}
