using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Move : MonoBehaviour
{
    private Rigidbody2D playerRb;
    public Animator playerAnim;
    private MapTransition boundaryScript;

    public float playerSpeed;

    public Vector2 saveMaxPos;
    public Vector2 saveMinPos;

    public UIBtn uIBtn;
    public GameObject mapItem;
    public GameObject effect;

    private Transform PinLocker;

    void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        //플레이어 움직임
        playerRb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * playerSpeed * Time.deltaTime;

        //플레이어 이동 애니메이션
        playerAnim.SetFloat("MoveX", playerRb.velocity.x);
        playerAnim.SetFloat("MoveY", playerRb.velocity.y);

        //방향에 일치하는 Idle 애니메이션 적용을 위해 lastMoveX/Y 변수에 저장
        if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1 || Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1)
        {
            playerAnim.SetFloat("lastMoveX", Input.GetAxisRaw("Horizontal"));
            playerAnim.SetFloat("lastMoveY", Input.GetAxisRaw("Vertical"));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Boundary")
        {
            boundaryScript = collision.GetComponent<MapTransition>();
            saveMaxPos = boundaryScript.newMaxCameraBoundary;
            saveMinPos = boundaryScript.newMinCameraBoundary;
        }

        if(collision.tag == "Map")
        {
            uIBtn.CheckMap = true;
            Destroy(mapItem);
            effect.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PinLocker")
        {
            PinLocker = GameObject.Find("PinLocker").transform.Find("Pin");
            PinLocker.gameObject.SetActive(true);
        }
    }
}