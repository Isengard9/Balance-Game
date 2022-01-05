using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CollectableManager : MonoBehaviour
{
    public static CollectableManager instance;

    [SerializeField] List<GameObject> CreatedCollectables = new List<GameObject>();
    [SerializeField] GameObject collectablePrefab;
    [SerializeField] Transform collectablePoint;
    [SerializeField] float boxDistance = 0.101f;

    [SerializeField] int collectableCount = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        CreateCollectable();
    }


    void Update()
    {
        
    }

    public void CreateCollectable()
    {
        collectableCount = Random.Range(1, 50);

        for (int i = 0; i < collectableCount; i++)
        {
            GameObject collectableBox = Instantiate(collectablePrefab, collectablePoint);

            collectableBox.transform.localPosition = new Vector3(collectablePoint.position.x, boxDistance * i, collectablePoint.position.z);
            collectableBox.transform.localEulerAngles = Vector3.zero;

            CreatedCollectables.Add(collectableBox);
            collectableBox.transform.parent = null;
        }
    }

    public void TakeCollectableToLeft(List<GameObject> LeftSide, float duration)
    {
        int lastIndex = StickManager.instance.FindLastActiveIndexLeft();

        for (int i = 0; i < collectableCount; i++)
        {
            CreatedCollectables[i].transform.DOMove(LeftSide[lastIndex].transform.position, duration)
                .OnComplete(() => LeftSide[lastIndex].SetActive(true))
                .OnComplete(() => lastIndex += 1)
                .OnComplete(() => PlayerManager.instance.leftStack.Add(LeftSide[lastIndex]))
                .OnComplete(() => Destroy(CreatedCollectables[i].gameObject));
        }

        CreatedCollectables.Clear();
    }

    public void TakeCollectableToRight(List<GameObject> RightSide, float duration)
    {
        int lastIndex = StickManager.instance.FindLastActiveIndexRight();

        for (int i = 0; i < collectableCount; i++)
        {
            CreatedCollectables[i].transform.DOMove(RightSide[lastIndex].transform.position, duration)
                .OnComplete(() => RightSide[lastIndex].SetActive(true))
                .OnComplete(() => lastIndex += 1)
                .OnComplete(() => PlayerManager.instance.rightStack.Add(RightSide[lastIndex]))
                .OnComplete(() => Destroy(CreatedCollectables[i].gameObject));
        }

        CreatedCollectables.Clear();
    }
}
