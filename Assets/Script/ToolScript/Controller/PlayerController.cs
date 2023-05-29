using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {
    private CharacterController cc;
    public float moveSpeed;
    public float jumpSpeed;
    private float horizontalMove, verticalMove;
    private Vector3 dir;
    public float gravity;
    private Vector3 velocity; //用来控制Y轴加速度

    private void Start() {
        cc = GetComponent<CharacterController>();
    }

    private void Update() {
        horizontalMove = Input.GetAxis("Horizontal") * moveSpeed;
        verticalMove = Input.GetAxis("Vertical") * moveSpeed;

        dir = transform.forward * verticalMove + transform.right * horizontalMove;
        cc.Move(dir * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space)) {
            velocity.y = jumpSpeed;
        }

        velocity.y -= gravity * Time.deltaTime; //每秒它就会减去重力的值不断下降
        cc.Move(velocity * Time.deltaTime);
    }
}