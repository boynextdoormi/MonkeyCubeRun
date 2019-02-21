using UnityEngine;
using System.Collections;

/// <summary>
/// 摄像机跟随角色移动
/// </summary>
public class CameraFollow : MonoBehaviour {

    private Transform m_Transform;//相机Transform
    private Transform m_Player;//角色Transform
    public bool startFollow = false;
    private Vector3 normalPos;//摄像机原始位置

	void Start () {
        //获取相应组件
        m_Transform = gameObject.GetComponent<Transform>();
        normalPos = m_Transform.position;
        m_Player =GameObject.Find("cube_books").GetComponent<Transform>();


	}
	

	void Update () {
        CameraMove();
	
	}

    /// <summary>
    /// 摄像机移动
    /// </summary>
    void CameraMove()
    {
        if (startFollow == true)
        {
            //摄像机开始跟随
            Vector3 nextPos = new Vector3(m_Transform.position.x, m_Player.position.y + 1.6f, m_Player.position.z);
            //m_Transform.position = nextPos;平滑处理镜头跟随
            m_Transform.position = Vector3.Lerp(m_Transform.position, nextPos,Time.deltaTime);
            
        }
    }


    public void ResetCamera()//重置摄像机位置
    {
        m_Transform.position = normalPos;
    }
}
