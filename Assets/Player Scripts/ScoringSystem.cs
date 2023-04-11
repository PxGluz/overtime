using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoringSystem : MonoBehaviour
{
    public Image comboBar;
    public TextMeshProUGUI combo, score;
    public GameObject pointsPrefab;

    public List<TextMeshProUGUI> pointsList = new List<TextMeshProUGUI>();

    public Color goodPoints, badPoints, minusPoints, scoringColor;

    public float animationSpeed, comboSmoothTime;
    public float rotationAngle, instantiateOffset;
    
    [HideInInspector] public int scoreValue = 0, comboValue = 1;

    private Vector3 ref1;
    
    public void AddScore(int addedScore, string pointType)
    {

        float calculatedCombo = comboValue + Mathf.Floor(comboBar.transform.localScale.x * 10f) / 10f;
        print(calculatedCombo);
        addedScore = (int) (addedScore * calculatedCombo);
        score.color = scoringColor;
        score.transform.localScale = Vector3.one * 2;
        score.transform.eulerAngles = Vector3.forward * Random.Range(-rotationAngle, rotationAngle);
        combo.color = scoringColor;
        combo.transform.localScale = Vector3.one * 2;
        combo.transform.eulerAngles = Vector3.forward * Random.Range(-rotationAngle, rotationAngle);
        comboBar.transform.localScale = Vector3.one;
        Vector3 offset = Vector3.right * Random.Range(-instantiateOffset, instantiateOffset) +
                         Vector3.up * Random.Range(-instantiateOffset, instantiateOffset);
        GameObject currentPoint = Instantiate(pointsPrefab, score.transform.position + offset, score.transform.rotation);
        TextMeshProUGUI currentPointText = currentPoint.GetComponent<TextMeshProUGUI>();
        pointsList.Add(currentPointText);
        switch (pointType)
        {
            case "good":
                currentPointText.color = goodPoints;
                currentPointText.text = "+";
                break;
            case "bad":
                currentPointText.color = badPoints;
                currentPointText.text = "+";
                break;
            case "minus":
                currentPointText.color = minusPoints;
                currentPointText.text = "-";
                break;
        }
        currentPointText.text += addedScore;
        scoreValue += addedScore;
        comboValue++;
    }
    
    private void Start()
    {
        comboValue = 1;
        comboBar.transform.localScale = Vector3.up + Vector3.forward;
    }

    void Update()
    {
        score.text = scoreValue.ToString();
        float calculatedCombo = comboValue + Mathf.Floor(comboBar.transform.localScale.x * 10f) / 10f;
        combo.text = "x" + calculatedCombo;
        comboBar.transform.localScale = Vector3.SmoothDamp(comboBar.transform.localScale, Vector3.up + Vector3.forward, ref ref1, comboSmoothTime);
        combo.transform.localScale = Vector3.Lerp(combo.transform.localScale, Vector3.one, animationSpeed * 10);
        combo.transform.rotation = Quaternion.Lerp(combo.transform.rotation, Quaternion.identity, animationSpeed * 10);
        combo.color = Color.Lerp(combo.color, Color.white, animationSpeed);
        score.transform.localScale = Vector3.Lerp(score.transform.localScale, Vector3.one, animationSpeed * 10);
        score.transform.rotation = Quaternion.Lerp(score.transform.rotation, Quaternion.identity, animationSpeed * 10);
        score.color = Color.Lerp(score.color, Color.white, animationSpeed);

        List<TextMeshProUGUI> newList = new List<TextMeshProUGUI>();
        foreach (TextMeshProUGUI point in pointsList)
        {
            point.transform.position += Vector3.up * animationSpeed;
            point.color = Color.Lerp(point.color, new Color(point.color.r, point.color.g, point.color.b, 0), animationSpeed * 5);
            if (point.color.a > 0)
                newList.Add(point);
            else
                Destroy(point.gameObject);
        }
        pointsList = newList;
        if (comboBar.transform.localScale.x < 0.01f && comboBar.transform.localScale != Vector3.up + Vector3.forward)
        {
            comboValue = 1;
            comboBar.transform.localScale = Vector3.up + Vector3.forward;
            combo.color = minusPoints;
        }
    }
}
