using UnityEngine;

public class IdleBobber : MonoBehaviour
{
    public float idleBobbingSpeed = 6f;
    public float bobbingAmount = 0.02f;

    private float defaultPosY = 0f;
    private float timer = 0f;

    public HeadBobber headBobber; // Reference to the HeadBobber script

    void Start()
    {
        defaultPosY = transform.localPosition.y;

        // Find the HeadBobber script if it's not set in the Inspector
        if (headBobber == null)
        {
            headBobber = GetComponent<HeadBobber>();
        }
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Check if there's no input from the horizontal or vertical axes
        if (Mathf.Approximately(horizontalInput, 0f) && Mathf.Approximately(verticalInput, 0f))
        {
            timer += Time.deltaTime * idleBobbingSpeed;
            float headBobYPosition = headBobber != null ? headBobber.GetHeadBobYPosition() : defaultPosY;
            transform.localPosition = new Vector3(transform.localPosition.x, headBobYPosition + Mathf.Sin(timer) * bobbingAmount, transform.localPosition.z);
        }
        else
        {
            timer = 0f;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultPosY, Time.deltaTime * idleBobbingSpeed), transform.localPosition.z);
        }
    }
}
