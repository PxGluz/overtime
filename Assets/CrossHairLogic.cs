using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrossHairLogic : MonoBehaviour
{
    public RectTransform crossHair;

    public float currentSize;
    public float restingSize;
    public float sizeMultiplier;
    public float reachMaxSizeSpeed;
    public float reachRestingSizeSpeed;

    [Header("Hit X effect")]
    public Image XImage;
    public float fadeDuration;
    public float currentOppacity;
    public Color HeadShotXColor;
    private Color normalXColor;

    private void Start()
    {
        XImage.color = new Color(XImage.color.r, XImage.color.g, XImage.color.b,0);

        restingSize = crossHair.sizeDelta.x;
        currentSize = restingSize;

        normalXColor = XImage.color;
    }

    IEnumerator fadeXRef;
    public void ActivateHitXEffect(bool isHeadShot)
    {
        if (fadeXRef != null)
        {
            StopCoroutine(fadeXRef);
        }

        fadeXRef = fadeX(isHeadShot);
        StartCoroutine(fadeXRef);
    }

    private IEnumerator fadeX(bool isHeadShot)
    {
        if (isHeadShot)
            XImage.color = HeadShotXColor;
        else
            XImage.color = normalXColor;

        XImage.color = new Color(XImage.color.r, XImage.color.g, XImage.color.b, 1);
        currentOppacity = 1;

        yield return new WaitForSeconds(0.1f);

        float time = 0.0f;
        do
        {
            time += Time.deltaTime;

            currentOppacity = Mathf.Lerp(currentOppacity, 0, time / fadeDuration);
            XImage.color = new Color(XImage.color.r, XImage.color.g, XImage.color.b, currentOppacity);

            yield return 0;

        } while (time < fadeDuration);

        XImage.color = new Color(XImage.color.r, XImage.color.g, XImage.color.b, 0);
    }


    IEnumerator transitionToSizeRef;
    public void ActivateCrossHairEffect()
    {
        if (transitionToSizeRef != null)
        {
            StopCoroutine(transitionToSizeRef);
        }

        transitionToSizeRef = transitionToSize();
        StartCoroutine(transitionToSizeRef);

    }

    private IEnumerator transitionToSize()
    {
        float time = 0.0f;
        do
        {
            time += Time.deltaTime;

            currentSize = Mathf.Lerp(currentSize, restingSize * sizeMultiplier, time / reachMaxSizeSpeed);

            crossHair.sizeDelta = new Vector2(currentSize,currentSize); 

            yield return 0;

        } while (time < reachMaxSizeSpeed);

        time = 0.0f;
        do
        {
            time += Time.deltaTime;

            currentSize = Mathf.Lerp(currentSize, restingSize, time / reachRestingSizeSpeed);

            crossHair.sizeDelta = new Vector2(currentSize, currentSize);

            yield return 0;

        } while (time < reachRestingSizeSpeed);
    }
    
}
