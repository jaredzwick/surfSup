using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float walkSpeed = 3.5f;
	public float runSpeed = 8;
	public float gravity = -9.8f;
	public float jumpHeight = 1.5f;
	[Range(0,1)]
	public float airControlPercent;
	public float turnSmoothTime = 0.2f;
	float turnSmoothVelocity;
	float velocityY;
	public float speedSmoothTime = 0.1f;
	float speedSmoothVelocity;
	float currentSpeed;
	float currentSpeed2;
	Animator animator;
	public CharacterController controller;
	public Transform cameraT;
	void Start () {
		animator = GetComponent<Animator> ();
		controller = GetComponent<CharacterController>();
	}

	void Update () {
		//input section
		Vector2 input = new Vector2 (Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		Vector2 inputDir = input.normalized;

		// Jump
		if (Input.GetKeyDown(KeyCode.Space)){
			Jump();
		}


		// Move
		if (inputDir != Vector2.zero) {
			float targetRotation = Mathf.Atan2 (inputDir.x, inputDir.y) * Mathf.Rad2Deg +cameraT.eulerAngles.y;
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, getModifiedSmoothTime(turnSmoothTime));
		}

		bool running = Input.GetKey (KeyCode.LeftShift);
		float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
		currentSpeed = Mathf.SmoothDamp (currentSpeed, targetSpeed, ref speedSmoothVelocity, getModifiedSmoothTime(speedSmoothTime));
		velocityY+= Time.deltaTime * gravity;
		Vector3 velocity = (transform.forward * currentSpeed) + (Vector3.up*velocityY);
		controller.Move(velocity*Time.deltaTime);
		currentSpeed2 = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;
		if(controller.isGrounded){
			velocityY=0;
		}

		// animator
		float animationSpeedPercent = ((running) ? currentSpeed2/runSpeed : currentSpeed2 /walkSpeed * 0.5f);
		animator.SetFloat ("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);

	}


	void Jump(){
		if(controller.isGrounded) {
			// kinematic equations
			float jumpVelocity = Mathf.Sqrt(-2*gravity *jumpHeight);
			velocityY = jumpVelocity;
		}
	}

	float getModifiedSmoothTime(float smoothTime){
		if(controller.isGrounded){
			return smoothTime;
		}
		if(airControlPercent==0){
			return float.MaxValue;
		}
		return smoothTime / airControlPercent;
	}
}