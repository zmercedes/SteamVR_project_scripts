/* Controller Input Manager
 0. set up observer pattern on script
 1. add teleportation
 2. add object grabbing/throwing
 	- 
 3. create rube goldberg objects
 	- try different geometries, colors, physics functions
 4. create object menu
 	- attaches to right hand, appears on touchpad press
 5. set special grab rules for rube goldberg objects
 	- can grab but cannot throw. on release, must stay in place
 6. gameplay!
 	- create collectible that ball must touch in order to win
 	- reenable collectible on ball touching floor
 	- create goal that loads next level on ball hitting it after having
 		collected all collectibles
 	- create anti cheating mechanics
 	- 4 different levels
 	-- can limit the number of objects that can be placed in a level,
 		can vary per level
 7. final polish!
 	- create instruction UI
 	- make environment nice
 	- runs at 90fps


 Both Hand Actions:
     Grab objects

 Right Hand Actions:
     rube goldberg item picker/generator (touchpad)
 
 Left Hand Actions:
     touchpad activates teleportation
     
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ControllerInputManager : MonoBehaviour {

	// controller references
	SteamVR_Controller.Device rightController;
	SteamVR_Controller.Device leftController;

	// controller object references, to check for active controllers
	GameObject[] controllerObjs; // 0 is left, 1 is right

	// tracked object references
	SteamVR_TrackedObject[] trackedObjs; // 0 is left, 1 is right

	// teleporting
	GameObject arc;
	ArcRenderer arcRenderer;
	public GameObject teleportAimObject;
	private LayerMask teleMask;
	private Vector3 teleportLocation;
	private GameObject player;

	// testing pane with touchpad
	GameObject canvas;
	private float touchLast;
	private float touchCurrent;
	private float distance;
	public float swipeSpeed = 2000;
	// private bool hasSwipedLeft;
	// private bool hasSwipedRight;

	void Start () {
		canvas = transform.GetChild(1).GetChild(1).gameObject;
		arc = transform.GetChild(0).GetChild(1).gameObject;
		arcRenderer = arc.GetComponent<ArcRenderer>();
		controllerObjs = new GameObject[2];
		trackedObjs = new SteamVR_TrackedObject[2];

		for(int i = 0; i < 2; i++){
			controllerObjs[i] = transform.GetChild(i).gameObject;
			trackedObjs[i] = controllerObjs[i].GetComponent<SteamVR_TrackedObject>();
		}	
	}
	
	void Update () {
		ActivateControllers();

		// right hand functions
		if(controllerObjs[1].activeSelf){
			if(rightController.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
				Debug.Log("right touched!");

			// setting initial touch position for swiping
			if(rightController.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
				touchLast = rightController.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
			
			// activate/deactivate canvas on touchpad press
			if(rightController.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)){
				Debug.Log("right tp pressed down!");
				canvas.SetActive(!canvas.activeSelf);
			}

			// if canvas is active and player is touching touchpad,
			// rotate the canvas based on swiping left/right on touchpad.
			if(canvas.activeSelf && rightController.GetTouch(SteamVR_Controller.ButtonMask.Touchpad)){

				touchCurrent = rightController.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
				distance = touchCurrent - touchLast;
				touchLast = touchCurrent;

				canvas.transform.Rotate(Vector3.forward* distance * swipeSpeed * Time.deltaTime);
			}
		}

		// left hand functions
		if(controllerObjs[0].activeSelf){
			// teleport code

			if(leftController.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
				arc.gameObject.SetActive(!arc.activeSelf);
			
			if(leftController.GetTouch(SteamVR_Controller.ButtonMask.Touchpad)){
				Debug.Log("left touched!");

				// teleportAimObject.SetActive(true);
				// arc.SetPosition(0, controllerObjs[0].transform.position);

				// RaycastHit hit;
				// if(Physics.Raycast(transform.position, transform.forward, out hit, 15, teleMask)){
					// 	teleportLocation = hit.point;
					// 	teleportAimObject.transform.position = teleportLocation;
				// }
			}

			if(arc.gameObject.activeSelf)
				arcRenderer.SetValues(controllerObjs[0].transform);

			// if(leftController.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad) && arc.gameObject.activeSelf)
				// arc.gameObject.SetActive(false);

		}
	}

	// sets appropriate controller references
	void ActivateControllers(){
		if(controllerObjs[0].activeSelf)
			leftController = SteamVR_Controller.Input((int)trackedObjs[0].index);

		if(controllerObjs[1].activeSelf)
			rightController = SteamVR_Controller.Input((int)trackedObjs[1].index);
	}
}
