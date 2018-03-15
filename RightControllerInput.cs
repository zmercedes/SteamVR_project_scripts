/* Controller Input Manager
 0. set up observer pattern on script -> scrapped
 1. add teleportation 				  -> done
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

public class RightControllerInput : MonoBehaviour {

	// controller references
	SteamVR_Controller.Device controller;
	SteamVR_TrackedObject trackedObj;

	// reference to left controller
	GameObject leftController;
	bool isGrabbing = false;

	// testing pane with touchpad
	GameObject canvas;
	private float touchLast;
	private float touchCurrent;
	private float distance;
	public float swipeSpeed = 50;
	// private bool hasSwipedLeft;
	// private bool hasSwipedRight;

	void Awake () {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
		leftController = transform.parent.GetChild(0).gameObject;
		canvas = transform.GetChild(1).gameObject;
	}

	void Update () {
		controller = SteamVR_Controller.Input((int)trackedObj.index);

		// right hand functions
			if(controller.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
				Debug.Log("right touched!");

			// setting initial touch position for swiping
			if(controller.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
				touchLast = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
			
			// activate/deactivate canvas on touchpad press
			if(controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)){
				Debug.Log("right tp pressed down!");
				canvas.SetActive(!canvas.activeSelf);
			}

			// if canvas is active and player is touching touchpad,
			// rotate the canvas based on swiping left/right on touchpad.
			if(canvas.activeSelf && controller.GetTouch(SteamVR_Controller.ButtonMask.Touchpad)){

				touchCurrent = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
				distance = touchCurrent - touchLast;
				touchLast = touchCurrent;

				canvas.transform.Rotate(Vector3.forward * distance * swipeSpeed);
			}
	}

	void OnTriggerStay(Collider col){
		if(col.gameObject.CompareTag("Throwable")){
			if(device.GetPressUp(SteamVR_Controller.ButtonMask.Grip)){ 
				isGrabbing = false;
	// 			ThrowObject(col);
			} else if(device.GetPressDown(SteamVR_Controller.ButtonMask.Grip)){
				isGrabbing = true;
	// 			GrabObject(col);
			}
		}
	}

	void ThrowObject(){

	}

	void GrabObject(){

	}

	void PlaceObject(){

	}
}