# 测试配置文件
# 用于验证项目设置和功能

extends Node

func _ready():
    print("=== 桌面羽毛球游戏测试 ===")
    print("正在初始化测试环境...")
    
    # 测试窗口设置
    test_window_settings()
    
    # 测试物理参数
    test_physics_settings()
    
    # 测试输入系统
    test_input_system()
    
    print("测试完成！")

func test_window_settings():
    print("\n--- 窗口设置测试 ---")
    var window = get_tree().root
    
    print("透明窗口: ", window.transparent)
    print("无边框: ", window.borderless)
    print("置顶: ", window.always_on_top)
    print("鼠标穿透: ", window.mouse_passthrough)
    
    # 验证窗口大小
    var screen_size = DisplayServer.screen_get_size()
    var window_size = window.size
    print("屏幕大小: ", screen_size)
    print("窗口大小: ", window_size)
    
    if window_size == screen_size:
        print("✓ 窗口大小设置正确")
    else:
        print("✗ 窗口大小设置错误")

func test_physics_settings():
    print("\n--- 物理设置测试 ---")
    
    # 测试重力设置
    var gravity = ProjectSettings.get_setting("physics/2d/default_gravity")
    print("重力加速度: ", gravity)
    
    # 测试空气阻力
    var linear_damp = ProjectSettings.get_setting("physics/2d/default_linear_damp")
    print("线性阻尼: ", linear_damp)
    
    # 测试角阻尼
    var angular_damp = ProjectSettings.get_setting("physics/2d/default_angular_damp")
    print("角阻尼: ", angular_damp)

func test_input_system():
    print("\n--- 输入系统测试 ---")
    
    # 测试鼠标输入
    if Input.is_mouse_button_pressed(MOUSE_BUTTON_LEFT):
        print("鼠标左键被按下")
    else:
        print("鼠标左键未按下")
    
    # 测试键盘输入
    if Input.is_key_pressed(KEY_R):
        print("R键被按下")
    else:
        print("R键未按下")
    
    if Input.is_key_pressed(KEY_ESCAPE):
        print("ESC键被按下")
    else:
        print("ESC键未按下")

func _process(delta):
    # 实时测试鼠标位置
    var mouse_pos = get_global_mouse_position()
    # print("鼠标位置: ", mouse_pos)  # 注释掉避免过多输出