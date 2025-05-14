using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragAction : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public Transform myTarget;
    Vector2 dragOffset = Vector2.zero;
    Vector2 halfSize = Vector2.zero;

    private void Awake()
    {
        halfSize = myTarget.GetComponent<RectTransform>().sizeDelta * 0.5f * GameManagement.Inst.myCanvas.scaleFactor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragOffset = (Vector2)myTarget.position - eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = eventData.position + dragOffset;
        pos.x = Mathf.Clamp(pos.x, halfSize.x, Screen.width - halfSize.x);
        pos.y = Mathf.Clamp(pos.y, halfSize.y, Screen.height - halfSize.y);
        myTarget.position = pos;
    }
}
