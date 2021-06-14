using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCreator : MonoBehaviour
{
    public GameObject Dog;
    public GameObject Cat;
    public GameObject Chicken;

    public GameObject Bed;
    public GameObject Table;
    public GameObject Car;

    public GameObject Dash;
    private GameObject animal;
    private GameObject mount;

    void Awake()
    {
        // 탑승물을 선택했는지 확인하기
        int isMounting = MountChoose.index;

        // 탑승 안함
        if (isMounting < 1)
        {
            CreateAnimal(null);
        }

        // 탑승물 생성
        else
        {
            string name = PlayerPrefs.GetString("Mount");
            if (Bed.name == name)
                mount = (GameObject)Instantiate(Bed, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            else if (Table.name == name)
                mount = (GameObject)Instantiate(Table, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            else if (Car.name == name)
                mount = (GameObject)Instantiate(Car, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            else // Error
                mount = null;

            CreateAnimal(mount);
        }
    }

    private void CreateAnimal(GameObject parent)
    {
        // 동물 생성
        string name = PlayerPrefs.GetString("Animal");

        if (Dog.name == name)
            animal = (GameObject)Instantiate(Dog, new Vector3(0, 1.5f, 0), Quaternion.identity) as GameObject;
        else if (Cat.name == name)
            animal = (GameObject)Instantiate(Cat, new Vector3(0, 1.5f, 0), Quaternion.identity) as GameObject;
        else if (Chicken.name == name)
            animal = (GameObject)Instantiate(Chicken, new Vector3(0, -1f, 0), Quaternion.Euler(new Vector3(0, 90, 0))) as GameObject;
        else // Error
            animal = (GameObject)Instantiate(Dog, new Vector3(0, 1.5f, 0), Quaternion.identity) as GameObject;

        // 탑승물이 없는 경우
        if (parent == null)
            return;

        // 탑승물이 있는 경우
        animal.transform.parent = mount.transform;
    }

    private void Start()
    {
        animal.SetActive(true);
        GameObject dash = (GameObject)Instantiate(Dash, new Vector3(0, animal.transform.GetChild(0).transform.position.y, 0), Quaternion.Euler(new Vector3(0, -90, 0))) as GameObject;
        dash.transform.parent = animal.transform;
        dash.gameObject.SetActive(true);
        dash.GetComponent<ParticleSystem>().Stop();
    }
}
