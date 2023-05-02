using UnityEngine;
using UnityEngine.UI;

public class TeleportCooldown : MonoBehaviour
{
    public GameObject ChargeSliderParent;
    public Slider sliderRight, sliderLeft;


    public void ActivateSliders(bool state)
    {
        ChargeSliderParent.SetActive(state);

        sliderRight.value = 0;
        sliderLeft.value = 0;

    }

    public void ChargeSliders(float minValue, float maxValue, float currentValue)
    {
        if (!ChargeSliderParent.activeSelf)
        {
            ActivateSliders(true);
        }

        maxValue -= minValue;
        currentValue -= minValue;

        //print(currentThrowForce/maxThrowForce);
        sliderRight.value = currentValue / maxValue;
        sliderLeft.value = currentValue / maxValue;
    }
}
