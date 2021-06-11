using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCreator : MonoBehaviour
{
    public GameObject Dog;
    public GameObject Cat;
    public GameObject Dash;
    private GameObject animal;

    // Start is called before the first frame update
    void Awake()
    {
        string name = PlayerPrefs.GetString("Animal");
        if (Dog.name == name)
            animal = (GameObject)Instantiate(Dog, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        else if (Cat.name == name)
            animal = (GameObject)Instantiate(Cat, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        else // Error
            animal = (GameObject)Instantiate(Dog, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
    }

    private void Start()
    {
        animal.SetActive(true);
        GameObject dash = (GameObject)Instantiate(Dash, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, -90, 0))) as GameObject;
        dash.transform.parent = animal.transform;
        dash.gameObject.SetActive(true);
        dash.GetComponent<ParticleSystem>().Stop();
    }
}
