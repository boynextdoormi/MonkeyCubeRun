using UnityEngine;
using System.Collections;

/// <summary>
/// ui管理器
/// </summary>
public class UIManager : MonoBehaviour {
    private GameObject m_StartUI;
    private GameObject m_GameUI;

    private UILabel m_Score_Lable;
    private UILabel m_Gem_Lable;
    private UILabel m_GameScore_Lable;
    private UILabel m_GameGem_Lable;

    private GameObject PlayButton;
    private GameObject m_Left;
    private GameObject m_Right;


    private PlayerCotroller m_PlayerController;

	void Start () {
        m_StartUI = GameObject.Find("Start_UI");
        m_GameUI = GameObject.Find("Game_UI");

        m_PlayerController =GameObject.Find("cube_books").GetComponent <PlayerCotroller>();


        m_Score_Lable = GameObject.Find("Score_Lable").GetComponent<UILabel>();
        m_Gem_Lable = GameObject.Find("Gem_Lable").GetComponent<UILabel>();
        m_GameScore_Lable = GameObject.Find("GameScore_Lable").GetComponent<UILabel>();
        m_GameGem_Lable = GameObject.Find("GameGem_Lable").GetComponent<UILabel>();

        PlayButton = GameObject.Find("play_btn");
        m_Left = GameObject.Find("Left");
        m_Right = GameObject.Find("Right");
        //开始委托事件
        UIEventListener.Get(PlayButton).onClick = PlayButtonClick;
        UIEventListener.Get(m_Left).onClick = Left;
        UIEventListener.Get(m_Right).onClick = Right;
        Init();

        m_StartUI.SetActive(true);
        m_GameUI.SetActive(false);
	}

    private void Init()
    {
        //StartUI
        m_Score_Lable.text = PlayerPrefs.GetInt("score",0)+"";
        m_Gem_Lable.text = PlayerPrefs.GetInt("gem", 0) + "/100";
        //GameUI
        m_GameScore_Lable.text = "0";
        m_GameGem_Lable.text = PlayerPrefs.GetInt("gem", 0) + "/100";
    }

    public void UpdateDate(int score,int gem)//游戏界面分数更新
    {
        m_Gem_Lable.text = gem + "/100";
        m_GameScore_Lable.text = score.ToString();
        m_GameGem_Lable.text = gem + "/100";
    }

    private void PlayButtonClick(GameObject go)//点击重开按钮
    {
        Debug.Log("游戏开始啦");
        m_StartUI.SetActive(false);
        m_GameUI.SetActive(true);
        m_PlayerController.StartGame();
    }

    private void Left(GameObject go)//点击左侧按钮
    {
        m_PlayerController.Left();
    }

    private void Right(GameObject go)//点击右侧按钮
    {
        m_PlayerController.Right();
    }

    public void ResetUI()//重置UI
    {
        m_StartUI.SetActive(true);
        m_GameUI.SetActive(false);
        m_GameScore_Lable.text = "0";
    }
}
