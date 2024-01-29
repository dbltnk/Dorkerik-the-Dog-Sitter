using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopSelector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // randomly activate one of the child gameobjects
        int randomIndex = Random.Range(0, transform.childCount);
        transform.GetChild(randomIndex).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
