# NoNailRain

无上辐光（神居 GG_Radiance）P1 被击倒的瞬间，清掉残留在场上的飞行剑。

无限剑雨（Control FSM `Rage Comb`）打出的剑在 Boss 转入倒地时不会自行消失，
会一直留在战场上。本 mod 在 Boss 倒地那一刻把场上所有名字含 `Radiant Nail`
的对象（`Radiant Nail` / `Radiant Nail Comb` / `(Clone)`）一并销毁。

## 触发点

Control FSM 进入 `Stun1 Start` —— 实测状态链（P1 打倒那一次，带当时血量）：

```
Rage1 Tele → Rage1 Antic → Rage1 Start → Rage Comb (hp 3432)
→ Stun1 Start (hp 2824) → Tendrils1 (hp 2824)
```

`Stun1` 是这一战的第一次倒地，紧接无限剑雨 `Rage Comb` 之后。

## 实现说明

用 **状态轮询**（每帧读 `ActiveStateName`）而不是往 FSM 注入 action。
往状态里加动作会改变该 FSM 的动作数量，破坏 `ManyRadiances` 等按动作索引
读取辐光 FSM 的 mod。轮询的代价只有 1 帧（约 16ms）延迟。

## 构建

```
dotnet build -c Release
```

产物自动拷到 `E:/game/Hollow Knight 1578/hollow_knight_Data/Managed/Mods/NoNailRain/`。
