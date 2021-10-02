using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer entered");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer exited");
    }
}
