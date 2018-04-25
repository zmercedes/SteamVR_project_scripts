/* Zoilo Mercedes
 * Arc renderer
 * Activates on left touchpad press, renders an arc
 */
using UnityEngine;

public class ArcRenderer : MonoBehaviour {

	// mesh components
	private Mesh mesh;
	private MeshCollider meshCollider;

	// controller reference
	private Transform controller;
	private float lastRotation;

	// teleport aimer object
	public GameObject aimerObject;

	// arc information
	public float meshWidth;
	public int resolution;
	public float time;
	public float timeToTarget;
	public float speed = 10f;
	public float g = -18f;
	public Color[] colorIndicators;
	public float multiplier = 1.15f;
	private Material arcMat;
	private Vector3 velocity;

	void Awake(){
		// set controller reference
		controller = transform.parent.GetChild(0);

		// set material reference
		arcMat = GetComponent<Renderer>().material;

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

		// disable aimer object and arc when tilting above 45 degrees and below -90 degrees (max and min teleport distances)
		if(controller.eulerAngles.x < 300 && controller.eulerAngles.x > 90){
			arcMat.color = colorIndicators[1];
			timeToTarget = 0.01f;
			aimerObject.SetActive(false);
		} else {
			arcMat.color = colorIndicators[0];
			timeToTarget = time;
		}

		// set velocity to shoot forward from controller
		velocity = controller.forward * speed;

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

			Debug.DrawLine (previousDrawPoint, drawPoint, Color.green);
			previousDrawPoint = drawPoint;
			if(transform.InverseTransformPoint(drawPoint).y <= aimerObject.transform.position.y){
				timeToTarget = ( i / (float)resolution * time ) * multiplier;
				// break;
			}
		}

		previousDrawPoint = controller.position;
		for(int i = 1; i <= resolution; i++){
			float simulationTime = i / (float)resolution * timeToTarget;
			Vector3 displacement = velocity * simulationTime + Vector3.up * g * simulationTime * simulationTime / 2f;
			Vector3 drawPoint = controller.position + displacement;

			arcArray[i] = transform.InverseTransformPoint(previousDrawPoint); // issue with calculation: local -> world points is annoying
			previousDrawPoint = drawPoint;
			if(i == resolution){
				int mask = 1 << 8;
				RaycastHit hit;
				if(Physics.Raycast(drawPoint, Vector3.down, out hit, 10f, mask))
					aimerObject.transform.position = hit.transform.position;
			}

		}
		return arcArray;
	}

	void OnCollisionEnter(Collision other){
		aimerObject.SetActive(true);

		if(other.gameObject.tag == "platform"){
			Vector3 platform = other.transform.position;
			aimerObject.transform.position = new Vector3(platform.x, 0.44f, platform.z);
		}
		if(other.gameObject.tag == "teleport"){
			Vector3 contact = other.contacts[0].point;
			Vector3 startPos = aimerObject.transform.position;
			Vector3 endPos = new Vector3(contact.x,0.02f,contact.z);
			aimerObject.transform.position = Vector3.Lerp(startPos, endPos, 0.75f);
		}
	}
}