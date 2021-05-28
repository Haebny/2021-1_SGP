using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCreator : MonoBehaviour
{
    public GameObject dog;
    public GameObject cat;
    private GameObject animal;

    // Start is called before the first frame update
    void Awake()
    {
        string name = PlayerPrefs.GetString("Animal");
        if (dog.name == name)
            animal = (GameObject)Instantiate(dog, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        else if (cat.name == name)
            animal = (GameObject)Instantiate(cat, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        else
            animal = (GameObject)Instantiate(dog, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

        animal.SetActive(true);
    }

    private void Start()
    {
        animal.SetActive(true);
    }
}
