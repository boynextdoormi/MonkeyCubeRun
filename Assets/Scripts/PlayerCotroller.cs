using UnityEngine;
using System.Collections;

/// <summary>
/// 角色控制
/// </summary>

public class PlayerCotroller : MonoBehaviour {
    //角色位置通过x和z进行控制
    public int x = 3;//左右方向
    public int z = 2;//上下方向

    private Color colorOne =new Color(122/255f,85/255f,179/255f);//蜗牛痕迹，第一排
    private Color colorTwo =new Color(126/255f,93/255f,183/255f);//蜗牛痕迹，第二排

    private CameraFollow m_CameraFollow;

    private Transform m_Transform;

    private MapManager m_MapManager;
    private UIManager m_UIManager;

    private bool life = true;//角色状态
    private int gemCount = 0;//宝石数量
    private int scoreCount = 0;//角色移动分数

    /// <summary>
    /// 增加宝石数量
    /// </summary>
    private void AddGemCount()
    {
        if (gemCount<100)
        {
            gemCount++;
        }
        Debug.Log("宝石数："+gemCount);
        m_UIManager.UpdateDate(scoreCount, gemCount);//更新到UI
    }
    /// <summary>
    /// 增加移动分数
    /// </summary>
    private void AddScoreCount()
    {
        scoreCount++;
        Debug.Log("分数：" + scoreCount);
        m_UIManager.UpdateDate(scoreCount, gemCount);
    }

    private void SaveDate()//保存数据
    {
        PlayerPrefs.SetInt("gem",gemCount);
        if (scoreCount > PlayerPrefs.GetInt("score",0))
        {
            PlayerPrefs.SetInt("score",scoreCount);
        }
    }

	void Start () {

        gemCount = PlayerPrefs.GetInt("gem",0);//从注册表取出宝石数量

        m_CameraFollow =GameObject.Find("Main Camera").GetComponent<CameraFollow>();//获取Camera Follow脚本

	    m_Transform =gameObject.GetComponent<Transform>();//获取角色Transform组件

        m_MapManager =GameObject.Find("MapManager").GetComponent<MapManager>();//获取一个MapManager对象
        
        m_UIManager = GameObject.Find("UI Root").GetComponent<UIManager>();//获取一个UIManager脚本
	}


    public void StartGame()//开始游戏
    {
        SetPlayerPos();//重置角色位置
        m_CameraFollow.startFollow = true;//摄像机自动跟随
        m_MapManager.StartTileDown();//开始地面塌陷协程
    }

	void Update () {
        //重置角色位置
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartGame();
        }

        if (life ==true)//能否控制角色
        {
             PlayerControl();
        }
       

	}

    public void Left()//向左移动
    {
        if (z != 0)          //边界控制
        {
            x++;//向前
            AddScoreCount();
        }
        if (x % 2 == 1 && z != 0)//奇数行左移
        {
            z--;
        }
        Debug.Log("Left:" + "X:" + x + " " + "Z:" + z);
        SetPlayerPos();
        CalcPosition();
    }

    public void Right()//向右移动
    {
        if (z != 4 || x % 2 != 1)
        {
            x++;//向前
            AddScoreCount();
        }
        if (x % 2 == 0 && z != 4)//偶数行右移
        {
            z++;
        }
        Debug.Log("Right:" + "X:" + x + " " + "Z:" + z);
        SetPlayerPos();
        CalcPosition();
    }


    /// <summary>
    /// 角色移动控制
    /// </summary>
    private void PlayerControl()
    {
        //向左移动
        if (Input.GetKeyDown(KeyCode.A))
        {
            Left();
        }
        //向右移动
        if (Input.GetKeyDown(KeyCode.D))
        {
            Right();
        }
    }

    /// <summary>
    /// 设置角色位置和旋转 生成蜗牛痕迹
    /// </summary>
    private void SetPlayerPos()
    {
        //将角色位置、旋转固定(设置Map List中的元素位置)
        Transform PlayerPos = m_MapManager.mapList[x][z].GetComponent<Transform>();
        //角色位置、旋转固定
        m_Transform.position = PlayerPos.position + new Vector3(0, 0.137f, 0);
        m_Transform.rotation = PlayerPos.rotation;

        MeshRenderer normal_a2 = null;
        if (PlayerPos.tag =="Tile")
        {
            normal_a2 = PlayerPos.Find("normal_a2").GetComponent<MeshRenderer>();//获取地板的子物体MeshRenderer组件
        }
        else if (PlayerPos.tag =="Spike")
        {
            normal_a2 = PlayerPos.Find("moving_spikes_a2").GetComponent<MeshRenderer>();
        }
        else if (PlayerPos.tag == "Sky_Spike")
        {
            normal_a2 = PlayerPos.Find("smashing_spikes_a2").GetComponent<MeshRenderer>();
        }


        if (normal_a2 != null)
        {
            if (z % 2 == 0)//给子物体赋色
            {
                normal_a2.material.color = colorOne;
            }
            else
            {
                normal_a2.material.color = colorTwo;
            }
        }
        else
        {
            gameObject.AddComponent<Rigidbody>();
            StartCoroutine("GameOver",true);
        }
        
    }

    /// <summary>
    /// 计算角色当前位置
    /// </summary>
    private void CalcPosition()
    {
        if (m_MapManager.mapList.Count-x <= 12)//还有12行就开始生成后续地图
        {
            Debug.Log("生成后续地图");
            m_MapManager.AddPR();
            //获取MapList最后一行第一个元素的位置.z
            float offsetZ = m_MapManager.mapList[m_MapManager.mapList.Count -1][0].GetComponent<Transform>().position.z +m_MapManager.bottomlenth/2;
            m_MapManager.CreatMapItem(offsetZ);
        }
    }


    /// <summary>
    /// 碰撞触发
    /// </summary>
    /// <param name="coll"></param>
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.tag=="Spike_Attack")
        {
            StartCoroutine("GameOver",false);
        }
        if (coll.tag == "Gem")
        {
            //销毁碰撞体的父物体
            GameObject.Destroy(coll.gameObject.GetComponent<Transform>().parent.gameObject);
            AddGemCount();
        }
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public IEnumerator GameOver(bool b)
    {
        if (b)
        {
            yield return new WaitForSeconds(0.5f);
        }

        if (life)
        {
            Debug.Log("游戏结束");
            m_CameraFollow.startFollow=false;
            life = false;
            SaveDate();
            //TODO:UI相关交互
            StartCoroutine("ResetGame");
        }
        //Time.timeScale = 0;
    }

    private void ResetPlayer()//重置角色
    {
        GameObject.Destroy(gameObject.GetComponent<Rigidbody>());//移除角色刚体组件
        //角色位置通过x和z进行重置
        x = 3;//左右方向
        z = 2;//上下方向
        life = true;//重置角色控制
        scoreCount = 0;//重置移动分数
 
    }

    private IEnumerator ResetGame()//重启游戏
    {
        yield return new WaitForSeconds(1);
        m_UIManager.ResetUI();
        ResetPlayer();
        m_MapManager.ResetGameMap();
        m_CameraFollow.ResetCamera();
    }
}
