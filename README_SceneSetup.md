# 电网塔防系统 - 场景设置指南

## 快速开始

### 1. 场景必需物体设置

在 Unity 场景中创建以下空物体并命名（用于自动查找）：

#### 敌人生成点
- 创建空物体，命名为 `SpawnPoint`
- 放置在地图边缘或敌人出现的位置

#### 敌人目标点（终点）
- 创建空物体，命名为 `EndTarget`
- 放置在基地/能源核心位置

#### 能源核心（优先攻击目标）
- 创建空物体，命名为 `EnergyCore`
- 放置在基地中心位置

### 2. 路径点设置

#### 创建路径
1. 创建空物体命名为 `Waypoint1`
2. 复制多个，按顺序排列为 `Waypoint1`, `Waypoint2`, `Waypoint3`...
3. 形成敌人行进路线

#### 关联路径点
- 选中任意一个路径点物体
- 添加 `Waypoint` 组件（脚本会自动识别所有同名物体）

### 3. 地形设置

#### 创建地形
1. GameObject > 3D Object > Terrain
2. 调整地形大小和高度

#### 设置地形层
1. Edit > Project Settings > Layers
2. 将 Layer 8 命名为 "Ground"
3. 选中地形物体，设置 Layer 为 "Ground"

#### 导航网格（NavMesh）
1. Window > AI > Navigation
2. 选中地形，勾选 Navigation Static
3. 点击 Bake 生成导航网格

### 4. UI 设置（可选但推荐）

#### 创建 Canvas
1. GameObject > UI > Canvas
2. 设置 Canvas Scaler 为 Scale With Screen Size

#### 添加 UI 元素
在 Canvas 下创建以下 TextMeshProUGUI 元素：

**资源显示区：**
- `EnergyText` - 显示能源数量
- `MaterialText` - 显示物资数量

**波次信息区：**
- `WaveText` - 显示当前波次
- `CountdownText` - 显示倒计时
- `KillCountText` - 显示击杀数
- `HealthText` - 显示生命值

**建造信息区：**
- `SelectedTowerName` - 显示选中塔名称
- `SelectedTowerCost` - 显示塔成本

**游戏面板：**
- `StartPanel` - 开始界面面板
- `GameOverPanel` - 游戏结束面板
- `PausePanel` - 暂停面板
- `WarningPanel` - 警告提示面板

#### 关联 UI 引用
- 找到场景中的 `GameManager` 物体
- 将上述 UI 元素拖拽到 UIManager 组件对应槽位

### 5. 塔预制体设置

#### 创建塔预制体
1. 创建 4 种塔的游戏物体（基础塔、激光塔、加农炮、导弹塔）
2. 每种塔需要：
   - 3D 模型或简单立方体
   - 对应的塔脚本（BaseTower 子类）
   - Tag 设置为 "Tower"
3. 拖入 Project 窗口创建预制体

#### 关联塔预制体
- 选中 `BuildManager` 物体
- 在 Inspector 中将 4 种塔预制体拖入对应槽位

### 6. 敌人预制体设置

#### 创建敌人
1. 创建敌人游戏物体（胶囊体或自定义模型）
2. 添加组件：
   - `BaseEnemy` 脚本
   - NavMeshAgent（用于寻路）
   - Collider（碰撞体）
3. 创建预制体

#### 关联敌人预制体
- 选中 `WaveSpawner` 物体
- 将敌人预制体拖入 Enemy Prefab 槽位

### 7. 管理器设置

确保场景中有以下管理器物体：

| 管理器 | 必需组件 | 说明 |
|--------|---------|------|
| GameManager | GameManager | 游戏状态控制 |
| ResourceManager | ResourceManager | 资源管理 |
| BuildManager | BuildManager | 建造系统 |
| WaveSpawner | WaveSpawner | 敌人生成 |
| UIManager | UIManager | UI 控制 |
| TerrainModifier | TerrainModifier | 地形修改 |

可以创建一个空物体命名为 `GameManager"，将所有管理器脚本挂载到同一个物体上。

## 操作说明

### 建造系统
- **1/2/3/8** - 选择塔类型（基础/激光/加农炮/导弹）
- **鼠标左键** - 建造塔
- **ESC** - 取消建造

### 地形工具
- **4** - 挖掘模式（降低地形）
- **5** - 抬高模式
- **6** - 平整模式
- **7** - 平滑模式
- **鼠标左键** - 使用工具

### 游戏控制
- **P** - 暂停游戏
- **R** - 重新开始（游戏结束后）
- **T** - 触发测试波次（调试用）

## 常见问题

### 敌人不移动
1. 检查是否生成了 NavMesh（Window > AI > Navigation > Bake）
2. 确认敌人有 NavMeshAgent 组件
3. 检查路径点是否正确设置

### 无法建造
1. 检查塔预制体是否已赋值
2. 确认 "Tower" Tag 已创建
3. 检查资源是否充足

### UI 不显示
1. 确认 Canvas 存在且正确设置
2. 检查 TextMeshProUGUI 组件是否安装
3. 确认 UI 引用已正确关联

## 测试流程

1. 运行场景
2. 点击开始按钮
3. 等待 10 秒准备时间
4. 观察敌人是否从 SpawnPoint 生成
5. 观察敌人是否沿路径点移动
6. 建造塔攻击敌人
7. 使用地形工具修改地形

祝游戏开发顺利！
