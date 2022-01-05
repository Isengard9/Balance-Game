using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickManager : MonoBehaviour
{
    public static StickManager instance;

    public List<GameObject> LeftBoxes = new List<GameObject>();
    public List<GameObject> RightBoxes = new List<GameObject>();

    [Header("Pooling")]
    [SerializeField] GameObject BoxPrefab;
    [SerializeField] int poolSize = 60;
    [SerializeField] float boxDistance = 0.1f;
    [SerializeField] Transform LeftBoxPoint;
    [SerializeField] Transform RightBoxPoint;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        CreatePool();
    }


    void Update()
    {
        
    }

    public void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject pizzaBox = Instantiate(BoxPrefab, LeftBoxPoint);
            pizzaBox.transform.localPosition = new Vector3(0, LeftBoxes.Count * boxDistance, 0);
            pizzaBox.transform.localEulerAngles = Vector3.zero;

            LeftBoxes.Add(pizzaBox);
            LeftBoxes[i].SetActive(false);
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject pizzaBox = Instantiate(BoxPrefab, RightBoxPoint);
            pizzaBox.transform.localPosition = new Vector3(0, RightBoxes.Count * boxDistance, 0);
            pizzaBox.transform.localEulerAngles = Vector3.zero;

            RightBoxes.Add(pizzaBox);
            RightBoxes[i].SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>

    public int FindLastActiveIndexLeft()
    {
        for (int i = 0; i < LeftBoxes.Count; i++)
        {
            if (!LeftBoxes[i].activeInHierarchy)
                return i;

        }

        return 0;
    }

    public int FindLastActiveIndexRight()
    {
        for (int i = 0; i < RightBoxes.Count; i++)
        {
            if (!RightBoxes[i].activeInHierarchy)
                return i;

        }

        return 0;
    }
}
