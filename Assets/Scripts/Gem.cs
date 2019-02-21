using UnityEngine;
using System.Collections;

public class Gem : MonoBehaviour {
    private Transform m_Transform;
    private Transform m_gem;

	void Start () {
        m_Transform = gameObject.GetComponent<Transform>();
        m_gem = m_Transform.Find("gem 3").GetComponent<Transform>();
	}
	

	void Update () {
        m_gem.Rotate(Vector3.up);
	}
}
