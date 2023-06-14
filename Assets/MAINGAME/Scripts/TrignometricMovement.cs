using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrignometricMovement : MonoBehaviour
{
    public static TrignometricMovement instance;
    public Vector3 Distance;
    public Vector3 MovementFrequency;
    Vector3 Moveposition;
    Vector3 startPosition;
    bool isShow = false;
    void Start()
    {
        instance = this;
        startPosition = transform.position;
        var random = Random.Range(0, 100);
        if(random > 95)
        {
            isShow = true;
            GetComponent<MeshRenderer>().enabled = true;
        }
        MovementFrequency.y = Random.Range(0.2f, 0.5f);
        Distance.y = Random.Range(1, 3);
    }
    void Update()
    {
        if (CompareTag("Wall") && isShow)
        {
            Moveposition.x = startPosition.x + Mathf.Sin(Time.timeSinceLevelLoad * MovementFrequency.x) * Distance.x;
            Moveposition.y = startPosition.y + Mathf.Sin(Time.timeSinceLevelLoad * MovementFrequency.y) * Distance.y;
            Moveposition.z = startPosition.z + Mathf.Sin(Time.timeSinceLevelLoad * MovementFrequency.z) * Distance.z;
            transform.position = new Vector3(Moveposition.x, Moveposition.y, Moveposition.z);
        }
    }
}
