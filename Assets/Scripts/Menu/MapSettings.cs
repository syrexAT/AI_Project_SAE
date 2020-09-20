using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapSettings : MonoBehaviour
{
    public Toggle randomSeedToggle;
    //public Dropdown animalDropDown;
    //public Dropdown predatorDropDown;
    public TMP_Dropdown animalDropDown;
    public TMP_Dropdown predatorDropDown;

    public bool smallAnimalDropDown = false;
    public bool mediumAnimalDropDown = false;
    public bool largeAnimalDropDown = false;

    public bool smallPredatorDropDown = false;
    public bool mediumPredatorDropDown = false;
    public bool largePredatorDropDown = false;

    public bool randomSeed = false;

    public TMP_InputField seedInputField;

    // Start is called before the first frame update
    void Start()
    {
        mediumAnimalDropDown = true;
        mediumPredatorDropDown = true;
        GetAnimalAmount();
        GetPredatorAmount();
        GetRandomSeedToggle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetAnimalAmount()
    {
        if (animalDropDown.value == 0) //small
        {
            smallAnimalDropDown = true;
            mediumAnimalDropDown = false;
            largeAnimalDropDown = false;
            //PlayerPrefs.SetInt("smallAnimalBool", smallAnimalDropDown ? 1 : 0);
            PlayerPrefExtension.SetBool("smallAnimalBool", true);
            PlayerPrefExtension.SetBool("mediumAnimalBool", false);
            PlayerPrefExtension.SetBool("largeAnimalBool", false);
            //PlayerPrefs.SetFloat("AnimalAmount", 0.0005f);
            PlayerPrefs.Save();
        }

        if (animalDropDown.value == 1)
        {
            mediumAnimalDropDown = true;
            smallAnimalDropDown = false;
            largeAnimalDropDown = false;
            //PlayerPrefs.SetInt("mediumAnimalBool", mediumAnimalDropDown ? 1 : 0);
            PlayerPrefExtension.SetBool("mediumAnimalBool", true);
            PlayerPrefExtension.SetBool("smallAnimalBool", false);
            PlayerPrefExtension.SetBool("largeAnimalBool", false);
            //PlayerPrefs.SetFloat("AnimalAmount", 0.0005f);
            PlayerPrefs.Save();
        }

        if (animalDropDown.value == 2)
        {
            largeAnimalDropDown = true;
            mediumAnimalDropDown = false;
            smallAnimalDropDown = false;
            //PlayerPrefs.SetInt("largeAnimalBool", largeAnimalDropDown ? 1 : 0);
            PlayerPrefExtension.SetBool("largeAnimalBool", true);
            PlayerPrefExtension.SetBool("smallAnimalBool", false);
            PlayerPrefExtension.SetBool("mediumAnimalBool", false);
            //PlayerPrefs.SetFloat("AnimalAmount", 0.0005f);
            PlayerPrefs.Save();
        }
    }

    public void GetPredatorAmount()
    {
        if (predatorDropDown.value == 0) //small
        {
            smallPredatorDropDown = true;
            mediumPredatorDropDown = false;
            largePredatorDropDown = false;
            //PlayerPrefs.SetInt("smallAnimalBool", smallAnimalDropDown ? 1 : 0);
            PlayerPrefExtension.SetBool("smallPredatorBool", true);
            PlayerPrefExtension.SetBool("mediumPredatorBool", false);
            PlayerPrefExtension.SetBool("largePredatorBool", false);
            //PlayerPrefs.SetFloat("AnimalAmount", 0.0005f);
            PlayerPrefs.Save();
        }

        if (predatorDropDown.value == 1)
        {
            mediumPredatorDropDown = true;
            smallPredatorDropDown = false;
            largePredatorDropDown = false;
            //PlayerPrefs.SetInt("mediumAnimalBool", mediumAnimalDropDown ? 1 : 0);
            PlayerPrefExtension.SetBool("mediumPredatorBool", true);
            PlayerPrefExtension.SetBool("smallPredatorBool", false);
            PlayerPrefExtension.SetBool("largePredatorBool", false);
            //PlayerPrefs.SetFloat("AnimalAmount", 0.0005f);
            PlayerPrefs.Save();
        }

        if (predatorDropDown.value == 2)
        {
            largePredatorDropDown = true;
            mediumPredatorDropDown = false;
            smallPredatorDropDown = false;
            //PlayerPrefs.SetInt("largeAnimalBool", largeAnimalDropDown ? 1 : 0);
            PlayerPrefExtension.SetBool("largePredatorBool", true);
            PlayerPrefExtension.SetBool("smallPredatorBool", false);
            PlayerPrefExtension.SetBool("mediumPredatorBool", false);
            //PlayerPrefs.SetFloat("AnimalAmount", 0.0005f);
            PlayerPrefs.Save();
        }
    }

    public void GetRandomSeedToggle()
    {
        if (randomSeedToggle.isOn)
        {
            randomSeed = true;
            PlayerPrefExtension.SetBool("randomSeedBool", true);
            PlayerPrefs.Save();

        }
        else if (!randomSeedToggle.isOn)
        {
            randomSeed = false;
            PlayerPrefExtension.SetBool("randomSeedBool", false);
            PlayerPrefs.Save();
        }
    }

    public void SetSeedFromInput()
    {
        string seedInput = seedInputField.text;
        int seedInputInt;
        int.TryParse(seedInput, out seedInputInt);
        if (seedInputInt == 0)
        {
            randomSeedToggle.isOn = true;
            PlayerPrefExtension.SetBool("randomSeedBool", true);
        }
        else if (seedInputInt >= 0)
        {
            randomSeedToggle.isOn = false;
            PlayerPrefExtension.SetBool("randomSeedBool", false);
        }
        Debug.Log(seedInputInt);
        PlayerPrefs.SetInt("customSeedInput", seedInputInt);
        PlayerPrefs.Save();
    }


}
