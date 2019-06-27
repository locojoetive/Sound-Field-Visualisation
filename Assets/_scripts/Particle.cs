using System;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public double[][] micSoundSignals;
    public float intensityLevel = 0.0f;
    public Vector3 intensity = Vector3.zero;
    public Quaternion micRotation;
    public Color color;
    private Material material = null;


    public void Awake()
    {
        if (GetComponent<MeshRenderer>() != null)
        {
            this.material = GetComponent<MeshRenderer>().material;
            if (!this.material.color.Equals(this.color))
                this.material.color = color;
        }
    }

    public void InitializeParticle(double[][] soundSignal, Vector3 position, Quaternion rotation)
    {
        setSoundSignal(soundSignal);
        setIntensity();
        setIntensityLevel();
        setColor();
        setMicRotation(rotation);
        GetComponent<Move>().setPosition(position);
        GetComponent<Move>().setDirection(intensity);
    }


    private void setSoundSignal(double[][] micSoundSignals)
    {
        this.micSoundSignals = micSoundSignals;
    }

    private void setMicRotation(Quaternion rotation)
    {
        this.micRotation = rotation;
        transform.rotation = rotation;
    }

    private void setIntensity()
    {
        intensity = AcousticsMath.CrossSpectrumMethod(micSoundSignals, 44100, 12, 707F, 1414F, 1.1717, 0.05f);
    }

    private void setIntensityLevel()
    {
        intensityLevel = AcousticsMath.CalcuIntensityLevel(intensity);
        GetComponent<Move>().setAmplitude(intensityLevel);
    }
    
    private void setColor()
    {
        color = ColorBar.DefineColor(2, IntesityLevel(), 0, 86);
        if(material != null)
            material.color = color;
    }

    public double SoundSignal(int micIndex, int sampleIndex)
    {
        return this.micSoundSignals[micIndex][sampleIndex];
    }

    public double[][] SoundSignals()
    {
        return this.micSoundSignals;
    }

    public float IntesityLevel()
    {
        return intensityLevel;
    }

    public Vector3 Intesity()
    {
        return intensity;
    }

    public Color Color()
    {
        return color;
    }
}
