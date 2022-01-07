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

    [SerializeField] Transform Finish;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        player = PlayerManager.instance.gameObject;
        Finish = GameObject.Find("FinishParent").transform;

        CreateCollectable();

        CreateNewValue();
    }

    /// <summary>
    /// Creates a collectable pool and adds it to the list
    /// </summary>
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

    /// <summary>
    /// Sets the collectable number randomly
    /// </summary>
    public void CreateNewValue()
    {
        if (!isFinishClose())
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
                    this.transform.position = new Vector3(0, transform.position.y, SetPosition());
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

        else
        {
            CloseEverything();
        }
        
    }

    /// <summary>
    /// Activates boxes according to collectableCount
    /// </summary>
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

    /// <summary>
    /// The process of going which side the boxes need to be positioned
    /// </summary>
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

    /// <summary>
    /// Increment movement of Boxes
    /// </summary>
    IEnumerator addSide(HandPoolManager hpm,int value)
    {
        for (int i = 0; i < value; i++)
        {
            //CreatedCollectables[i].transform.localPosition = Vector3.zero;
            var o = CreatedCollectables[i];
            o.transform.parent = hpm.transform;
            o.GetComponent<Collider>().enabled = false;

            o.transform.DOLocalMove(hpm.LastIndexPosition(), 0.5f).OnComplete(() => {
                hpm.GetBoxFromPool(); GetBackCollectable(o);
                
            });
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.5f);
        CreateNewValue();
    }

    /// <summary>
    /// Decrement movement of boxes
    /// </summary>
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

    /// <summary>
    /// Activates the boxes
    /// </summary>
    private void ActivateBoxes(int value) {
        for (int i = 0; i < value; i++)
        {
            CreatedCollectables[i].SetActive(true);
            CreatedCollectables[i].transform.localPosition = Vector3.zero;
        }
    }

    /// <summary>
    /// Sets the position of the boxes
    /// </summary>
    float SetPosition()
    {
        float PositionZ = player.transform.position.z + offset;

        return PositionZ;
    }

    /// <summary>
    /// Checks if the finish is close
    /// </summary>
    private bool isFinishClose()
    {
        if (Finish.position.z - transform.position.z <= 15)
            return true;

        else
            return false;
    }

    private void CloseEverything()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
   
}
