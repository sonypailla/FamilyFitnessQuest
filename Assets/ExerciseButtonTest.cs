
using UnityEngine;
using UnityEngine.EventSystems;

public class ExerciseButtonTest : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("BUTTON CLICK DETECTED!");
    }
}