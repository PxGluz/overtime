using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{

    public EnemyMaster enemy;
    public Slider HealthBarSlider;
    public Slider HealthBarBackGroundSlider;

    public bool healthBarTransition;
    public IEnumerator ChangeHealthBackGroundRef;

    public void Start()
    {
        setSliderMax(HealthBarSlider, enemy.maxHealth);
        setSliderMax(HealthBarBackGroundSlider, enemy.maxHealth);
    }

    private void setSliderMax(Slider slider, float maxVal)
    {
        slider.maxValue = maxVal;
        slider.value = maxVal;
    }

    public void activateHealthSliders(bool enable)
    {
        HealthBarSlider.gameObject.SetActive(enable);
        HealthBarBackGroundSlider.gameObject.SetActive(enable);
    }

    public void UpdateHealthBar(float healthAfterTakingDamage)
    {
        HealthBarSlider.value = healthAfterTakingDamage;

        if (healthBarTransition && ChangeHealthBackGroundRef != null)
        {
            StopCoroutine(ChangeHealthBackGroundRef);
        }

        ChangeHealthBackGroundRef = ChangeHealthBackGround(HealthBarBackGroundSlider.value, healthAfterTakingDamage);
        StartCoroutine(ChangeHealthBackGroundRef);
    }

    public IEnumerator ChangeHealthBackGround(float healthBackgroundWhenCalled, float newEnemyCurrentHealth)
    {
        healthBarTransition = true;

        yield return new WaitForSeconds(0.2f);

        float TransitionTime = 0.5f;

        for (float t = 0f; t < TransitionTime; t += Time.deltaTime)
        {
            float newValue = Mathf.Lerp(healthBackgroundWhenCalled, newEnemyCurrentHealth, t / TransitionTime);
            HealthBarBackGroundSlider.value = newValue;
            yield return null;
        }

        HealthBarBackGroundSlider.value = newEnemyCurrentHealth;

        healthBarTransition = false;

        if (enemy.isDead)
        {
            //yield return new WaitForSeconds(0.2f);
            HealthBarBackGroundSlider.gameObject.SetActive(false);
            HealthBarSlider.gameObject.SetActive(false);
        }

    }
}
