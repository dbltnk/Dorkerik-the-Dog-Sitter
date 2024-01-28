using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Security.Cryptography.X509Certificates;

public class ShopUIManager : MonoBehaviour
{
    public Button TabDogs;
    public Button TabTreats;
    public Button TabPlace;
    public GameObject PanelTabDogs;
    public GameObject PanelTabTreats;
    public GameObject PanelTabPlace;

    [System.Serializable]
    public class Place
    {
        public Button Button;
        public GameObject GameObject;
        public int Price;
        public bool Bought;
        public string Name;
    }

    [System.Serializable]
    public class Dog
    {
        public Button Button;
        public GameObject DogPrefab;
        public int Price;
        public bool Bought;
        public string Name;
    }

    [System.Serializable]
    public class Treat
    {
        public Button Button;
        public GameObject TreatPrefab;
        public int Price;
        public bool Bought;
        public string Name;
    }

    public List<Place> Places;
    public List<Dog> Dogs;
    public List<Treat> Treats;


    void Start()
    {
        TabDogsClicked();
    }

    void Update()
    {
        for (int i = 0; i < Places.Count; i++)
        {
            Place place = Places[i];
            place.Button.interactable = Scorer.Instance.CanAfford(place.Price) && !place.Bought;
        }

        for (int i = 0; i < Dogs.Count; i++)
        {
            Dog dog = Dogs[i];
            dog.Button.interactable = Scorer.Instance.CanAfford(dog.Price);
        }

        for (int i = 0; i < Treats.Count; i++)
        {
            Treat treat = Treats[i];
            treat.Button.interactable = Scorer.Instance.CanAfford(treat.Price);
        }

    }

    public void PlaceButtonClicked(int index)
    {
        Place place = Places[index];
        if (!place.Bought)
        {
            // Subtract money
            bool success = Scorer.Instance.TrySpendMoney(place.Price);

            if (success)
            {
                // Activate the game object and mark the place as bought
                place.GameObject.SetActive(true);
                place.Bought = true;

                // Update the button text and disable the button
                place.Button.interactable = false;
            }
        }
    }

    public void DogButtonClicked(int index)
    {
        Dog dog = Dogs[index];
        bool success = Scorer.Instance.TrySpendMoney(dog.Price);

        if (success)
        {
            GameObject dogsParent = GameObject.Find("Dogs");
            System.Random rand = new System.Random();
            float randomX = (float)(rand.NextDouble() * (15 - (-17)) + (-17));
            float randomZ = (float)(rand.NextDouble() * (4 - (-13)) + (-13));
            float fixedY = 0.0f;
            Instantiate(dog.DogPrefab, new Vector3(randomX, fixedY, randomZ), Quaternion.identity, dogsParent.transform);
            dog.Bought = true;
        }
    }

    public void TreatButtonClicked(int index)
    {
        Treat treat = Treats[index];
        bool success = Scorer.Instance.TrySpendMoney(treat.Price);

        if (success)
        {
            GameObject treatsParent = GameObject.Find("Treats");
            System.Random rand = new System.Random();
            float randomX = (float)(rand.NextDouble() * (15 - (-17)) + (-17));
            float randomZ = (float)(rand.NextDouble() * (4 - (-13)) + (-13));
            float fixedY = 5.0f;
            float randomYRotation = Random.Range(0f, 360f);
            Quaternion randomRotation = Quaternion.Euler(0, randomYRotation, 0);
            Instantiate(treat.TreatPrefab, new Vector3(randomX, fixedY, randomZ), randomRotation, treatsParent.transform);
            treat.Bought = true;
        }
    }

    public void TabDogsClicked()
    {
        // Activate PanelTabDogs and deactivate other panels
        PanelTabDogs.SetActive(true);
        PanelTabTreats.SetActive(false);
        PanelTabPlace.SetActive(false);

        // Change button state to indicate active tab
        TabDogs.interactable = false;
        TabTreats.interactable = true;
        TabPlace.interactable = true;
    }

    public void TabTreatsClicked()
    {
        // Activate PanelTabTreats and deactivate other panels
        PanelTabDogs.SetActive(false);
        PanelTabTreats.SetActive(true);
        PanelTabPlace.SetActive(false);

        // Change button state to indicate active tab
        TabDogs.interactable = true;
        TabTreats.interactable = false;
        TabPlace.interactable = true;
    }

    public void TabPlaceClicked()
    {
        // Activate PanelTabPlace and deactivate other panels
        PanelTabDogs.SetActive(false);
        PanelTabTreats.SetActive(false);
        PanelTabPlace.SetActive(true);

        // Change button state to indicate active tab
        TabDogs.interactable = true;
        TabTreats.interactable = true;
        TabPlace.interactable = false;
    }
}