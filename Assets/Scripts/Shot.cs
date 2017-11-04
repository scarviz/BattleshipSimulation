using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Shot : MonoBehaviour {
	private const string REQUEST_URL = "https://blooming-brushlands-57006.herokuapp.com/pulldata";

	public Transform m_cannonRot;
	public Transform m_muzzle;
	public GameObject m_shotPrefab;

	private class Message
	{
		public string intent;
	}

	private class MessType
	{
		public const string SHOT = "shot";
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update ()
	{
		StartCoroutine(GetText());
	}

	IEnumerator GetText()
	{
		var request = UnityWebRequest.Get(REQUEST_URL);

		// リクエスト送信
		yield return request.Send();

		// 通信エラーチェック
		if (request.isError)
		{
			Debug.Log(request.error);
		}
		else if (request.responseCode == 200)
		{
			// UTF8文字列として取得する
			var json = request.downloadHandler.text;
			Debug.Log(json);

			if (string.IsNullOrEmpty(json) || json == "null") yield return null;

			var mes = LitJson.JsonMapper.ToObject<Message>(json);
			if (mes == null || string.IsNullOrEmpty(mes.intent)) yield return null;

			switch (mes.intent)
			{
				case MessType.SHOT:
					shot();
					break;
				default:
					break;
			}

		}
	}

	private void shot()
	{
		GameObject go = GameObject.Instantiate(m_shotPrefab, m_muzzle.position, m_muzzle.rotation) as GameObject;
		GameObject.Destroy(go, 3f);
	}
}
