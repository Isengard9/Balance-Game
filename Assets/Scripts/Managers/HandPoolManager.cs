using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hand Pool Manager using for left or right hand box manages
/// </summary>
public class HandPoolManager : MonoBehaviour
{

    [SerializeField]
    List<GameObject> Boxes = new List<GameObject>();//Using for box pool


    [SerializeField] GameObject BoxPrefab;//using for box instantiate

    [SerializeField]
    [Range(0,100)]
    int BoxPoolCount = 60;//Pool start count

    private int lastIndex = 0;
    public int _lastIndex { get { return lastIndex; } set { lastIndex = value; } }
    private void Start()
    {
        CreatePool();
    }

    /// <summary>
    /// Creating hand boxes pool
    /// </summary>
    private void CreatePool()
    {
        for (int i = 0; i < BoxPoolCount; i++)
        {
            var o = Instantiate(BoxPrefab, this.transform);
            o.transform.localPosition = new Vector3(0, i * 0.105f, 0);

            o.gameObject.SetActive(false);
            Boxes.Add(o);
        }
    }

    /// <summary>
    /// Set Active box from using last index
    /// </summary>
    public void GetBoxFromPool()
    {
        int index = FindLastIndex();

        Boxes[index].SetActive(true);
    }

    /// <summary>
    /// Set DeActive box from using last index -1
    /// </summary>
    public void RemoveBoxFromPool()
    {
        int index = FindLastIndex();

        if (index != 0)
            Boxes[index - 1].SetActive(false);

        else
            Debug.Log("Player failed");
    }

    /// <summary>
    /// Finds the last active index in the list
    /// </summary>
    /// <returns></returns>
    public int FindLastIndex()
    {
        int index = 0;
        for (int i = 0; i < Boxes.Count; i++)
        {
            if (!Boxes[i].activeInHierarchy)
            {
                _lastIndex = i;
                return i;
            }
              
        }
        _lastIndex = index;
        return index;
    }

    /// <summary>
    /// Finds the last active index's position in the list
    /// </summary>
    /// <returns></returns>
    public Vector3 LastIndexPosition()
    {
      
        for (int i = 0; i < Boxes.Count; i++)
        {
            if (!Boxes[i].activeInHierarchy)
                return Boxes[i].transform.localPosition;
        }
        return Boxes[0].transform.localPosition;
    }
}
