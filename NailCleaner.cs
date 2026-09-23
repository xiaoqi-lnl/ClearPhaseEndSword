using System;
using HutongGames.PlayMaker;
using UnityEngine;

namespace NoNailRain
{
    // 辐光 P1 倒地瞬间的清场逻辑。
    //
    // 触发点：Control FSM 进入 "Stun1 Start"。Stun1 是这一战的第一次倒地，
    // 紧接无限剑雨 `Rage Comb` 之后。实测状态链（P1 打倒那一次，带当时血量）：
    //   Rage1 Tele → Rage1 Antic → Rage1 Start → Rage Comb (hp 3432)
    //   → Stun1 Start (hp 2824) → Tendrils1 (hp 2824)
    //
    // 只轮询 ActiveStateName、不注入 action —— 保持 FSM 动作数量不变，
    // 不干扰 ManyRadiances 等按索引读取该 FSM 的 mod。
    public class NailCleaner : MonoBehaviour
    {
        public PlayMakerFSM fsm;

        private const string KnockDownState = "Stun1 Start";
        private const string NailNameToken = "Radiant Nail";   // 同时覆盖 Radiant Nail / Radiant Nail Comb / (Clone)

        private bool wasInState;

        private void Update()
        {
            if (fsm == null || fsm.Fsm == null) return;

            bool nowInState = fsm.Fsm.ActiveStateName == KnockDownState;
            if (nowInState && !wasInState) ClearNails();
            wasInState = nowInState;
        }

        private void ClearNails()
        {
            GameObject[] all = FindObjectsOfType<GameObject>();
            int cleared = 0;
            for (int i = 0; i < all.Length; i++)
            {
                GameObject go = all[i];
                if (go == null) continue;
                if (go.name.IndexOf(NailNameToken, StringComparison.Ordinal) < 0) continue;
                Destroy(go);
                cleared++;
            }
            // 临时的验证日志：确认清了几把、有没有清到。实测通过后删掉。
            NoNailRain.Instance.Log("P1 knock down: cleared " + cleared + " residual nails");
        }
    }
}
