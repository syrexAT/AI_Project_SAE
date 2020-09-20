using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowSeedText : MonoBehaviour
{
    public MapGenerator mapGen;
    public TextMeshProUGUI seedText;

    // Start is called before the first frame update
    void Start()
    {
        seedText.text = "Seed: " + " " + mapGen.seed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
