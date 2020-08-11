using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.

    [SerializeField]
    private  Transform camTransform;

    // How long the object should shake for.
    [SerializeField]
    private float shakeDuration = 1f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    [SerializeField]
    private float shakeAmount = 0.7f;
    [SerializeField]
    private float decreaseFactor = 0.1f;


    private Vector3 originalPos;

    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    public void CreateCameraShake()
    {
        while (shakeDuration > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }

        shakeDuration = 0f;
        camTransform.localPosition = originalPos;
    }
}