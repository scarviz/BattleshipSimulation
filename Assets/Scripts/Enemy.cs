using UnityEngine;

public class Enemy : MonoBehaviour
{
	public Transform m_cannonRot;
	public Transform m_muzzle;
	public GameObject m_shotPrefab;
	public float coefficient = 1f;

	private const float INTERVAL = 3f;
	private float elapsed = 0f;
	private float randomVal = 0f;

	// Use this for initialization
	void Start () {
		if(coefficient == 0)
		{
			coefficient = 1f;
		}

		randomVal = RandomVal();
	}
	
	// Update is called once per frame
	void Update ()
	{
		elapsed += Time.deltaTime * coefficient * randomVal;
		if (INTERVAL < elapsed)
		{
			GameObject go = GameObject.Instantiate(m_shotPrefab, m_muzzle.position, m_muzzle.rotation) as GameObject;
			GameObject.Destroy(go, 3f);
			elapsed = 0;
			randomVal = RandomVal();
		}
	}

	private float RandomVal()
	{
		return Random.Range(0.5f, 1.5f);
	}
}
