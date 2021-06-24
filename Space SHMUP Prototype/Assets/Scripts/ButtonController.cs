using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private bool isPressed;
    public bool IsPressed => isPressed;

    public void OnPointerDown()
    {
        isPressed = true;
    }
    public void OnPointerUp()
    {
        isPressed = false;
    }

}
