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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LeftControllerInput : MonoBehaviour {

	// controller references
	private SteamVR_Controller.Device controller;
	private SteamVR_TrackedObject trackedObj;

	// teleporting
	public GameObject arc;
	private ArcRenderer arcRenderer;
	private LayerMask teleMask;
	private Vector3 teleportLocation;
	private GameObject player;

	void Awake () {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
		// arc = transform.GetChild(1).gameObject;
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

	public float throwForce = 1.5f;

	void OnTriggerStay(Collider col){
		if(col.gameObject.CompareTag("Throwable")){
			if(controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip)){ 
				// ThrowObject(col);
			} else if(controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip)){
				// GrabObject(col);
			}
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

}
