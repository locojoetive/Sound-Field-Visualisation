using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public GameObject pointPrefab;
    public Transform[] particles = new Transform[0];
    public int numberOfPoints;
    public float intensityLevelThreshold = 60.0f;
    public bool sorted = false,
        calculate = false;
    public GameObject anchor;

    void Start()
    {
        if(particles.Length == 0)
        {
            particles = new Transform[numberOfPoints];
            for(int indexOfParticle = 0; indexOfParticle < numberOfPoints; indexOfParticle++)
            {
                ConvertMeasurementPointToPrefab(indexOfParticle);
            }
        }
        if (!sorted)
        {
            particles = MergeSort(particles, false);
            sorted = true;
        }


        /*
        for (int i = 0; i < particles.Length; i++)
        {
            if (particles[i].GetComponent<Particle>().intensityLevel < intensityLevelThreshold)
                break;
            // TODOS:
        }
        */
    }

    private void Update()
    {
        if (calculate)
        {
            MeasureAverageDistance();
            calculate = false;
        }
        for (int i = 0; i < particles.Length; i++) {
            if(particles[i].GetComponent<Particle>().IntesityLevel() < intensityLevelThreshold)
            {
                particles[i].gameObject.SetActive(false);
            } else if (!particles[i].gameObject.activeSelf){
                particles[i].gameObject.SetActive(true);
            }
        }
    }

    private void MeasureAverageDistance()
    {
        Vector3 pos1, pos2,
            dir1, dir2;
        List<float> distances = new List<float>();
        int border = 0;
        for (int i = 0; i < particles.Length; i++)
        {
            if (particles[i].GetComponent<Particle>().intensityLevel < intensityLevelThreshold)
            {
                border = i;
                Debug.Log("Hoer auf bei: " + border);
                break;
            }
        }
        for (int i = 0; i < border; i++)
        {

            for (int j = i; j < border; j++)
            {
                if (particles[j].GetComponent<Particle>().intensityLevel < intensityLevelThreshold)
                    break;
                if(i != j)
                {
                    pos1 = particles[i].position;
                    dir1 = particles[i].forward;
                    pos2 = particles[j].position;
                    dir2 = particles[j].forward;
                    distances.Add(skewDistance(pos1, dir1, pos2, dir2));
                }
            }
        }
        Debug.Log(Average(distances));
    }

    private float Average(List<float> of)
    {
        float sum = 0.0f;
        foreach(float f in of)
        {
            sum += f;
        }
        return sum / of.Count;
    }

    public bool linearlyIndependent(Vector3 v1, Vector3 v2)
    {
        float x, y, z;
        x = v1.x / v2.x;
        y = v1.y / v2.y;
        z = v1.z / v2.z;

        if (x == y && y == z)
            return true;
        else
            return false;
    }

    public float skewDistance(Vector3 position1, Vector3 direction1, Vector3 position2, Vector3 direction2)
    {
        // the vectors are interpreted as a line defined by: position1 + x * direction1 && position2 + y * direction2
        float x, y;
        float k1 = Vector3.Dot(direction1, direction1),
            k2 = Vector3.Dot(direction1, direction2),
            l1 = -k2,
            l2 = Vector3.Dot(direction2, direction2),
            m1 = Vector3.Dot(direction1, position1 - position2),
            m2 = Vector3.Dot(direction2, position1 - position2);
        y = m2 / ((-k2 * (l1 + m1)) / k1 + l2);
        x = (-y * l1 - m1) / k1;
        Vector3 anchor1 = position1 + x * direction1,
            anchor2 = position2 + y * direction2;
        Debug.Log("Punkt P1: " + anchor1);
        Debug.Log("Punkt P2: " + anchor2);
        Vector3 distance = anchor1 - (anchor2);
        Transform test = Instantiate(anchor, anchor2 + 0.5f * distance, Quaternion.identity).GetComponent<Transform>();
        Debug.Log("Mitte bei: " + test.position);
        return distance.magnitude;
    }


    private static Transform[] AddFirstElement(Transform[] of, Transform[] to)
    {
        Transform[] result = new Transform[to.Length + 1];
        for (int i = 0; i < to.Length; i++)
        {
            result[i] = to[i];
        }
        result[to.Length] = of[0];
        return result;
    }

    private static Transform[] RemoveFirstElement(Transform[] of)
    {
        Transform[] result = new Transform[of.Length - 1];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = of[i + 1];
        }
        return result;
    }

    private static Transform[] MergeSort(Transform[] unsorted, bool ascending = true)
    {
        if (unsorted.Length <= 1)
            return unsorted;

        int middle = unsorted.Length / 2;
        Transform[] left = new Transform[middle];
        Transform[] right = unsorted.Length % 2 == 0 
            ? right = new Transform[middle]
            : right = new Transform[middle + 1];

        for (int i = 0; i < middle; i++)  //Dividing the unsorted list
        {
            left[i] = unsorted[i];
        }
        for (int i = middle; i < unsorted.Length; i++)
        {
            right[i-middle] = unsorted[i];
        }
        
        left = MergeSort(left, ascending);
        right = MergeSort(right, ascending);
        if(ascending)
            return Merge(left, right);
        else
            return MergeDescending(left, right);
    }

    private static Transform[] Merge(Transform[] left, Transform[] right)
    {
        Transform[] result = new Transform[0];

        while (left.Length > 0 || right.Length > 0)
        {
            if (left.Length > 0 && right.Length > 0)
            {
                if (left[0].GetComponent<Particle>().intensityLevel <= right[0].GetComponent<Particle>().intensityLevel)  //Comparing First two elements to see which is smaller
                {
                    result = AddFirstElement(left, result);
                    left = RemoveFirstElement(left);
                }
                else
                {
                    result = AddFirstElement(right, result);
                    right = RemoveFirstElement(right);
                }
            }
            else if (left.Length > 0)
            {
                result = AddFirstElement(left, result);
                left = RemoveFirstElement(left);
            }
            else if (right.Length > 0)
            {
                result = AddFirstElement(right, result);
                right = RemoveFirstElement(right);
            }
        }
        return result;
    }
    
    private static Transform[] MergeDescending(Transform[] left, Transform[] right)
    {
        Transform[] result = new Transform[0];

        while (left.Length > 0 || right.Length > 0)
        {
            if (left.Length > 0 && right.Length > 0)
            {
                if (left[0].GetComponent<Particle>().intensityLevel >= right[0].GetComponent<Particle>().intensityLevel)  //Comparing First two elements to see which is bigger
                {
                    result = AddFirstElement(left, result);
                    left = RemoveFirstElement(left);
                }
                else
                {
                    result = AddFirstElement(right, result);
                    right = RemoveFirstElement(right);
                }
            }
            else if (left.Length > 0)
            {
                result = AddFirstElement(left, result);
                left = RemoveFirstElement(left);
            }
            else if (right.Length > 0)
            {
                result = AddFirstElement(right, result);
                right = RemoveFirstElement(right);
            }
        }
        return result;
    }

    private void ConvertMeasurementPointToPrefab(int measurementPointIndex)
    {
        Transform particle = Instantiate(pointPrefab, Vector3.zero, Quaternion.identity, transform).transform;
        string fileName = "speakerTest2/measurepoint_" + (measurementPointIndex+1) + ".bytes";
        BinaryReader binaryReader;
        try
        {
            binaryReader = new BinaryReader(new FileStream(fileName, FileMode.Open));
            double[][] micSoundSignals = new double[4][];
            Vector3 position = new Vector3();
            Quaternion rotation = new Quaternion();
            for (int micIndex = 0; micIndex < 4; micIndex++)
            {
                micSoundSignals[micIndex] = new double[4096];
                for (int sampleIndex = 0; sampleIndex < 4096; sampleIndex++)
                {
                    micSoundSignals[micIndex][sampleIndex] = binaryReader.ReadDouble();
                }
            }
            position.x = (float) binaryReader.ReadDouble();
            position.y = (float) binaryReader.ReadDouble();
            position.z = (float) binaryReader.ReadDouble();

            rotation.w = (float) binaryReader.ReadDouble();
            rotation.x = (float) binaryReader.ReadDouble();
            rotation.y = (float) binaryReader.ReadDouble();
            rotation.z = (float) binaryReader.ReadDouble();

            particle.GetComponent<Particle>().InitializeParticle(micSoundSignals, position, rotation);
            particles[measurementPointIndex] = particle;
        }
        catch(IOException e)
        {
            Debug.Log("File " + e.Message + " does not exist");
            return;
        }
        binaryReader.Dispose();
    }
}
