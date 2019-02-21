using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 地图管理器
/// </summary>
public class MapManager : MonoBehaviour {
    private GameObject m_prefab_tile;//地板元素
    private GameObject m_prefab_wall;//墙体元素
    private GameObject m_prefab_spikes;//地面陷阱
    private GameObject m_prefab_sky_spikes;//天空陷阱
    private GameObject m_prefab_gem;//奖励物品


    private int pr_hole = 0; //路障概率
    private int pr_spikes = 0;//地面陷阱概率
    private int pr_sky_spikes = 0;//天空陷阱概率
    private int pr_gem = 2;//宝石概率

    public List<GameObject[]> mapList = new List<GameObject[]>();//地图数组集合

    private Transform m_Transform;//Map Manager的Transform组件
    private PlayerCotroller m_PlayerCotroller;

    public  float bottomlenth = 0.254f * Mathf.Sqrt(2);//地板元素的对角线长
    private int index = 0;//当前塌陷的行数

    private Color colorOne = new Color(124 / 255f, 155 / 255f, 230 / 255f);//第一排地板颜色
    private Color colorTwo = new Color(125 / 255f, 169 / 255f, 233 / 255f);//第二排地板颜色
    private Color colorWall = new Color(87 / 255f, 93 / 255f, 169 / 255f);//墙体颜色

	void Start () {
        
        //获取相关元素
        m_prefab_tile = Resources.Load("tile_white") as GameObject;
        m_prefab_wall = Resources.Load("wall2") as GameObject;
        m_prefab_spikes = Resources.Load("moving_spikes") as GameObject;
        m_prefab_sky_spikes = Resources.Load("smashing_spikes") as GameObject;
        m_prefab_gem = Resources.Load("gem 2") as GameObject;

        m_Transform = gameObject.GetComponent<Transform>();//给父子关系创造一个Transform桥梁
        m_PlayerCotroller = GameObject.Find("cube_books").GetComponent<PlayerCotroller>();//获取角色的PlayerController

        CreatMapItem(0);//从0开始生成地图
        
	}
    /// <summary>
    /// 创建地图元素
    /// </summary>
    public void CreatMapItem(float offsetZ)
    {
        //第一排
        for (int j = 0; j < 10; j++)
        {
            GameObject[] item =new GameObject[6];//定义一行数组  深色
            for (int i = 0; i < 6; i++)
            {
                //实例化地板
                Vector3 pos = new Vector3(i * bottomlenth, 0, offsetZ + j * bottomlenth);//让元素平铺
                Vector3 rot = new Vector3(-90,45,0);//沿X轴旋转90度，让地板的显色面朝上
                Quaternion qua = Quaternion.Euler(rot);//将Vector3类型转换为四元数   Quaternion.Euler(Vector3);
                GameObject tile = null;
                if (i== 0 || i== 5)
                {
                    tile = GameObject.Instantiate(m_prefab_wall, pos, qua) as GameObject;//生成墙壁
                    tile.GetComponent<MeshRenderer>().material.color = colorWall;
                }
                else
                {
                    int pr = CalcPR();
                    if (pr ==0)//瓷砖
                    {
                        tile = GameObject.Instantiate(m_prefab_tile, pos, qua) as GameObject;
                        tile.GetComponent<Transform>().Find("normal_a2").GetComponent<MeshRenderer>().material.color = colorOne;//给元素的面填色
                        tile.GetComponent<MeshRenderer>().material.color = colorOne;//给元素填色
                        int gemPR = CalcGemPR();
                        if (gemPR==1)
                        {
                            //生成宝石
                            GameObject gem =GameObject.Instantiate(m_prefab_gem, tile.GetComponent<Transform>().position+new Vector3(0,0.06f,0),Quaternion.identity) as GameObject;
                            gem.GetComponent<Transform>().SetParent(tile.GetComponent<Transform>());
                            
                        }



                    }else if(pr ==1)//坑洞
                    {
                        tile = new GameObject();//空物体
                        tile.gameObject.GetComponent<Transform>().position =pos;
                        tile.gameObject.GetComponent<Transform>().rotation =qua;
                    }
                    else if (pr==2)//地面陷阱
                    {
                        tile = GameObject.Instantiate(m_prefab_spikes, pos, qua) as GameObject;
                    }else if (pr==3)//天空陷阱
                    {
                        tile = GameObject.Instantiate(m_prefab_sky_spikes, pos, qua) as GameObject;
                    }
                    



                 }
               //将生成的地板元素的父物体设置为MapManager
                tile.GetComponent<Transform>().SetParent(m_Transform );
                item[i] = tile;//将地板添加到数组
            } mapList.Add(item);//再将数组加入到地图集和
           

            //第二排 浅色
            GameObject[] item2 =new GameObject[5];
            for (int i = 0; i < 5; i++)
            {
                //实例化地板
                Vector3 pos = new Vector3(i * bottomlenth + bottomlenth / 2, 0,offsetZ + j * bottomlenth + bottomlenth / 2);//让元素平铺
                Vector3 rot = new Vector3(-90, 45, 0);//沿X轴旋转90度，让地板的显色面朝上
                Quaternion qua = Quaternion.Euler(rot);//将Vector3类型转换为四元数   Quaternion.Euler(Vector3);
                GameObject tile =null;

                int pr = CalcPR();
                if (pr == 0)//瓷砖
                {
                    tile = GameObject.Instantiate(m_prefab_tile, pos, qua) as GameObject;
                    tile.GetComponent<Transform>().Find("normal_a2").GetComponent<MeshRenderer>().material.color = colorTwo;//给元素的面填色
                    tile.GetComponent<MeshRenderer>().material.color = colorTwo;//给元素填色
                }
                else if (pr == 1)//坑洞
                {
                    tile = new GameObject();//空物体
                    tile.gameObject.GetComponent<Transform>().position = pos;
                    tile.gameObject.GetComponent<Transform>().rotation = qua;
                }
                else if (pr == 2)//地面陷阱
                {
                    tile = GameObject.Instantiate(m_prefab_spikes, pos, qua) as GameObject;
                }
                else if (pr == 3)//天空陷阱
                {
                    tile = GameObject.Instantiate(m_prefab_sky_spikes, pos, qua) as GameObject;
                }

                //将生成的地板元素的父物体设置为MapManager
                tile.GetComponent<Transform>().SetParent(m_Transform);
                item2[i] = tile;
            } mapList.Add(item2);
        }



        }

        
	

	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string str = "";
            for (int i = 0; i < mapList.Count; i++)
            {
                for (int j = 0; j < mapList[i].Length; j++)
                {
                    str += mapList[i][j].name;
                    mapList[i][j].name = i + "--"+j;
                }
                str += "\n";
            }
            Debug.Log(str);
        }
	}


    /// <summary>
    /// 开启地面塌陷效果
    /// </summary>
    public void StartTileDown()
    {
        StartCoroutine("TileDown");
    }
    /// <summary>
    /// 停止地面塌陷效果
    /// </summary>
    public void StopTileDown()
    {
        StopCoroutine("TileDown");
    }

    /// <summary>
    /// 地面塌陷
    /// </summary>
    /// <returns></returns>
    private IEnumerator TileDown()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            for (int i = 0; i < mapList[index].Length; i++)//每一行
            {
                Rigidbody rb= mapList[index][i].AddComponent<Rigidbody>();//给每行的元素添加刚体组件
                rb.angularVelocity = new Vector3(Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f)) * Random.Range(1.0f, 10.0f);//给掉落的物体增加旋转
                GameObject.Destroy(mapList[index][i],1.0f);//给每个元素添加定时销毁功能
            }
            if (index == m_PlayerCotroller.x)//塌陷行数与角色行数相等，停止塌陷
            {
                StopTileDown();
                m_PlayerCotroller.gameObject.AddComponent<Rigidbody>();
                m_PlayerCotroller.StartCoroutine("GameOver",true);


            }
            index++;
          
        }
    }

    /// <summary>
    /// 计算概率
    ///0.瓷砖
    ///1.坑洞
    ///2.地面陷阱
    ///3.天空陷阱
    private int CalcPR()
    {
        int pr = Random.Range(1, 100);//没有从0开始是为了避免第一段地图出现坑洞
        if (pr<=pr_hole)
        {
            return 1;
        }
        else if (31<pr&&pr<pr_spikes+30)
        {
            return 2;
        }
        else if (61 < pr && pr < pr_spikes + 60)
        {
            return 3;
        }
        return 0;
    }

    /// <summary>
    /// 计算宝石生成概率
    /// </summary>
    /// <returns>0:不生成 1:生成</returns>
    private int CalcGemPR()
    {
        int pr = Random.Range(0,100);
        if (pr<pr_gem)
        {
            return 1;
        }
        return 0;
    }
    /// <summary>
    /// 增加概率
    /// </summary>
    public void AddPR()
    {
        pr_hole += 2;
        pr_spikes += 2;
        pr_sky_spikes += 2;
    }

    public void ResetGameMap()//重置地图
    { 
     Transform[] sonTransform =m_Transform.GetComponentsInChildren<Transform>();
     for (int i = 1; i < sonTransform.Length; i++)
     {
         GameObject.Destroy(sonTransform[i].gameObject);
     } //重置陷阱概率
         pr_hole = 0; //路障概率
         pr_spikes = 0;//地面陷阱概率
         pr_sky_spikes = 0;//天空陷阱概率
         pr_gem = 2;//宝石概率
        //重置塌陷角标
         index = 0;//当前塌陷的行数
        //清空地图
         mapList.Clear();
        //重新生成地图
         CreatMapItem(0);
    }

}
