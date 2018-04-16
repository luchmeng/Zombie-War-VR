using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.AI;
using Lean.Pool;

[RequireComponent(typeof(Animation))]
public class ZombieAnimation : MonoBehaviour {
    [Tooltip("怪物血量")][SerializeField] private float healthPoint = 5f;
    [Tooltip("伤害值")][SerializeField] private int damage = 1;
    [Tooltip("攻击音效")][SerializeField] private AudioClip attackAC;
    [Tooltip("死亡音效")][SerializeField] private AudioClip deadAC;
    [Tooltip("平常音效")][SerializeField] private AudioClip walkAC;
    private NavMeshAgent agent;    //寻路组件
    private GameObject player;//玩家
    private Animation ani;    //动画控制器
    private float duration = 2.2f;//死亡动画持续时间
    private float curTime = 0;
    private float breakForce = 150f;
    void OnEnable () {
        agent = this.GetComponent<NavMeshAgent>();//获取寻路组件，如果为空则添加之
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.radius = 0.3f;
        }
        agent.enabled = true;
        player = GameObject.FindGameObjectWithTag("Player");//获取玩家
        ani = this.GetComponent<Animation>();
        ani.wrapMode = WrapMode.Loop;
        ani["die"].wrapMode = WrapMode.Once;
    }


    // Update is called once per frame
    void Update () {
        if (healthPoint <= 0)//僵尸如果健康值小于=0时判断死亡动画是否播放完毕
        {
            curTime += Time.deltaTime;
            if (curTime > duration)
            {
                curTime = 0;
                LeanPool.Despawn(this.gameObject);//用对象池来回收之，可以节省性能开销
            }
        }
        if (agent.enabled && healthPoint > 0)//只有僵尸活着就会来寻找玩家，找到玩家则攻击之
        {
            agent.destination = player.transform.position;
            if (Vector3.Distance(this.transform.position, player.transform.position) < 2.5f)
            {
                SwitchAttack();
                agent.Stop();
            }
            else
            {
                SwitchWalk();
                agent.Resume();
            }
        }
    }
	void PlaySound(AudioClip ac){
		AudioSource Ac = this.GetComponent<AudioSource> ();
		if (Ac == null) {
			Ac=this.gameObject.AddComponent<AudioSource> ();
		}
		Ac.clip = ac;
		Ac.Play ();

	}
	/// <summary>
	/// 减血
	/// </summary>
	/// <param name="_damageAmount">Damage amount.</param>
	void ApplyDamage(float _damageAmount){
		float t = healthPoint - _damageAmount;
		//Debug.Log (_damageAmount);
		if (t > 0)
			healthPoint = t;
		else {
			healthPoint = 0;
			SwitchDead ();
			HitHapticPulse (2000);
		}
    }
	/// <summary>
	/// 震动
	/// </summary>
	/// <param name="duration">震动时间.</param>
	void HitHapticPulse(ushort duration){
		var deviceIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
		var deviceIndex1 = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
		SteamVR_Controller.Input (deviceIndex).TriggerHapticPulse (duration);
		SteamVR_Controller.Input (deviceIndex1).TriggerHapticPulse (duration);
	}
	void SwitchAttack(){
		ani.CrossFade ("attack2");
        PlaySound(attackAC);
    }
    void SwitchWalk()
    {
        ani.CrossFade("walk1");
        PlaySound(walkAC);
    }
	void SwitchDead(){
		ani.CrossFade ("die");
		this.GetComponent<CapsuleCollider>().enabled =false;//关掉碰撞器
        agent.enabled = false;
		PlaySound (deadAC);
    }
}
