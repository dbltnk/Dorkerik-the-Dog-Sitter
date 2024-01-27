using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    public Button TabDogs;
    public Button TabTreats;
    public Button TabPlace;
    public GameObject PanelTabDogs;
    public GameObject PanelTabTreats;
    public GameObject PanelTabPlace;

    void Start()
    {
        // Set default active tab and panel
        TabDogsClicked();
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