using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopSelector : MonoBehaviour
{
    void Start()
    {
        int randomIndex = Random.Range(0, transform.childCount - 1);
        transform.GetChild(randomIndex).gameObject.SetActive(true);
    }
}
