using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class CollectableManager : MonoBehaviour
{
    public static CollectableManager instance;

    [SerializeField] List<GameObject> CreatedCollectables = new List<GameObject>();
    [SerializeField] GameObject collectablePrefab;
    [SerializeField] Transform collectablePoint;
    [SerializeField] float boxDistance = 0.101f;

    [SerializeField] int offset = 20;
    [SerializeField] GameObject player;

    private int _collectableCount = 0;
    [SerializeField] int collectableCount {
                                            get { return _collectableCount; }
                                            set { _collectableCount = value; collectableCountText.text = value.ToString(); } }
   
    [SerializeField] TMP_Text collectableCountText;
    [SerializeField] HandPoolManager leftHand;
    [SerializeField] HandPoolManager rightHand;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        player = PlayerManager.instance.gameObject;

        CreateCollectable();

        CreateNewValue();
    }

    public void CreateCollectable()
    {
        int poolCount = 30;

        for (int i = 0; i < poolCount; i++)
        {
            GameObject collectableBox = Instantiate(collectablePrefab, collectablePoint.position, Quaternion.identity, this.transform);
            collectableBox.transform.localPosition = Vector3.zero;
            //collectableBox.transform.position = new Vector3(collectablePoint.position.x, boxDistance * i, collectablePoint.position.z);
            //collectableBox.transform.eulerAngles = Vector3.zero;
            collectableBox.SetActive(false);
            CreatedCollectables.Add(collectableBox);
            //collectableBox.transform.parent = this.transform;
        }
    }

    public void CreateNewValue()
    {
        collectableCount = Random.Range(-15, 15);

        int smallestHand = 0;

        if (leftHand._lastIndex < rightHand._lastIndex)
            smallestHand = leftHand._lastIndex;
        else
            smallestHand = rightHand._lastIndex;

        if (collectableCount < 0)
        {
            if ((collectableCount * -1) < smallestHand)
            {
                //isleme devam et
            }

            else
            {
                CreateNewValue();
            }
        }

        else if (collectableCount == 0)
            CreateNewValue();

        else
        {
            ActivateBoxes();
        }
    }

    public void ActivateBoxes()
    {
        this.transform.position = new Vector3(0, transform.position.y, SetPosition());

        for (int i = 0; i < collectableCount; i++)
        {
            CreatedCollectables[i].SetActive(true);
            CreatedCollectables[i].transform.parent = this.transform;
            CreatedCollectables[i].transform.localPosition = new Vector3(0, (boxDistance * i ), 0);

        }
    }

    public void AddToStickSide( bool isLeft)
    {
        ActivateBoxes(collectableCount);
        HandPoolManager hpm;
        if (isLeft)
            hpm = leftHand;
        else
            hpm = rightHand;

        if (collectableCount > 0)
            StartCoroutine(addSide(hpm, collectableCount));
        else
            StartCoroutine(removeSide(hpm, collectableCount));

       
    }

    IEnumerator addSide(HandPoolManager hpm,int value)
    {
        for (int i = 0; i < value; i++)
        {
            //CreatedCollectables[i].transform.localPosition = Vector3.zero;
            var o = CreatedCollectables[i];
            o.transform.parent = hpm.transform;

            o.transform.DOLocalMove(hpm.LastIndexPosition(), 0.5f).OnComplete(() => {
                hpm.GetBoxFromPool(); GetBackCollectable(o);
                
            });
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.5f);
        CreateNewValue();
    }

    IEnumerator removeSide(HandPoolManager hpm,int value)
    {
        value *= -1;
        for (int i = 0; i < value; i++)
        {
            hpm.RemoveBoxFromPool();
            yield return new WaitForEndOfFrame();
        }
        CreateNewValue();
    }

    private void GetBackCollectable(GameObject o) { o.transform.parent = this.transform;  o.transform.localPosition = Vector3.zero; o.SetActive(false); }

    private void ActivateBoxes(int value) {
        for (int i = 0; i < value; i++)
        {
            CreatedCollectables[i].SetActive(true);
            CreatedCollectables[i].transform.localPosition = Vector3.zero;
        }
    }
   
    float SetPosition()
    {
        float PositionZ = player.transform.position.z + offset;

        return PositionZ;
    }
   
}
