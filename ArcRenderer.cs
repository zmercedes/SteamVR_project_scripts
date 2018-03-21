using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcRenderer : MonoBehaviour {

	Mesh mesh;
	public float meshWidth;
	private MeshCollider meshCollider;

	// shows teleport point
	public GameObject aimerObject;

	// rework stuff
	public Vector3 velocity;
	public int resolution = 30;
	public float time = 5f;
	public float speed = 10f;
	public float g = -18f;

	void Awake(){
		Physics.gravity = transform.up * g;
		mesh = GetComponent<MeshFilter>().mesh;
		meshCollider = GetComponent<MeshCollider>();
		// RenderArc(CalculateArcArray());
	}

	// void OnValidate(){
	// 	if(mesh != null && Application.isPlaying)
	// 		RenderArc(CalculateArcArray());
	// }

	void Update(){
		if(transform.parent.eulerAngles.x < 315 && transform.parent.eulerAngles.x > 90)
			aimerObject.SetActive(false);

		velocity = transform.parent.forward * speed;

		DrawPath();

		// RenderArc(CalculateArcArray());
	}

	void RenderArc(Vector3[] arcVerts){
		mesh.Clear();
		Vector3[] vertices = new Vector3[(resolution + 1) * 2];
		int[] triangles = new int[resolution * 6 * 2];
		
		for(int i = 0; i <= resolution; i++){
			// set vertices
			vertices[i*2] = new Vector3(meshWidth * 0.5f, arcVerts[i].y, arcVerts[i].x);
			vertices[i*2 +1] = new Vector3(meshWidth * -0.5f, arcVerts[i].y, arcVerts[i].x);
		
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
	
	// calculates points for arc
	Vector3[] CalculateArcArray(){
		Vector3[] arcArray = new Vector3[resolution +1];
		Vector3 previous = transform.position;

		// float maxDist = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / g;

		for(int i = 1; i <= resolution; i++){
			float t = i / (float)resolution * time;
			Vector3 displacement = velocity * t + transform.up * g * time * time / 2f;
			//arcArray[i] = transform.position + displacement;
			previous = arcArray[i];
		}

		return arcArray;
	}

	void DrawPath() {
		Vector3 previousDrawPoint = transform.position;

		for (int i = 1; i <= resolution; i++) {
			float simulationTime = i / (float)resolution * time;
			Vector3 displacement = velocity * simulationTime + transform.up *g * simulationTime * simulationTime / 2f;
			Vector3 drawPoint = transform.position + displacement;
			Debug.DrawLine (previousDrawPoint, drawPoint, Color.green);
			previousDrawPoint = drawPoint;
		}
	}

	// calculates individual points in the arc using 
	// 
	// Vector3 CalculateArcPoint(float t, float maxDist){
	// 	float x = t * maxDist;
	//     float y = x * Mathf.Tan(radianAngle) - ((g * x * x) / (2 * velocity * velocity * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));
	// 	return new Vector3(x,y);
	// }

	void OnCollisionEnter(Collision other){
		if(other.gameObject.tag == "teleport"){
			aimerObject.SetActive(true);
			Vector3 contact = other.contacts[other.contacts.Length / 2].point;
			Vector3 startPos = aimerObject.transform.position;
			Vector3 endPos = new Vector3(contact.x,0.03f,contact.z);
			aimerObject.transform.position = Vector3.Lerp(startPos, endPos, 0.75f);
		}
	}

}