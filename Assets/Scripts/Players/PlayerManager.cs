using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    #region Movement 
    [System.Serializable]
    public struct Stats
    {
        [Tooltip("Player's forward speed")]
        public float MovementSpeed;

        [Tooltip("Mouse X input clamp value")]
        public float horizontalClamp;

        [Tooltip("Stick and Player Z rotation")]
        public float rotationClamp;

        [Tooltip("Player's must be y value")]
        public float height;

        [Tooltip("Swerve touch sensitivity")]
        public float PlayerSensitivity;
    }

    public PlayerManager.Stats baseStats = new PlayerManager.Stats
    {
        MovementSpeed = 10,
        horizontalClamp = 0.5f,
        rotationClamp = 30,
        height = 1,
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

    /*public List<GameObject> leftStack = new List<GameObject>();
    public List<GameObject> rightStack = new List<GameObject>();*/
    [SerializeField] HandPoolManager leftHand;
    [SerializeField] HandPoolManager rightHand;
    [SerializeField] Transform LeftBoxPoint, RightBoxPoint;

    [Range(10, 100)]
    [SerializeField] int maxStackValue = 60;
    [SerializeField] float maxDegree = 30;

    //Rigidbody rb;
    
    public GameObject Player;
    public GameObject Stick;

    

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        //rb = GetComponent<Rigidbody>();
    }


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

    public void MoveForward()
    {
        this.transform.position += Vector3.forward * baseStats.MovementSpeed * Time.deltaTime;
        
    }
    
    public void SwipeMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FirstTouch = CalculateXPos();
        }
        if (Input.GetMouseButtonUp(0))
        {
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

    public float CalculateXPos()
    {
        Vector3 location = Input.mousePosition;
        return (location.x / (Screen.width / (baseStats.PlayerSensitivity + Mathf.Abs(-baseStats.PlayerSensitivity)))) - 5f;
    }
    
    public float CalcBalance()
    {
        float rot = 0;
        bool isRight = true;

        if(leftHand._lastIndex> rightHand._lastIndex)//will rotate left
        {
            isRight = false;
        }
        rot = ((leftHand._lastIndex- rightHand._lastIndex) * maxDegree) / maxStackValue;

        return rot;
        
    }

    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag.Equals("Obstacle"))
        {
            //ragdol olma ve fail fonk
        }
    }

}
