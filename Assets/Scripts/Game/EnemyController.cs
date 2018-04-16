using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Lean.Pool;

public class EnemyController : MonoBehaviour
{
    [Tooltip("怪物血量")] [SerializeField] private float healthPoint = 5f;
    [Tooltip("伤害值")] [SerializeField] private int damage = 1;
    [Tooltip("攻击音效")] [SerializeField] private AudioClip attackAC;
    [Tooltip("死亡音效")] [SerializeField] private AudioClip deadAC;
    [Tooltip("平常音效")] [SerializeField] private AudioClip walkAC;
    private NavMeshAgent agent;    //寻路组件
    private GameObject player;//玩家
    private Animator ani;    //动画控制器
    void OnEnable()
    {
        agent = this.GetComponent<NavMeshAgent>();//获取寻路组件，如果为空则添加之
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.radius = 0.3f;
        }
        agent.enabled = true;
    }
    private void Start()
    {
      player = GameObject.FindGameObjectWithTag("Player");    //获取玩家
      ani = this.GetComponent<Animator>();                    //获取动画组件 
    }
    //void OnDisable()
    //{
    //    agent.enabled = false;
    //    GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
    //}


    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="ac">声音</param>
    void PlaySound(AudioClip ac)
    {
        AudioSource Ac = this.GetComponent<AudioSource>();//获取声源，如果为空则添加之
        if (Ac == null)
        {
            Ac = this.gameObject.AddComponent<AudioSource>();
        }
        Ac.clip = ac;
        Ac.Play();
    }

    /// <summary>
    /// 减血
    /// </summary>
    /// <param name="_damageAmount">伤害值</param>
    void ApplyDamage(float _damageAmount)
    {
        float t = healthPoint - _damageAmount;
        if (t > 0)//如果健康值不低于0则造成伤害
        {
            healthPoint = t;

        }
        else //反之则切换到死亡，并且震动下手柄提醒玩家已经消灭敌人
        {
            healthPoint = 0;
            SwitchDead();
            HitHapticPulse(1000);
        }
    }
    /// <summary>
    /// 震动
    /// </summary>
    /// <param name="duration">震动时间.</param>
    void HitHapticPulse(ushort duration)
    {
        var deviceIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
        var deviceIndex1 = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
        SteamVR_Controller.Input(deviceIndex).TriggerHapticPulse(duration);
        SteamVR_Controller.Input(deviceIndex1).TriggerHapticPulse(duration);
    }
    /// <summary>
    /// 切换死亡
    /// </summary>
    void SwitchDead()
    {
        ani.SetBool("isDead", true);
        agent.enabled = false;//停止寻路
        this.GetComponent<CapsuleCollider>().enabled = false;  //关掉碰撞器
        PlaySound(deadAC);
    }

    /// <summary>
    /// 切换攻击
    /// </summary>
    /// <param name="value">布尔值</param>
    void SwitchAttack(bool value)
    {
        ani.SetBool("isAttack", value);
        PlaySound(attackAC);
    }
    void Update()
    {
        if (healthPoint <= 0)//僵尸如果健康值小于=0时判断死亡动画是否播放完毕
        {
            AnimatorStateInfo asi = this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            //if (asi.normalizedTime > 1f && asi.IsName("back_fall"))
            //{
            //    LeanPool.Despawn(this.gameObject);//用对象池来回收之，可以节省性能开销
            //}
        }
        if (agent.enabled=true && healthPoint > 0)//只有僵尸活着就会来寻找玩家，找到玩家则攻击之
        {
            agent.destination = player.transform.position;
            if (Vector3.Distance(this.transform.position, player.transform.position) < 5f)
            {
                SwitchAttack(true);
                agent.isStopped=true;
            }
            else
            {
                SwitchAttack(false);
                PlaySound(walkAC);
                agent.isStopped = false;
                
            }
        }
    }
}
