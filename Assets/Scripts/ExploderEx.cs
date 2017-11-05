/// <summary>
/// Exploder.cs
/// Self Contained utility that allows exploting a mesh into multiple triangle pieces.
/// For questions or support:
/// http://www.imagitechdj.com
/// </summary>
using UnityEngine;
using System.Collections;

public class ExploderEx : MonoBehaviour
{
	/// <summary>
	/// The Exploder force.
	/// </summary>
	public float exploderForce;
	/// <summary>
	/// The Exploder radius.
	/// </summary>
	public float exploderRadius;
	/// <summary>
	/// The Exploder slow motion amount.
	/// </summary>
	public float exploderSlowMotionAmount = 0.2f;
	/// <summary>
	/// The destroy pieces after amount of seconds specified.
	/// </summary>
	public float destroyPiecesAfterSeconds = 5.0f;
	/// <summary>
	/// The mesh piece prefix.
	/// </summary>
	public string meshPiecePrefix = "Piece";
	/// <summary>
	/// The triangle points. Do not change this as you need 3 points for a triangle
	/// </summary>
	private int trianglePoints = 3;

	/// <summary>
	/// The max indices. This number must carefully watched as it could
	/// lead to hight CPU consumption. It determines how many debris to create
	/// during the explotion.
	/// </summary>
	private int maxIndicesTotal = 500;

	/// <summary>
	/// Creates the Exploder piece game object.
	/// </summary>
	/// <returns>The Exploder piece game object.</returns>
	/// <param name="i">The index.</param>
	/// <param name="mesh">Mesh.</param>
	/// <param name="MR">M.</param>
	GameObject CreateExploderPieceGameObject (int i, Mesh mesh, MeshRenderer MR, int submesh)
	{
		GameObject pieceGameObject = new GameObject (meshPiecePrefix + (i / 3));
		pieceGameObject.transform.position = transform.position;
		pieceGameObject.transform.rotation = transform.rotation;
		pieceGameObject.AddComponent<MeshRenderer> ().material = MR.materials [submesh];
		pieceGameObject.AddComponent<MeshFilter> ().mesh = mesh;
		pieceGameObject.AddComponent<BoxCollider> ();
		pieceGameObject.AddComponent<Rigidbody> ().AddExplosionForce (exploderForce, transform.position, exploderRadius);
		return pieceGameObject;
	}

	/// <summary>
	/// Divides the mesh.
	/// </summary>
	/// <returns>The mesh.</returns>
	IEnumerator DivideMesh ()
	{
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		Mesh mesh = meshFilter.mesh;
		Vector3[] vertices = mesh.vertices;
		Vector3[] normals = mesh.normals;
		Vector2[] uvs = mesh.uv;
		for (int submesh = 0; submesh < mesh.subMeshCount; submesh++)
		{
			int[] indices = mesh.GetTriangles(submesh);

			//Add limits to avoid performance issues (delays)
			int maxIndices = indices.Length >= maxIndicesTotal ? maxIndicesTotal : indices.Length;

			for (int i = 0; i < maxIndices; i += trianglePoints)
			{
				Vector3[] newVertices = new Vector3[trianglePoints];
				Vector3[] newNormals = new Vector3[trianglePoints];
				Vector2[] newUvs = new Vector2[trianglePoints];
				for (int n = 0; n < trianglePoints; n++)
				{
					int index = indices[i + n];
					newVertices[n] = vertices[index];
					newUvs[n] = uvs[index];
					newNormals[n] = normals[index];
				}
				Mesh currentMesh = new Mesh();
				currentMesh.vertices = newVertices;
				currentMesh.normals = newNormals;
				currentMesh.uv = newUvs;
				
				currentMesh.triangles = new int[] { 0, 1, 2, 2, 1, 0 };
				//Calculate Mesh Tangets
				MeshTangents.calculateMeshTangents(currentMesh);

				GameObject temporaryGameObject = CreateExploderPieceGameObject (i, currentMesh, meshRenderer, submesh);
				Destroy(temporaryGameObject, destroyPiecesAfterSeconds + Random.Range(0.0f, destroyPiecesAfterSeconds));
			}
		}
		meshRenderer.enabled = false;
		Time.timeScale = exploderSlowMotionAmount;
		yield return new WaitForSeconds(0.2f);
		Time.timeScale = 1.0f;
		Destroy(gameObject);
	}
	void OnTriggerEnter(Collider col){
		if(col.tag == "SpaceshipBullet"){
			StartCoroutine(DivideMesh());
		}
	}

	public void Crash()
	{
		StartCoroutine(DivideMesh());
	}
}