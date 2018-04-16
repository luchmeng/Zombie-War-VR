using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
/// <summary>
/// 怪物生成控制器
/// </summary>
public class EnemySpawnController : MonoBehaviour {
	[Tooltip("产怪的等级")][SerializeField]private string _level="1";
	[Tooltip("产怪的总量")][SerializeField]private int enemyCount = 25;
	[Tooltip("产怪的间隔时间")][SerializeField]private float interval = 2f;
	private float timeCount =0;//计时                        
    private Transform[] SpawnPoints;//出生点
    private int createobj = 0;//生成僵尸的总数，达到enemycount后不再有新的僵尸出现
    private List<GameObject> Enemy =new List<GameObject>();//怪物列表
    void Start () {
		timeCount = 1f;
        createobj = 1;
        Transform SpawnPoint = gameObject.transform.Find("SpawnPoints");//寻找出生点
        //GameObject SpawnPoint = GameObject.Find("ZombieSpawner");
        SpawnPoints = SpawnPoint.GetComponentsInChildren<Transform> ();//获取其位置
        StartCoroutine(Loaditem ("Monster/Level"+_level));//加载怪物
	}
	IEnumerator Loaditem(string path)
	{
		Object[] obj= Resources.LoadAll(path,typeof(GameObject));//开始加载游戏对象
        for (int i = 0; i < obj.Length; i++) {
			Enemy.Add((GameObject)obj [i]); 
			yield return 0;
		}
		yield return 0;//结束
    }
    void Update()
    {
        if (timeCount > 0)
        {//如果倒计时结束则生成一个僵尸并且倒计时重置
            timeCount -= Time.deltaTime;
        }
        else
        {
            int liveEnemyCount = transform.childCount - 1;//当前生存的僵尸数量
            if (liveEnemyCount < enemyCount && createobj < enemyCount)
            { //如果当前的僵尸数量小于规定的数量，则随机生成僵尸
                LeanPool.Spawn(Enemy[Random.Range(0, Enemy.Count - 1)], SpawnPoints[Random.Range(0, SpawnPoints.Length - 1)].position, Quaternion.identity, transform);
                createobj++;
                Debug.Log(createobj);
            }
            timeCount = interval;
        }
    }      
		
}
