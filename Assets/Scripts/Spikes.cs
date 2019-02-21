using UnityEngine;
using System.Collections;

/// <summary>
/// 地面陷阱动画
/// </summary>
public class Spikes : MonoBehaviour {
    private Transform m_Transform;
    private Transform son_Transform;

    private Vector3 normalPos;
    private Vector3 targetPos;

	void Start () {
        m_Transform = gameObject.GetComponent<Transform>();
        son_Transform =m_Transform.Find("moving_spikes_b").GetComponent<Transform>();

        normalPos = son_Transform.position;
        targetPos = son_Transform.position + new Vector3(0,0.15f,0);

        StartCoroutine("UpAndDown");
	}

    private IEnumerator UpAndDown()
    {
        while (true)
        {
            StopCoroutine("Down");
            StartCoroutine("Up");
            yield return new WaitForSeconds(2.0f);
            StopCoroutine("Up");
            StartCoroutine("Down");
            yield return new WaitForSeconds(2.0f);


        }
    }

    private IEnumerator Up()
    {
        while (true)
        {
            son_Transform.position = Vector3.Lerp(son_Transform.position, targetPos,Time.deltaTime*50);
            yield return null;
        }
    }

    private IEnumerator Down()
    {
        while (true)
        {
            son_Transform.position = Vector3.Lerp(son_Transform.position, normalPos, Time.deltaTime * 10);
            yield return null;
        }
    }
   
}
