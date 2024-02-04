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
    private AudioSource audioSource;

    public AudioClip clickSound;
    public AudioClip buyPlaceSound;
    public AudioClip buyDogSound;
    public AudioClip buyTreatSound;

    public Sprite QuestionMarkSprite;

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
        public GameObject CorrespondingGameObject;
        public Sprite OriginalSprite;
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

    public enum ActiveTab
    {
        Dogs,
        Treats,
        Places
    }

    private ActiveTab currentTab;

    private void Awake()
    {
        audioSource = GetComponentInParent<AudioSource>();

        for (int i = 0; i < Dogs.Count; i++)
        {
            Dog dog = Dogs[i];
            dog.OriginalSprite = dog.Button.image.sprite;
        }
    }

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

        // Check for key presses from 1 to 9
        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                // Depending on the currently active tab and the key pressed, call the corresponding button click method
                switch (currentTab)
                {
                    case ActiveTab.Dogs:
                        if (i - 1 < Dogs.Count)
                        {
                            DogButtonClicked(i - 1, true);
                        }
                        break;
                    case ActiveTab.Treats:
                        if (i - 1 < Treats.Count)
                        {
                            TreatButtonClicked(i - 1, true);
                        }
                        break;
                    case ActiveTab.Places:
                        if (i - 1 < Places.Count)
                        {
                            PlaceButtonClicked(i - 1);
                        }
                        break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // If the Tab key is pressed, call the CycleTabs method
            CycleTabs();
        }

        for (int i = 0; i < Dogs.Count; i++)
        {
            Dog dog = Dogs[i];
            if (dog.CorrespondingGameObject == null || dog.CorrespondingGameObject.activeInHierarchy)
            {
                dog.Button.interactable = Scorer.Instance.CanAfford(dog.Price);
                dog.Button.image.sprite = dog.OriginalSprite; // Set the sprite to the original one
            }
            else
            {
                dog.Button.interactable = false;
                dog.Button.image.sprite = QuestionMarkSprite; // Set the sprite to the question mark symbol
            }
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
                audioSource.PlayOneShot(buyPlaceSound);
            }
        }
    }

    public void DogButtonClickedUI(int index)
    {
        DogButtonClicked(index, false);
    }

    public void TreatButtonClickedUI(int index)
    {
        TreatButtonClicked(index, false);
    }

    public void DogButtonClicked(int index, bool isKeyboardInput = false)
    {
        Dog dog = Dogs[index];
        bool success = Scorer.Instance.TrySpendMoney(dog.Price);

        if (success)
        {
            GameObject dogsParent = GameObject.Find("Dogs");

            Vector3 spawnPosition = Vector3.zero;
            if (isKeyboardInput)
            {
                // Get the mouse position in screen space
                Vector3 mousePosition = Input.mousePosition;

                // Create a ray from the camera to the mouse position
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);

                // Define the distance from the hit point to the spawn position
                float distanceToGround = 0.17f; // Adjust this value as needed

                // Perform the raycast
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    // If the raycast hits something, set the spawn position a fixed distance above the hit point
                    spawnPosition = hit.point + new Vector3(0, distanceToGround, 0);
                } 
            }
            else
            {
                System.Random rand = new System.Random();
                float randomX = (float)(rand.NextDouble() * (15 - (-17)) + (-17));
                float randomZ = (float)(rand.NextDouble() * (4 - (-13)) + (-13));
                float fixedY = 0.0f;
                spawnPosition = new Vector3(randomX, fixedY, randomZ);
            }

            Instantiate(dog.DogPrefab, spawnPosition, Quaternion.identity, dogsParent.transform);
            dog.Bought = true;
            audioSource.PlayOneShot(buyDogSound);
        }
    }

    public void TreatButtonClicked(int index, bool isKeyboardInput = false)
    {
        Treat treat = Treats[index];
        bool success = Scorer.Instance.TrySpendMoney(treat.Price);

        if (success)
        {
            GameObject treatsParent = GameObject.Find("Treats");

            Vector3 spawnPosition = Vector3.zero;
            if (isKeyboardInput)
            {
                // Get the mouse position in screen space
                Vector3 mousePosition = Input.mousePosition;

                // Create a ray from the camera to the mouse position
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);

                // Define the distance from the hit point to the spawn position
                float distanceToGround = 1f; // Adjust this value as needed

                // Perform the raycast
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    // If the raycast hits something, set the spawn position a fixed distance above the hit point
                    spawnPosition = hit.point + new Vector3(0, distanceToGround, 0);
                } 
            }
            else
            {
                System.Random rand = new System.Random();
                float randomX = (float)(rand.NextDouble() * (15 - (-17)) + (-17));
                float randomZ = (float)(rand.NextDouble() * (4 - (-13)) + (-13));
                float fixedY = 5.0f;
                spawnPosition = new Vector3(randomX, fixedY, randomZ);
            }

            float randomYRotation = Random.Range(0f, 360f);
            Quaternion randomRotation = Quaternion.Euler(0, randomYRotation, 0);
            Instantiate(treat.TreatPrefab, spawnPosition, randomRotation, treatsParent.transform);
            treat.Bought = true;
            audioSource.PlayOneShot(buyTreatSound);
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

        audioSource.PlayOneShot(clickSound);
        currentTab = ActiveTab.Dogs;
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

        audioSource.PlayOneShot(clickSound);
        currentTab = ActiveTab.Treats;
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

        audioSource.PlayOneShot(clickSound);
        currentTab = ActiveTab.Places;
    }

    void CycleTabs()
    {
        // Change currentTab to the next tab and call the corresponding tab click method
        switch (currentTab)
        {
            case ActiveTab.Dogs:
                currentTab = ActiveTab.Treats;
                TabTreatsClicked();
                break;
            case ActiveTab.Treats:
                currentTab = ActiveTab.Places;
                TabPlaceClicked();
                break;
            case ActiveTab.Places:
                currentTab = ActiveTab.Dogs;
                TabDogsClicked();
                break;
        }
    }
}