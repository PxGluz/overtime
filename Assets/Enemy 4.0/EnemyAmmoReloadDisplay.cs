using UnityEngine;
using UnityEngine.UI;


public class EnemyAmmoReloadDisplay : MonoBehaviour
{

    public GameObject ReloadDisplayMaster;
    public Slider ReloadSlider;

    private void Start()
    {
        SliderSetActive(false);
    }

    public void SliderSetActive(bool state)
    {
        ReloadDisplayMaster.SetActive(state);
        ReloadSlider.value = 0;
    }

    public void UpdateSliderValue(float newValue)
    {
        ReloadSlider.value = newValue;
    }

}
