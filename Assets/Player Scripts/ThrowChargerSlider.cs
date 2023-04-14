using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowChargerSlider : MonoBehaviour
{
    public GameObject ChargeSliderParent;
    public Slider sliderRight, sliderLeft;


    public void ActivateSliders(bool state)
    {
        ChargeSliderParent.SetActive(state);
        
        sliderRight.value = 0;
        sliderLeft.value = 0;
        
    }

    public void ChargeSliders(float minThrowForce, float maxThrowForce, float currentThrowForce)
    {
        if (!ChargeSliderParent.activeSelf)
        {
            ActivateSliders(true);
            print("muie");
        }  

        maxThrowForce -= minThrowForce;
        currentThrowForce -= minThrowForce;

        //print(currentThrowForce/maxThrowForce);
        sliderRight.value = currentThrowForce / maxThrowForce;
        sliderLeft.value = currentThrowForce / maxThrowForce;
    }

}
