using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public List<Dog> Dogs;


    public List<Place> Places;

    void Start()
    {
        for (int i = 0; i < Places.Count; i++)
        {
            Place place = Places[i];
            place.Button.GetComponentInChildren<TMP_Text>().text = $"{place.Name} ({place.Price})";
        }

        for (int i = 0; i < Dogs.Count; i++)
        {
            Dog dog = Dogs[i];
            dog.Button.GetComponentInChildren<TMP_Text>().text = $"{dog.Name} ({dog.Price})";
        }

        // Set default active tab and panel
        TabPlaceClicked();
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
                place.Button.GetComponentInChildren<TMP_Text>().text = "Bought";
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
            Instantiate(dog.DogPrefab, new Vector3(-0.49000001f, 0.17f, 1.55999994f), Quaternion.identity, dogsParent.transform);
            dog.Bought = true;
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