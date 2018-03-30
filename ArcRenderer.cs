using UnityEngine;

public class ArcRenderer : MonoBehaviour {

	// mesh components
	public float meshWidth;
	private Mesh mesh;
	private MeshCollider meshCollider;

	// controller reference
	public Transform controller;
	private float lastRotation;

	// teleport aimer object
	public GameObject aimerObject;

	// arc information
	public int resolution = 30;
	public float time = 5f;
	public float speed = 10f;
	public float g = -18f;
	private Vector3 velocity;

	// debug stuff
	public bool debugPath = false;
	public bool textDebug = false;
	public string textLocation = @"C:\Users\Zoilo\Desktop\rube goldberg VR\High-Immersion-Starter-Project-master\arcdebug2.txt";

	void Awake(){
		// set mesh components
		mesh = GetComponent<MeshFilter>().mesh;
		meshCollider = GetComponent<MeshCollider>();

		// set initial y rotation of controller
		lastRotation = controller.eulerAngles.y;

		// sets appropriate rotation for arc
		transform.Rotate(new Vector3(0f,controller.eulerAngles.y + 90f,0f));
	}

	void Update(){
		// set point of origin to controller
		transform.position = controller.position;

		// applying y rotation
		if(controller.eulerAngles.y != lastRotation){
			transform.Rotate(new Vector3(0f, controller.eulerAngles.y - lastRotation,0f));
			lastRotation = controller.eulerAngles.y;
		}

		// disable aimer object and arc when tilting above 45 degrees and below -90 degrees
		if(controller.eulerAngles.x < 315 && controller.eulerAngles.x > 90)
			aimerObject.SetActive(false);

		// set velocity to shoot forward from controller
		velocity = controller.forward * speed;

		// if(textDebug){
		// 	using (System.IO.StreamWriter file = new System.IO.StreamWriter(textLocation, true)){
		// 		file.WriteLine("transform.position: " + transform.position.ToString() +
		// 					   "\ntransform.forward: " + transform.forward.ToString());
		// 	}
		// }

		RenderArc(ArcArray());
	}

	void RenderArc(Vector3[] arcVerts){
		mesh.Clear();
		Vector3[] vertices = new Vector3[(resolution + 1) * 2];
		int[] triangles = new int[resolution * 6 * 2];
		
		for(int i = 0; i <= resolution; i++){
			// set vertices
			vertices[i*2] = new Vector3(arcVerts[i].x, arcVerts[i].y, meshWidth * 0.5f);
			vertices[i*2 +1] = new Vector3(arcVerts[i].x, arcVerts[i].y, -meshWidth * 0.5f);
		
			// set triangles
			if(i != resolution){
				triangles[i*12] = i*2;
				triangles[i*12 +1] = triangles[i*12 +4] = i*2 +1;
				triangles[i*12 +2] = triangles[i*12 +3] = (i+1) *2;
				triangles[i*12 +5] = (i+1) *2 +1;

				triangles[i*12 +6] = i*2;
				triangles[i*12 +7] = triangles[i*12 +10] = (i+1) *2;
				triangles[i*12 +8] = triangles[i*12 +9] = i*2 +1;
				triangles[i*12 +11] = (i+1) *2 +1;
			}
			
			mesh.vertices = vertices;
			mesh.triangles = triangles;
		}
		// if(textDebug){
		// 	using (System.IO.StreamWriter file = new System.IO.StreamWriter(textLocation, true)){
		// 		for(int i = 0; i < vertices.Length; i++)
		// 			file.WriteLine(" vertex " + i + ": " + vertices[i].ToString());
		// 	}
		// }

		meshCollider.sharedMesh = mesh;
		mesh.RecalculateBounds(); // must call this for mesh to be seen even when out of camera bounds
	}

	Vector3[] ArcArray() {
		Vector3[] arcArray = new Vector3[resolution +1];
		Vector3 previousDrawPoint = controller.position;

		for (int i = 1; i <= resolution; i++) {
			float simulationTime = i / (float)resolution * time;
			Vector3 displacement = velocity * simulationTime + Vector3.up * g * simulationTime * simulationTime / 2f;
			Vector3 drawPoint = controller.position + displacement;
			if(debugPath){
				Debug.DrawLine (previousDrawPoint, drawPoint, Color.green);
				// if(textDebug){
				// 	using (System.IO.StreamWriter file = new System.IO.StreamWriter(textLocation, true)){
				// 		file.WriteLine(" point " + i + ": " + previousDrawPoint.ToString());
				// 	}
				// }
			}
			arcArray[i] = transform.InverseTransformPoint(previousDrawPoint); // issue with calculation: local -> world points are annoying
			previousDrawPoint = drawPoint;
		}
		return arcArray;
	}

	void OnCollisionEnter(Collision other){
		if(other.gameObject.tag == "teleport"){
			aimerObject.SetActive(true);
			Vector3 contact = other.contacts[other.contacts.Length / 2].point;
			Vector3 startPos = aimerObject.transform.position;
			Vector3 endPos = new Vector3(contact.x,0.02f,contact.z);
			aimerObject.transform.position = Vector3.Lerp(startPos, endPos, 0.75f);
		}
	}
}