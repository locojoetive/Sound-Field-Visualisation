using System.Collections;
using UnityEngine;

public class OscillateParticle : MonoBehaviour
{
    public Vector3 startPosition;
    public float distance = 2F,
        amplitude = 0.005f;
    public bool forward = true;

    private void Start()
    {
        StartCoroutine(Oscillate(distance));
    }

    private IEnumerator Oscillate(float period = 2F)
    {
        while (true)
        {
            transform.position = startPosition + transform.TransformDirection(
                new Vector3(
                    0,
                    0,
                    amplitude * Mathf.Sin(Time.time * 2f * Mathf.PI)
                )
            );
            yield return null;
        }
    }
    
    public void setPosition(Vector3 position)
    {
        this.startPosition = position;
        transform.position = position;
    }

    public void setDirection(Vector3 intensity)
    {
        transform.forward = Vector3.Normalize(intensity);
    }

    public void setAmplitude(float intensityLevel)
    {
        amplitude = Mathf.Sign(intensityLevel)/10F * ((Mathf.Abs(intensityLevel) -60F) / 45F);

        Debug.Log("Intensity Level: " + intensityLevel);
        Debug.Log("Amplitude: " + amplitude);
    }
}
