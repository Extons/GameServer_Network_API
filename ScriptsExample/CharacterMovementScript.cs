using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BambooNetCode;

[RequireComponent(typeof(PlayerCharacter))]
public class CharacterMovementScript : MonoBehaviour
{
    [Header("Movements Settings")]
    [SerializeField] private Animator animator_;
    [SerializeField] private ConfigurableJoint pivot_;
    [SerializeField] private float rotationSpeed_;
    [SerializeField] private float jumpForce_;
    [SerializeField] private FeetTouchDetectionScript rightFeetDetect_;
    [SerializeField] private FeetTouchDetectionScript leftFeetDetect_;
    [SerializeField] private HipsScript hipsScript_;
    [Header("Punsh Setting")]
    [SerializeField] private float punshForce_;
    [SerializeField] private ConfigurableJoint rightFist_;
    [SerializeField] private ConfigurableJoint leftFist_;
    [Header("Grab Settings")]
    [SerializeField] private LayerMask grabLayer_;
    [SerializeField] private float grabRadius_ = 1f;
    [SerializeField] private HandGrabControllerScript rightGrabScript_;
    [SerializeField] private HandGrabControllerScript leftGrabScript_;
    [SerializeField] private HeadScript headScript_;
    [Header("Joints")]
    [SerializeField] private List<JointFollowScript> followJointsList_ = new List<JointFollowScript>();

    private PlayerCharacter playerCharacter_;
    private Transform initParent_;
    private Action onDeadCallback_;
    private Quaternion quaternionZero_ = new Quaternion(0, 0, 0, 0);
    private Quaternion initRot_;
    private Vector3 spawnPoint_;
    private Vector3 lastMoveVec_;
    private float vertical_;
    private float horizontal_;
    private bool isKo_;
    private bool onAir_ = false;
    private bool onLittleJump_ = false;
    private bool isControllable_;
    private bool isJumped_;
    private float tOut_;
    private float jumpTimeOut_;
    private float punchCooldown_;

    public PlayerCharacter playerCharacter { get { return playerCharacter_; } }
    public HipsScript hipsScript { get { return hipsScript_; } }
    public Action onDeadCallback { get { return onDeadCallback_; } set { onDeadCallback_ = value; } }

    private void Awake()
    {
        playerCharacter_ = GetComponent<PlayerCharacter>();
    }
    private void Start()
    {
        initParent_ = transform.parent;
        initRot_ = transform.rotation;
        spawnPoint_ = transform.position;

        if (headScript_ != null)
            headScript_.onKnockOutMethod += () => { OnKnockOut(); };
    }
    private void Update()
    {
        if (!isControllable_)
            return;

        InputsUpdate();
        jumpTimeOut_ = (jumpTimeOut_ > 0f) ? jumpTimeOut_ - Time.deltaTime : 0f;
        punchCooldown_ = (punchCooldown_ > 0f) ? punchCooldown_ - Time.deltaTime : 0f;
    }
    private void FixedUpdate()
    {
        if (!isKo_)
        {
            FloorDetection();
            Rotate();
        }
    }
    public void SetControllable(bool _value)
    {
        isControllable_ = _value;
    }
    public void TeleportTo(Vector3 _position , Quaternion _rotation = default(Quaternion))
    {
        hipsScript_?.TeleportTo(_position , _rotation);
    }
    public void OnKnockOut(bool _permanetly = false)
    {
        DropObject();

        JointFollowScript pJfs = pivot_.GetComponent<JointFollowScript>();
        if (pJfs != null)
        {
            pJfs.applyConstantsOnSetOff = true;
        }

        foreach (JointFollowScript j in followJointsList_)
            j.SetOff();
        isKo_ = true;

        if (!_permanetly)
        {
            StopAllCoroutines();
            StartCoroutine(IEGetUp(5f));
        }
    }
    public void Dead()
    {
        OnKnockOut(true);
        onDeadCallback?.Invoke();
    }
    public void LootAt(Vector3 _direction)
    {
        if(pivot_ != null)
            pivot_.targetRotation = Quaternion.LookRotation(_direction);
    }
    private void Rotate()
    {
        if (pivot_ != null && !onAir_ && playerCharacter_ != null)
        {
            float cameraYAngle = playerCharacter_.networkInputs.cameraYAngle;
            Vector3 input = new Vector3(vertical_, 0, -horizontal_);
            Vector3 direction = Quaternion.Euler(0, cameraYAngle, 0) * input;

            if (horizontal_ + vertical_ != 0)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                pivot_.targetRotation = Quaternion.RotateTowards(pivot_.targetRotation, rotation,90f);
            }
        }
    }
    private void InputsUpdate()
    {
        if (playerCharacter_ == null)
            return;

        vertical_ = playerCharacter_.networkInputs.vertical;
        horizontal_ = playerCharacter_.networkInputs.horizontal;

        float movement = Mathf.Clamp01( Mathf.Abs(vertical_) + Mathf.Abs(horizontal_));

        if (animator_ != null)
            animator_.SetFloat("movement", movement);

        // NEEW REWORK
        if (playerCharacter_.networkInputs.rightGrab.down)
            GrabObjectWithRightHand();
        if (playerCharacter_.networkInputs.leftGrab.down)
            GrabObjectWithLeftHand();

        if (playerCharacter_.networkInputs.drop.down)
            DropObject();
        if (playerCharacter_.networkInputs.punsh.down)
            Punch();
        if (playerCharacter_.networkInputs.grabUp.down)
            GrabUp();

        if (playerCharacter_.networkInputs.jump.down)
        {
            Jump();
        }
    }
    private void FloorDetection()
    {
        if (rightFeetDetect_ == null || leftFeetDetect_ == null)
            return;

        float time1 = rightFeetDetect_.timeOut;
        float time2 = leftFeetDetect_.timeOut;

        float rsltTime = Mathf.Sqrt(time1 * time2);

        if (rsltTime > 0.25f && !onLittleJump_)
        {
            foreach (JointFollowScript j in followJointsList_)
                j.SetOff();
            onLittleJump_ = true;
            UnparentPlatform();
        }else if (rsltTime > 1.0f && !onAir_) //2f
        {
            foreach (JointFollowScript j in followJointsList_)
                j.SetOff();
            onAir_ = true;
        }

        if(rsltTime <= 0f && onAir_)
        {
            StartCoroutine(IEGetUp(2)); 
        }
        else if(rsltTime <= 0f && !onAir_ && onLittleJump_)
        {
            foreach (JointFollowScript j in followJointsList_)
                j.Initializing();
            onLittleJump_ = false;
        }

        if(rsltTime <= 0 && isJumped_)
            isJumped_ = false;

        ParentPlatform(rightFeetDetect_, leftFeetDetect_);

        tOut_ = rsltTime;
    }
    private void ParentPlatform(FeetTouchDetectionScript _rFeet , FeetTouchDetectionScript _lFeet)
    {

        Transform parent = _rFeet.floorParent;
        if (parent == null)
            parent = _lFeet.floorParent;

        if (parent != null && transform.parent != parent)
        {
            transform.SetParent(_rFeet.floorParent);
        }
        else if (parent == null && transform.parent != initParent_)
            UnparentPlatform();
    }
    private void UnparentPlatform()
    {
        transform.parent = (initParent_);
    }

    private void Jump()
    {
        if (isKo_ || isJumped_ || pivot_ == null || jumpTimeOut_ > 0f)
            return;

        isJumped_ = true;
        jumpTimeOut_ = 0.5f;

        float jumpRatio = 1;
        if (rightGrabScript_ != null && leftGrabScript_ != null && (rightGrabScript_.isGrab || leftGrabScript_.isGrab))
            jumpRatio = 1.35f;

        pivot_.GetComponent<Rigidbody>().AddForce((Vector3.up + (pivot_.transform.right * 0.25f)) * jumpForce_ * jumpRatio, ForceMode.Impulse);
        GrabDown();
    }
    
    private void Punch()
    {
        if (rightGrabScript_ == null || leftGrabScript_ == null || punchCooldown_ > 0)
            return;

        float force = 30f;
        ConfigurableJoint selectObj = rightGrabScript_?.grabCJoint;
        if (selectObj != null && !leftGrabScript_.isGrab)
        {
            leftGrabScript_.PunchGrab(force, selectObj.gameObject);
        }
        else
        {
            selectObj = leftGrabScript_?.grabCJoint;
            if (selectObj != null && !rightGrabScript_.isGrab)
            {
                rightGrabScript_.PunchGrab(force, selectObj.gameObject);
                return;
            }
        }

        if (selectObj != null && leftGrabScript_.isGrab && rightGrabScript_.isGrab && headScript_ != null)
        {
            headScript_.HeadPunch(force, selectObj.gameObject);
            return;
        }

        if(UnityEngine.Random.value > 0.5f)
            rightGrabScript_?.PunchGrab(force, null);
        else
            leftGrabScript_?.PunchGrab(force, null);

        punchCooldown_ = 0.35f;
    }
    private void GrabObjectWithRightHand()
    {
        rightGrabScript_?.Grab();
    }
    private void GrabObjectWithLeftHand()
    {
        leftGrabScript_?.Grab();
    }
    private void GrabUp()
    {
        if (rightGrabScript_ != null && rightGrabScript_.isGrab)
            rightGrabScript_.GrabUp();
        if (leftGrabScript_ != null && leftGrabScript_.isGrab)
            leftGrabScript_.GrabUp();
    }
    private void GrabDown()
    {
        if (rightGrabScript_ != null)
            rightGrabScript_.GrabDown();
        if (leftGrabScript_ != null)
            leftGrabScript_.GrabDown();
    }
    private void DropObject()
    {
        rightGrabScript_?.Drop();
        leftGrabScript_?.Drop();
    }

    private IEnumerator IEGetUp(float _timeOut)
    {
        onAir_ = false;
        onLittleJump_ = false;
        yield return new WaitForSeconds(_timeOut);
        foreach (JointFollowScript j in followJointsList_)
            j.Initializing();
        isKo_ = false;

        JointFollowScript pJfs = pivot_.GetComponent<JointFollowScript>();
        if (pJfs != null)
        {
            pJfs.applyConstantsOnSetOff = false;
        }
    }
}
