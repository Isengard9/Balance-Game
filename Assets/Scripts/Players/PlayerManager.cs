using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using RootMotion.FinalIK;

/// <summary>
/// Player Manager controls the Player's behaviours (movement, rotation, balance, trigger) and mouse touches
/// </summary>
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    #region Movement 
    [System.Serializable]
    public struct Stats
    {
        [Tooltip("Player's forward speed")]
        public float MovementSpeed;

        [Tooltip("Swerve touch sensitivity")]
        public float PlayerSensitivity;
    }

    public PlayerManager.Stats baseStats = new PlayerManager.Stats
    {
        MovementSpeed = 10,
        PlayerSensitivity = 10,
    };

    #endregion

    #region Touch State
    public enum TouchState
    {
        none,
        left,
        right
    }
    public TouchState touchState = TouchState.none;

    float FirstTouch, lastTouch = 0;

    #endregion

    [SerializeField] HandPoolManager leftHand;
    [SerializeField] HandPoolManager rightHand;
    [SerializeField] Transform LeftBoxPoint, RightBoxPoint;

    [Range(10, 100)]
    [SerializeField] int maxStackValue = 60;
    [SerializeField] float maxDegree = 30;

    Rigidbody rb;

    [SerializeField] GameObject LookAtObj;
    [SerializeField] Animator playerAnim;

    FullBodyBipedIK bipedIK;
    [SerializeField] float leftFootWeight, rightFootWeight = 0;

    [SerializeField] List<Rigidbody> Rigidbodies = new List<Rigidbody>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        FindRb(playerAnim.gameObject);

        bipedIK = playerAnim.gameObject.GetComponent<FullBodyBipedIK>();
        leftFootWeight = bipedIK.solver.leftFootEffector.positionWeight;
        rightFootWeight = bipedIK.solver.rightFootEffector.positionWeight;
    }

    bool isGameStarted = false;
    void Update()
    {
        if (GameManager.isGameEnded || !GameManager.isGameStarted)
            return;

        MoveForward();
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, CalcBalance()));
        SwipeMovement();

        //BoxesRotControl();
    }

    #region Player Movement

    /// <summary>
    /// Player's forward movement
    /// </summary>
    public void MoveForward()
    {
        this.transform.position += Vector3.forward * baseStats.MovementSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Controls what happens when swiped right or left
    /// </summary>
    public void SwipeMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FirstTouch = CalculateXPos();
            if (!isGameStarted)
                isGameStarted = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (!isGameStarted)
                return;
            lastTouch = CalculateXPos();

            if (FirstTouch > lastTouch)
            {
                touchState = TouchState.left;
                CollectableManager.instance.AddToStickSide(true);
            }

            else
            {
                touchState = TouchState.right;
                CollectableManager.instance.AddToStickSide(false);
            }

            touchState = TouchState.none;
        }
    }

    /// <summary>
    /// calculates x position based on screen width
    /// </summary>
    public float CalculateXPos()
    {
        Vector3 location = Input.mousePosition;
        return (location.x / (Screen.width / (baseStats.PlayerSensitivity + Mathf.Abs(-baseStats.PlayerSensitivity)))) - 5f;
    }

    /// <summary>
    /// Calculates rotation based on incoming weights
    /// </summary>
    public float CalcBalance()
    {
        float rot = 0;
        bool isRight = true;

        if(leftHand._lastIndex> rightHand._lastIndex)//will rotate left
        {
            isRight = false;
        }
        rot = ((leftHand._lastIndex- rightHand._lastIndex) * maxDegree) / maxStackValue;

        GameManager.instance.BalanceBarControl(rot);
        FootUp(rot);
        return rot;
        
    }

    #endregion

    /// <summary>
    /// Finds rigidbodies in children under player and adds them to the list
    /// </summary>
    /// <param name="researchedObject"></param>
    void FindRb(GameObject researchedObject)
    {
        if (researchedObject.GetComponent<Rigidbody>() != null)
            Rigidbodies.Add(researchedObject.GetComponent<Rigidbody>());

        for (int i = 0; i < researchedObject.transform.childCount; i++)
        {
            FindRb(researchedObject.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Ragdoll activation process
    /// </summary>
    void RagdollActivated()
    {
        foreach (var r in Rigidbodies)
        {
            r.isKinematic = false;
        }
    }

    /// <summary>
    /// Controls the processes that should happen when it fails
    /// </summary>
    void FailTime()
    {
        LookAtObj.transform.parent = null;
        playerAnim.enabled = false;
        playerAnim.gameObject.GetComponent<FullBodyBipedIK>().enabled = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        this.GetComponent<Collider>().isTrigger = true;
    }

    /// <summary>
    /// FinalIK process
    /// </summary>
    void FootUp(float rotValue)
    {
        float weight = 0;
        weight = rotValue * 0.02f;
        weight = Mathf.Abs(weight);
        if(rotValue > 0)
        {
            //sag ayak
            bipedIK.solver.rightFootEffector.positionWeight = weight;
            bipedIK.solver.leftFootEffector.positionWeight = 0;

            bipedIK.solver.rightFootEffector.rotationWeight = weight;
            bipedIK.solver.leftFootEffector.rotationWeight = 0;
        }

        else if(rotValue < 0)
        {
            //sol ayak
            bipedIK.solver.leftFootEffector.positionWeight = weight;
            bipedIK.solver.rightFootEffector.positionWeight = 0;

            bipedIK.solver.leftFootEffector.rotationWeight = weight;
            bipedIK.solver.rightFootEffector.rotationWeight = 0;
        }

        if(weight < 0.4f)
        {
            playerAnim.enabled = true;
        }

        else
        {
            playerAnim.enabled = false;
        }
        
    }

    public void FailProcess()
    {
        FailTime();
        RagdollActivated();
    }

    public void FinishProcess()
    {
        transform.eulerAngles = Vector3.zero;
        playerAnim.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag.Equals("Obstacle"))
        {
            GameManager.instance.OnLevelFailed();
        }

        if (other.transform.tag.Equals("Finish"))
        {
            GameManager.instance.OnLevelSuccessed();
        }
    }

   
}
