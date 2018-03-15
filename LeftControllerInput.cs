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

public class LeftControllerInput : MonoBehaviour {

	// controller references
	SteamVR_Controller.Device controller;
	SteamVR_TrackedObject trackedObj;

	// reference to right controller
	GameObject rightController;
	bool isGrabbing = false;

	// teleporting
	GameObject arc;
	ArcRenderer arcRenderer;
	private LayerMask teleMask;
	private Vector3 teleportLocation;
	private GameObject player;

	void Awake () {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
		rightController = transform.parent.GetChild(1).gameObject;
		arc = transform.GetChild(1).gameObject;
		arcRenderer = arc.GetComponent<ArcRenderer>();
		player = transform.parent.gameObject;
	}
	
	void Update () {
		controller = SteamVR_Controller.Input((int)trackedObj.index);

		// if(controller.GetPressDown)

		if(controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad) && !arc.activeSelf)
			arc.SetActive(true);

		// teleport code
		if(controller.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad) && arc.activeSelf){
			if(arcRenderer.aimerObject.activeSelf){
				player.transform.position = arcRenderer.aimerObject.transform.position;
				print("teleported to " + transform.position);
			}
			arcRenderer.aimerObject.SetActive(false);
			arc.SetActive(false);
		}

		if(controller.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
			Debug.Log("left touched!");

	}

	void OnTriggerStay(Collider col){
		if(col.gameObject.CompareTag("Throwable")){
			if(controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip)){ 
				isGrabbing = false;
				// ThrowObject(col);
			} else if(controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip)){
				isGrabbing = true;
				// GrabObject(col);
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
