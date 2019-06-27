using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropagateSpheres : MonoBehaviour
{
    public GameObject propagatingObject;
    public int numberOfObjects;
    public float distance;
    
    void Start()
    {
        float angleSteps = 360F / (float) numberOfObjects,
            currentAngleZ = 0.0f,
            currentAngleY = 0.0f;

        for (int i = 0; i < numberOfObjects; i++)
        {
            for (int j = 0; j < numberOfObjects; j++)
            {
                Debug.Log(angleSteps);
                float x = Mathf.Cos(currentAngleZ) * distance,
                    y = Mathf.Sin(currentAngleZ) * distance;
                Vector3 direction = new Vector3(x, y, 0F);
                GameObject.Instantiate(
                    propagatingObject,
                    transform.position,
                    Quaternion.Euler(0F, currentAngleY, currentAngleZ),
                    transform
                );
                currentAngleZ += angleSteps;
            }
            currentAngleZ = 0F;
            currentAngleY += angleSteps;
        }
    }
}
