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
    [SerializeField] int decreaseCount = 0;

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
        collectableCount = Random.Range(1, 30);

        for (int i = 0; i < collectableCount; i++)
        {
            GameObject collectableBox = Instantiate(collectablePrefab, collectablePoint.position, Quaternion.identity, this.transform);

            collectableBox.transform.position = new Vector3(collectablePoint.position.x, boxDistance * i, collectablePoint.position.z);
            //collectableBox.transform.eulerAngles = Vector3.zero;

            CreatedCollectables.Add(collectableBox);
            //collectableBox.transform.parent = this.transform;
        }
    }

    
    public void GetCollectableToLeft(Transform LeftSide)
    {
        int leftStackCount = PlayerManager.instance.leftStack.Count;

        for (int i = 0; i < collectableCount; i++)
        {
            CreatedCollectables[i].transform.parent = LeftSide;
            CreatedCollectables[i].transform.DOLocalMove(new Vector3(0, ((boxDistance * i) + (leftStackCount * boxDistance)), 0), 0.5f);

            PlayerManager.instance.leftStack.Add(CreatedCollectables[i]);
            //CreatedCollectables.RemoveAt(i);

            StartCoroutine(DelayActivated(0.1f));
        }

        CreatedCollectables.Clear();
    }

    public void GetCollectableToRight(Transform RightSide)
    {
        int rightStackCount = PlayerManager.instance.rightStack.Count;

        for (int i = 0; i < collectableCount; i++)
        {
            CreatedCollectables[i].transform.parent = RightSide;
            CreatedCollectables[i].transform.DOLocalMove(new Vector3(0, ((boxDistance * i) + (rightStackCount * boxDistance)), 0), 0.5f);

            PlayerManager.instance.rightStack.Add(CreatedCollectables[i]);
            //CreatedCollectables.RemoveAt(i);

            StartCoroutine(DelayActivated(0.1f));
        }

        CreatedCollectables.Clear();
    }

    public void CreateMinusText()
    {
        decreaseCount = Random.Range(1, 30);
        if(decreaseCount > collectableCount)
        {
            CreateMinusText();
        }
        Debug.Log(decreaseCount + "eksi cikti");
        //eksi yazısı üretilecek
    }

    public void DecreaseBoxesLeft(List<GameObject> leftSide)
    {
        int lastIndex = leftSide.Count - 1;
        Debug.Log(lastIndex);
        for (int i = 0; i < decreaseCount; i++)
        {
            leftSide.RemoveAt(lastIndex - i);
            Destroy(leftSide[lastIndex - i]);
        }
    }

    public void DecreaseBoxesRight(List<GameObject> rightSide)
    {
        int lastIndex = rightSide.Count - 1;
        Debug.Log(lastIndex);
        for (int i = 0; i < decreaseCount; i++)
        {
            rightSide.RemoveAt(lastIndex - i);
            Destroy(rightSide[lastIndex - i]);
        }
    }


    public void TakeCollectableToLeft(List<GameObject> LeftSide, float duration)
    {
        int lastIndex = StickManager.instance.FindLastActiveIndexLeft();

        for (int i = 0; i < collectableCount; i++)
        {
            CreatedCollectables[i].transform.DOMove(LeftSide[lastIndex].transform.position, duration)
                .OnComplete(() => LeftSide[lastIndex].SetActive(true));

            StartCoroutine(DelayActivated(duration));
            lastIndex += 1;
            PlayerManager.instance.leftStack.Add(LeftSide[lastIndex]);
            Destroy(CreatedCollectables[i].gameObject);

               // .OnComplete(() => lastIndex += 1)
               // .OnComplete(() => PlayerManager.instance.leftStack.Add(LeftSide[lastIndex]))
               // .OnComplete(() => Destroy(CreatedCollectables[i].gameObject));
        }

        CreatedCollectables.Clear();
    }

    public void TakeCollectableToRight(List<GameObject> RightSide, float duration)
    {
        int lastIndex = StickManager.instance.FindLastActiveIndexRight();

        for (int i = 0; i < collectableCount; i++)
        {
            CreatedCollectables[i].transform.DOMove(RightSide[lastIndex].transform.position, duration)
                .OnComplete(() => RightSide[lastIndex].SetActive(true));

            StartCoroutine(DelayActivated(duration));
            lastIndex += 1;
            PlayerManager.instance.rightStack.Add(RightSide[lastIndex]);
            Destroy(CreatedCollectables[i].gameObject);

            // .OnComplete(() => lastIndex += 1)
            // .OnComplete(() => PlayerManager.instance.rightStack.Add(RightSide[lastIndex]))
            // .OnComplete(() => Destroy(CreatedCollectables[i].gameObject));
        }

        CreatedCollectables.Clear();
    }

    IEnumerator DelayActivated(float delay)
    {
        yield return new WaitForSeconds(delay);
    }
}
