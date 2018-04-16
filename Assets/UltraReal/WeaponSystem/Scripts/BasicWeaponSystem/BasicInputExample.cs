using UnityEngine;
using System.Collections;
using UltraReal.Utilities;
using UltraReal.WeaponSystem;
/// <summary> 
/// 这只是一个简单的Htc Vive的输入案例
/// </summary>
public class BasicInputExample : UltraRealMonobehaviorBase {
    SteamVR_TrackedObject trackedObj; //手柄
    RaycastHit hit; //射线检测结果
    private UltraRealLauncherBase launcher;//之前解析的BasicLauncher组件
    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();//获取追踪组件
        SteamVR_TrackedController trackController = GetComponent<SteamVR_TrackedController>();//获取控制器组件
    }
    /// <summary> 
    /// Finds the launcher script.找到Launcher脚本组件
    /// </summary>
    protected override void OnStart()
    {
        base.OnStart();
        launcher = GetComponent<UltraRealLauncherBase>();
    }
    /// <summary> 
    /// Tests to see if the player is pressing the fire button, or the reload button.  Activateds methods on launcher accordingly.
    /// 测试下玩家有没有按下扳机开火，或者换子弹Grip手柄按钮，在Launcher上面激活对应的方法
    /// </summary>
    protected override void OnUpdate()
    {
        base.OnUpdate();

        var device = SteamVR_Controller.Input((int)trackedObj.index);//获取控制器的索引
        if (device.GetPress(SteamVR_Controller.ButtonMask.Grip) && launcher != null)//如果是Grip手柄按钮，则换子弹
        {
            launcher.Reload();
        }
        if (device.GetTouch(SteamVR_Controller.ButtonMask.Trigger) && launcher != null)//如果是Trigger扳机按钮则开火
        {
            launcher.Fire();
        }

    }
}
