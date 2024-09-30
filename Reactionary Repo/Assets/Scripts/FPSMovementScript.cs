using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSMovementScript : MonoBehaviour
{
    Rigidbody g_RigidBody;
    [SerializeField] Transform groundChecker = null;
    [SerializeField] LayerMask groundLayer;

    [SerializeField] float g_fJumpForce = 12.0f;
    [SerializeField] float g_fCheckRadius = 1;
    [SerializeField] float g_fSpeed = 20.0f;
    [SerializeField] float g_fSprintMultiplier = 1.5f;

    [SerializeField] GameObject g_dataManagerObject = null;
    [SerializeField] JumpBar g_jumpBar = null;
    DataManager g_dataManager = null;

    public JumpBar jumpBar;
    public bool isCooldown = true;

    void Start()
    {
        g_RigidBody = GetComponent<Rigidbody>();
        g_dataManager = g_dataManagerObject.GetComponent<DataManager>();
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 moveBy = transform.right * x + transform.forward * z;

        float actualSpeed = g_fSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            actualSpeed *= g_fSprintMultiplier;
        }

        g_RigidBody.MovePosition(transform.position + moveBy.normalized * actualSpeed * Time.deltaTime);

        if (isCooldown)
        {
           
            //Jump functionality here. If on ground and pressing jump, then jump
            if (Input.GetKeyDown(KeyCode.Space) && IsOnGround())
            {
                SFXManager.g_cInstance.PlaySoundEffect(SFXManager.eSounds.PlayerJumps);
                g_dataManager.BroadcastMessage("ChangeJumpsUsed", 1);
                Debug.Log("Broadcast " + 1 + " Jump to DataManager");
                g_RigidBody.AddForce(Vector3.up * g_fJumpForce, ForceMode.Impulse);
                
                isCooldown = false;
                jumpBar.jumpBar.fillAmount = 0;
            }
        }
    }

    //Checks if player is on ground
    bool IsOnGround()
    {
        Collider[] colliders = Physics.OverlapSphere(groundChecker.position, g_fCheckRadius, groundLayer);

        if (colliders.Length > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
