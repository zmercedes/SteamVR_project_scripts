/* Controller Input Manager
 0. set up observer pattern on script -> scrapped
 0. set up hand control scripts       -> done
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
using UnityEngine;
using System;

public class RightControllerInput : MonoBehaviour {

	// controller references
	SteamVR_Controller.Device controller;
	SteamVR_TrackedObject trackedObj;

	// testing pane with touchpad
	GameObject objectMenu;
	private float touchLast;
	private float touchCurrent;
	private float distance;
	public float swipeSpeed = 50;
	// private bool hasSwipedLeft;
	// private bool hasSwipedRight;

	void Awake () {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
		objectMenu = transform.GetChild(1).gameObject;
	}

	void Update () {
		controller = SteamVR_Controller.Input((int)trackedObj.index);

		// right hand functions
			if(controller.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
				Debug.Log("right touched!");

			// setting initial touch position for swiping
			if(controller.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
				touchLast = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
			
			// activate/deactivate objectMenu on touchpad press
			if(controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)){
				Debug.Log("right tp pressed down!");
				objectMenu.SetActive(!objectMenu.activeSelf);
			}

			// if objectMenu is active and player is touching touchpad,
			// rotate the objectMenu based on swiping left/right on touchpad.
			if(objectMenu.activeSelf && controller.GetTouch(SteamVR_Controller.ButtonMask.Touchpad)){

				touchCurrent = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
				distance = touchCurrent - touchLast;
				touchLast = touchCurrent;

				objectMenu.transform.Rotate(Vector3.forward * distance * swipeSpeed);
			}
	}

	public float throwForce = 1.5f;

	void OnTriggerStay(Collider col){
		if(col.gameObject.CompareTag("grabable")){
			if(controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
				ThrowObject(col);
			else if(controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
				GrabObject(col);
		}
	}

	void GrabObject(Collider coll){
		coll.transform.SetParent(gameObject.transform);     // make controller parent
		coll.GetComponent<Rigidbody>().isKinematic = true;  // turn off physics
		controller.TriggerHapticPulse(2000);				// vibrate controller
		Debug.Log("Grabbing object!");
	}

	void ThrowObject(Collider coll){
		coll.transform.SetParent(null);
		Rigidbody rigidBody = coll.GetComponent<Rigidbody>();
		rigidBody.isKinematic = false;
		rigidBody.velocity = controller.velocity * throwForce;
		rigidBody.angularVelocity = controller.angularVelocity;
		Debug.Log("Released object!");
	}

	// void ThrowObject(){

	// }

	// void GrabObject(){

	// }

	// void PlaceObject(){

	// }
}
