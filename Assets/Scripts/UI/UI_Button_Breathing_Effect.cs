using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Button_Breathing_Effect : MonoBehaviour
{
    [SerializeField] float cycleDuration = 2.0f; // Duration of the full color cycle (from white to black and back to white)
    [SerializeField] Image buttonImage;
    [SerializeField] Color color1;
    [SerializeField] Color color2;

    void Start()
    {
        buttonImage = GetComponent<Image>();
    }

    void Update()
    {
        float t = Mathf.PingPong(Time.time / cycleDuration, 1);

        buttonImage.color = Color.Lerp(color1, color2, t);
    }
}
