using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HighlightPlus;

public class Hoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<HighlightEffect>().highlighted = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<HighlightEffect>().highlighted = false;
    }
}
