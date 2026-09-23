using System.Collections.Generic;
using HutongGames.PlayMaker;
using Modding;
using UnityEngine;

namespace NoNailRain
{
    // 独立 mod：无上辐光（神居 GG_Radiance）P1 被击倒的瞬间，
    // 清掉残留在场上的飞行剑（无限剑雨 `Rage Comb` 打出的那批）。
    //
    // 实现方式是**状态轮询**，不往 FSM 注入任何 action：
    // 本机同时加载着 ManyRadiances 等按动作索引读取辐光 FSM 的 mod，
    // 往状态里加动作会改变动作数量、把它们读错。
    public class NoNailRain : Mod
    {
        internal static NoNailRain Instance;

        // 同一 FSM 只挂一次（同对象重复 Enable 跳过；新挑战场景重载会生成新的 FSM 实例，自动重新挂）
        private static readonly HashSet<PlayMakerFSM> Attached = new HashSet<PlayMakerFSM>();

        public override string GetVersion() => "1.0.0";

        public NoNailRain() : base("ClearPhaseEndSword")
        {
            Instance = this;
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("NoNailRain initializing");
            On.PlayMakerFSM.OnEnable += OnFsmEnable;
        }

        // 不按场景名门控：`Absolute Radiance` + `Control` 这个组合只属于辐光，
        // 且 OnEnable 触发时新场景可能还不是 active scene，用场景名判断时机不可靠。
        private void OnFsmEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM fsm)
        {
            orig(fsm);

            if (fsm.gameObject.name != "Absolute Radiance") return;
            if (fsm.FsmName != "Control") return;
            if (!Attached.Add(fsm)) return;

            NailCleaner cleaner = fsm.gameObject.AddComponent<NailCleaner>();
            cleaner.fsm = fsm;
            Log("NoNailRain: attached to Control FSM");
        }
    }
}
