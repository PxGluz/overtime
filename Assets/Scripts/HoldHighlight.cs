using UnityEngine;

public class HoldHighlight : MonoBehaviour
{

    private Outline myOutLine;
    // Start is called before the first frame update

    private float destinationColor = 0, refFloat;
    [SerializeField] private float smoothTime;
    private Interact interact = null;

    void Start()
    {
        if (gameObject.TryGetComponent(out Interactable inter))
            interact = Player.m.interact;
        if (!TryGetComponent(out myOutLine))
            Debug.LogError("Outline script shouldn't be on this object! (" + gameObject.name + ")");
    }

    // Update is called once per frame
    void Update()
    {
        if (myOutLine == null)
            return;

        if (Input.GetMouseButton(2))
        {
            destinationColor = 1;
            Player.m.Slowing();
        }
        else
        {
            destinationColor = 0;
            Player.m.Fasting();
        }
        destinationColor = Mathf.SmoothDamp(myOutLine.OutlineColor.a, destinationColor, ref refFloat, smoothTime);
        if (!interact || (interact && interact.itemBeingPickedUp != gameObject.GetComponent<Interactable>()))
            myOutLine.OutlineColor = new Color(myOutLine.OutlineColor.r, myOutLine.OutlineColor.g, myOutLine.OutlineColor.b, destinationColor);
    }
}
