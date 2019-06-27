using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public Vector3 startPosition;
    public float distance = 2F,
        amplitude = 0.005f;
    public bool forward = true;
    public enum OccilationFuntion { Sine, Cosine }

    private void Start()
    {
        StartCoroutine(Oscillate(OccilationFuntion.Sine, distance));
    }

    private void Update()
    {
        Debug.DrawLine(startPosition - GetComponent<Particle>().IntesityLevel()/100F * transform.forward, startPosition, GetComponent<Particle>().Color());
    }

    private IEnumerator Oscillate(OccilationFuntion method, float period = 2F)
    {
        while (true)
        {
            if (method == OccilationFuntion.Sine)
            {
                transform.position = startPosition + transform.TransformDirection(new Vector3(0, 0, amplitude * Mathf.Sin(2F * Time.time)));
            }
            else if (method == OccilationFuntion.Cosine)
            {
                transform.position = new Vector3(Mathf.Cos(Time.time), 0, 0);
            }
            yield return new WaitForEndOfFrame();
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

    private void OnEnable()
    {
        StartCoroutine(Oscillate(OccilationFuntion.Sine, distance));
    }
}
