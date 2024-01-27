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
    }

    public List<Place> Places;

    void Start()
    {
        // Initialize places
        for (int i = 0; i < Places.Count; i++)
        {
            Place place = Places[i];
            place.Button.GetComponentInChildren<TMP_Text>().text = $"Buy ({place.Price})";
        }

        // Set default active tab and panel
        TabPlaceClicked();
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

                // Disable the button
                place.Button.interactable = false;
            }

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