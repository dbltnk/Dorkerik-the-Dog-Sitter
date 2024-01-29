using UnityEngine;
using UnityEngine.UI;

public class FaceCamera : MonoBehaviour
{

    void Start()
    {
        // find the rawimage component
        RawImage rawImage = GetComponentInChildren<RawImage>();
        // set rotation to 60 0 0
        rawImage.transform.rotation = Quaternion.Euler(60, 0, 0);
    }
}
