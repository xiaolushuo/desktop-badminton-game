# 桌面羽毛球游戏

这是一个基于Godot引擎开发的桌面羽毛球游戏，可以在Windows桌面上投掷羽毛球，具有真实的物理效果。

## 功能特性

### 🎮 核心功能
- **透明无边框窗口**：100%透明背景，不干扰正常桌面使用
- **置顶显示**：始终显示在桌面最上层
- **鼠标穿透**：空白区域鼠标可穿透，不影响正常操作
- **物理模拟**：真实的重力、空气阻力和弹跳效果
- **拖拽投掷**：鼠标拖拽羽毛球进行投掷
- **轨迹显示**：显示羽毛球飞行轨迹
- **碰撞检测**：与桌面边界的碰撞反弹
- **游戏模式**：简单模式（轨迹预测）和困难模式（无辅助）

### 🎯 游戏模式

#### 🟢 简单模式（Easy Mode）
- **轨迹预测**：拖拽时显示预测的飞行路径
- **视觉辅助**：黄色轨迹点显示投掷方向和力度
- **适合新手**：帮助玩家掌握投掷技巧
- **学习工具**：理解物理规律和轨迹计算

#### 🔴 困难模式（Hard Mode）
- **无轨迹辅助**：不显示任何预测信息
- **真实挑战**：完全依靠玩家的感觉和经验
- **高手模式**：提供更具挑战性的游戏体验
- **成就感强**：成功投掷需要更高的技巧

### 🎯 游戏机制
- **抓取**：点击羽毛球并拖拽
- **投掷**：释放鼠标进行投掷，力度和方向基于拖拽速度
- **物理效果**：
  - 重力加速度：980像素/秒²
  - 空气阻力：0.99（轻微阻力）
  - 弹跳衰减：0.7（碰撞时能量损失）
  - 旋转效果：基于飞行速度的自动旋转
- **轨迹预测**：简单模式下实时显示预测飞行路径

### 🎨 视觉效果
- **羽毛球外观**：逼真的羽毛球纹理
- **飞行轨迹**：半透明轨迹线显示飞行路径
- **轨迹预测**：简单模式下的黄色预测点
- **碰撞效果**：碰撞时的粒子效果
- **悬停高亮**：鼠标悬停时的高亮效果
- **模式UI**：简洁的模式切换界面

## 系统要求

- **操作系统**：Windows 10/11
- **Godot版本**：4.3+
- **.NET支持**：C#脚本支持
- **显卡**：支持透明窗口的显卡

## 安装和使用

### 1. 环境准备
```bash
# 安装Godot 4.3+
# 下载地址：https://godotengine.org/
```

### 2. 项目设置
1. 打开Godot编辑器
2. 导入项目文件夹 `desktop_badminton_game`
3. 确保启用了C#支持
4. 在项目设置中配置窗口属性

### 3. 资源准备
需要准备以下图片资源：
- `assets/shuttlecock_body.png` - 羽毛球主体纹理
- `assets/shuttlecock_feathers.png` - 羽毛部分纹理  
- `assets/trail.png` - 轨迹效果纹理
- `assets/icon.png` - 应用图标

### 4. 运行游戏
1. 在Godot编辑器中打开 `Main.tscn` 场景
2. 点击运行按钮（F5）
3. 游戏将以透明窗口模式启动

## 操作说明

### 基本操作
- **抓取羽毛球**：鼠标左键点击羽毛球
- **拖拽**：按住左键拖拽到想要的位置
- **投掷**：释放鼠标左键进行投掷
- **重置位置**：按 R 键重置羽毛球到屏幕中心
- **退出游戏**：按 ESC 键退出

### 模式操作
- **切换模式UI**：按 M 键显示/隐藏模式切换界面
- **快速切换模式**：按 Tab 键在简单/困难模式间切换
- **按钮切换**：点击UI中的"切换模式"按钮

### 投掷技巧
- **力度控制**：拖拽距离越远，投掷力度越大
- **方向控制**：拖拽方向决定投掷方向
- **最大距离**：拖拽距离限制在200像素内
- **投掷倍数**：实际投掷力 = 拖拽速度 × 3倍

### 简单模式技巧
- **观察轨迹**：拖拽时注意黄色轨迹点的分布
- **预测落点**：轨迹点密集处表示羽毛球会经过该区域
- **调整力度**：根据轨迹长度调整拖拽距离
- **学习物理**：通过轨迹预测理解重力和空气阻力的影响

### 物理参数调节
在 `Main.cs` 中可以调节以下参数：
```csharp
[Export] public float MaxDragDistance = 200.0f;    // 最大拖拽距离
[Export] public float ThrowMultiplier = 3.0f;       // 投掷力度倍数
[Export] public float Gravity = 980.0f;            // 重力加速度
[Export] public float AirResistance = 0.99f;       // 空气阻力系数
[Export] public float BounceDamping = 0.7f;         // 弹跳衰减系数
[Export] public bool StartInEasyMode = false;      // 是否以简单模式开始
```

### 轨迹预测参数
在 `TrajectoryPredictor.cs` 中可以调节轨迹显示：
```csharp
[Export] public int MaxPoints = 30;                  // 最大轨迹点数
[Export] public float PointSpacing = 15.0f;          // 轨迹点间距
[Export] public float PredictionTime = 2.0f;         // 预测时间（秒）
[Export] public Color TrajectoryColor = new Color(1, 1, 0, 0.6f); // 轨迹颜色
```

## 项目结构

```
desktop_badminton_game/
├── project.godot              # Godot项目配置
├── README.md                  # 项目说明
├── src/                       # 源代码目录
│   ├── Main.cs               # 主场景脚本
│   ├── Shuttlecock.cs        # 羽毛球脚本
│   ├── GameMode.cs           # 游戏模式管理
│   ├── TrajectoryPredictor.cs # 轨迹预测器
│   └── ModeUI.cs             # 模式切换UI
├── scenes/                    # 场景文件
│   └── Main.tscn             # 主场景
├── objects/                   # 游戏对象
│   ├── Shuttlecock.tscn      # 羽毛球对象
│   └── TrajectoryPoint.tscn  # 轨迹点对象
└── assets/                    # 资源文件
    ├── icon.png              # 应用图标
    ├── shuttlecock_body.png  # 羽毛球主体
    ├── shuttlecock_feathers.png # 羽毛部分
    ├── trail.png             # 轨迹纹理
    └── trajectory_point.png   # 轨迹点纹理
```

## 技术实现

### 窗口透明化
```csharp
private void SetupWindow()
{
    var window = GetTree().Root;
    window.Transparent = true;           // 透明窗口
    window.Borderless = true;           // 无边框
    window.AlwaysOnTop = true;          // 置顶显示
    window.MousePassthrough = true;     // 鼠标穿透
}
```

### 物理引擎
- **刚体物理**：使用 `RigidBody2D` 实现真实的物理模拟
- **碰撞检测**：多重碰撞形状（圆形、胶囊形、矩形）
- **力的应用**：通过 `ApplyCentralImpulse` 应用投掷力
- **空气阻力**：每帧应用阻力模拟能量损失

### 轨迹预测系统
```csharp
// 简单模式下的轨迹预测
private void UpdateDragging(Vector2 mousePos)
{
    // ... 拖拽逻辑
    
    // 在简单模式下显示轨迹预测
    if (GameMode.IsEasyMode())
    {
        var predictedVelocity = _dragVelocity * ThrowMultiplier;
        _trajectoryPredictor.UpdateTrajectory(targetPos, predictedVelocity);
    }
}
```

### 游戏模式管理
```csharp
// 游戏模式切换
public static void ToggleDifficulty()
{
    CurrentDifficulty = CurrentDifficulty == GameDifficulty.Easy ? 
        GameDifficulty.Hard : GameDifficulty.Easy;
}

// 轨迹预测控制
public void ShowTrajectory(Vector2 startPosition, Vector2 initialVelocity)
{
    if (!GameMode.IsEasyMode())
    {
        HideTrajectory();
        return;
    }
    // 显示轨迹预测
}
```

### 鼠标交互
- **区域检测**：使用 `Area2D` 检测鼠标悬停
- **拖拽控制**：限制拖拽距离，计算投掷力度
- **鼠标穿透**：非交互区域允许鼠标穿透
- **模式切换**：专用的UI界面进行模式控制

## 故障排除

### 常见问题

**Q: 窗口不透明**
- A: 检查显卡是否支持透明窗口
- A: 在项目设置中启用透明选项

**Q: 鼠标无法穿透**
- A: 确保背景区域的 `mouse_filter` 设置为 2
- A: 检查碰撞区域是否正确设置

**Q: 物理效果不真实**
- A: 调整重力、空气阻力等物理参数
- A: 检查碰撞形状是否合理

**Q: 性能问题**
- A: 减少轨迹点数量
- A: 禁用不必要的视觉效果

## 扩展功能

### 可添加的功能
1. **多个羽毛球**：支持同时投掷多个羽毛球
2. **音效系统**：添加投掷、碰撞音效
3. **粒子效果**：更丰富的视觉效果
4. **计分系统**：记录投掷距离、次数等
5. **设置界面**：调节物理参数、视觉效果
6. **多屏幕支持**：支持多显示器环境

### 性能优化
1. **对象池**：重用羽毛球对象减少内存分配
2. **LOD系统**：根据距离调整细节级别
3. **异步加载**：异步加载资源减少卡顿

## 许可证

本项目基于MIT许可证开源。

## 贡献

欢迎提交Issue和Pull Request来改进这个项目！

## 联系方式

如有问题或建议，请通过以下方式联系：
- GitHub Issues
- Email: [your-email@example.com]