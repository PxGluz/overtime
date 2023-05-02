using System.Collections;
using UnityEngine;

public class CrouchLogic : MonoBehaviour
{
    //Possible fix needed: should only be able to crouch while in "stop" or "walk"/"run" to fix looking wonky while crouch jumping

    [Header("Crouching")]
    [SerializeField] public Transform HeadTransform;
    private Vector3 OriginalHeadLocation;
    [SerializeField] public Transform CeilingCheck1;
    [SerializeField] public Transform CeilingCheck2;
    [SerializeField] public float CeilingCheckRadius;
    private float initialHeight;
    public CapsuleCollider playerCapsuleCollider;
    [SerializeField][Range(0.0f, 2f)] public float CrouchAnimationDuration;

    [HideInInspector] public bool hasSpaceAboveHead;
    public bool hasEnteredCrouch = false;


    void Start()
    {
        initialHeight = playerCapsuleCollider.height;
        OriginalHeadLocation = HeadTransform.localPosition;
    }



    private IEnumerator enterCrouchAnim, exitCrouchAnim;
    public void enterCrouch()
    {
        Player.m.MoveType = "crouch";
        hasEnteredCrouch = true;
        playerCapsuleCollider.height = 1.1f;
        playerCapsuleCollider.center = new Vector3(playerCapsuleCollider.center.x, -0.45f, playerCapsuleCollider.center.z);

        enterCrouchAnim = MoveHeadToPos(new Vector3(0, -0.5f, 0), CrouchAnimationDuration);

        StartCoroutine(enterCrouchAnim);

        if (exitCrouchAnim != null)
            StopCoroutine(exitCrouchAnim);

    }

    public void enterCrouchInstantly()
    {
        Player.m.MoveType = "crouch";
        hasEnteredCrouch = true;
        playerCapsuleCollider.height = 1.1f;
        playerCapsuleCollider.center = new Vector3(playerCapsuleCollider.center.x, -0.45f, playerCapsuleCollider.center.z);
        //enterCrouchAnim = MoveHeadToPos(new Vector3(0, -0.5f, 0), CrouchAnimationDuration);

        //StartCoroutine(enterCrouchAnim);

        HeadTransform.localPosition = new Vector3(0, -0.5f, 0);

        if (exitCrouchAnim != null)
            StopCoroutine(exitCrouchAnim);

    }

    public void exitCrouch()
    {
        Player.m.MoveType = "stop";
        hasEnteredCrouch = false;
        playerCapsuleCollider.height = initialHeight;
        playerCapsuleCollider.center = new Vector3(playerCapsuleCollider.center.x, 0, playerCapsuleCollider.center.z);

        exitCrouchAnim = MoveHeadToPos(new Vector3(0, OriginalHeadLocation.y, 0), CrouchAnimationDuration);

        StartCoroutine(exitCrouchAnim);

        if (enterCrouchAnim != null)
            StopCoroutine(enterCrouchAnim);

    }

    public IEnumerator MoveHeadToPos(Vector3 targetPosition, float duration)
    {
        Vector3 previousPosition = HeadTransform.localPosition;
        float time = 0.0f;

        do
        {
            time += Time.deltaTime;

            HeadTransform.localPosition = Vector3.Lerp(previousPosition, targetPosition, time / duration);

            yield return 0;

        } while (time < duration);

        HeadTransform.localPosition = targetPosition;

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(CeilingCheck1.position, CeilingCheckRadius);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(CeilingCheck2.position, CeilingCheckRadius);
    }
}
