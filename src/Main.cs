using Godot;
using System;

namespace DesktopBadmintonGame;

public partial class Main : Node2D
{
    private PackedScene _shuttlecockScene;
    private Shuttlecock _currentShuttlecock;
    private bool _isDragging = false;
    private Vector2 _dragOffset;
    private Vector2 _dragStartPos;
    private Vector2 _dragVelocity;
    private double _dragStartTime;

    [Export] public float MaxDragDistance = 200.0f;
    [Export] public float ThrowMultiplier = 3.0f;
    [Export] public float Gravity = 980.0f;
    [Export] public float AirResistance = 0.99f;
    [Export] public float BounceDamping = 0.7f;

    public override void _Ready()
    {
        base._Ready();
        
        // 设置窗口属性
        SetupWindow();
        
        // 加载羽毛球场景
        _shuttlecockScene = ResourceLoader.Load<PackedScene>("res://objects/Shuttlecock.tscn");
        
        // 创建第一个羽毛球
        SpawnShuttlecock();
        
        // 连接输入信号
        Input.Singleton.Connect("gui_input", new Callable(this, nameof(OnGuiInput)));
    }

    private void SetupWindow()
    {
        var window = GetTree().Root;
        window.Transparent = true;
        window.Borderless = true;
        window.AlwaysOnTop = true;
        window.MousePassthrough = true;
        
        // 设置窗口大小为屏幕大小
        var screen = DisplayServer.ScreenGetSize();
        window.Size = screen;
        window.Position = Vector2I.Zero;
        
        // 设置背景透明
        GetNode<ColorRect>("Background").Color = new Color(0, 0, 0, 0);
    }

    private void SpawnShuttlecock()
    {
        if (_shuttlecockScene != null)
        {
            _currentShuttlecock = _shuttlecockScene.Instantiate<Shuttlecock>();
            _currentShuttlecock.Position = new Vector2(960, 540); // 屏幕中心
            _currentShuttlecock.GravityScale = Gravity / 980.0f; // 标准化重力
            _currentShuttlecock.AirResistance = AirResistance;
            _currentShuttlecock.BounceDamping = BounceDamping;
            
            var container = GetNode<Node2D>("ShuttlecockContainer");
            container.AddChild(_currentShuttlecock);
            
            // 连接羽毛球信号
            _currentShuttlecock.Connect("mouse_entered", new Callable(this, nameof(OnShuttlecockMouseEntered)));
            _currentShuttlecock.Connect("mouse_exited", new Callable(this, nameof(OnShuttlecockMouseExited)));
        }
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (_currentShuttlecock == null) return;

        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                if (mouseEvent.IsPressed())
                {
                    // 检查是否点击在羽毛球上
                    var mousePos = GetGlobalMousePosition();
                    var distance = mousePos.DistanceTo(_currentShuttlecock.GlobalPosition);
                    
                    if (distance < 30) // 羽毛球点击半径
                    {
                        StartDragging(mousePos);
                    }
                }
                else if (mouseEvent.IsReleased() && _isDragging)
                {
                    EndDragging();
                }
            }
        }
        else if (@event is InputEventMouseMotion mouseMotion && _isDragging)
        {
            UpdateDragging(mouseMotion.Position);
        }
    }

    private void StartDragging(Vector2 mousePos)
    {
        _isDragging = true;
        _dragOffset = mousePos - _currentShuttlecock.GlobalPosition;
        _dragStartPos = _currentShuttlecock.GlobalPosition;
        _dragStartTime = Time.GetTicksMsec();
        
        // 停止羽毛球物理模拟
        _currentShuttlecock.Freeze = true;
        _currentShuttlecock.LinearVelocity = Vector2.Zero;
        _currentShuttlecock.AngularVelocity = 0;
        
        // 设置鼠标穿透为false，以便捕获鼠标事件
        _currentShuttlecock.MouseFilter = MouseFilterEnum.Stop;
    }

    private void UpdateDragging(Vector2 mousePos)
    {
        if (_currentShuttlecock != null && _isDragging)
        {
            var targetPos = mousePos - _dragOffset;
            
            // 限制拖拽距离
            var distance = targetPos.DistanceTo(_dragStartPos);
            if (distance > MaxDragDistance)
            {
                var direction = (targetPos - _dragStartPos).Normalized();
                targetPos = _dragStartPos + direction * MaxDragDistance;
            }
            
            _currentShuttlecock.GlobalPosition = targetPos;
            
            // 计算拖拽速度（用于投掷）
            var currentTime = Time.GetTicksMsec();
            var deltaTime = (currentTime - _dragStartTime) / 1000.0;
            if (deltaTime > 0)
            {
                _dragVelocity = (targetPos - _dragStartPos) / (float)deltaTime;
            }
        }
    }

    private void EndDragging()
    {
        if (_currentShuttlecock != null && _isDragging)
        {
            _isDragging = false;
            
            // 恢复物理模拟
            _currentShuttlecock.Freeze = false;
            
            // 应用投掷力
            var throwForce = _dragVelocity * ThrowMultiplier;
            _currentShuttlecock.LinearVelocity = throwForce;
            
            // 添加一些旋转效果
            _currentShuttlecock.AngularVelocity = throwForce.Length() * 0.1f;
            
            // 恢复鼠标穿透
            _currentShuttlecock.MouseFilter = MouseFilterEnum.Pass;
            
            // 播放投掷音效（如果有）
            PlayThrowSound();
        }
    }

    private void OnShuttlecockMouseEntered()
    {
        if (!_isDragging && _currentShuttlecock != null)
        {
            // 鼠标悬停效果
            _currentShuttlecock.Modulate = new Color(1.2f, 1.2f, 1.2f, 1.0f);
            _currentShuttlecock.MouseFilter = MouseFilterEnum.Stop;
        }
    }

    private void OnShuttlecockMouseExited()
    {
        if (!_isDragging && _currentShuttlecock != null)
        {
            // 恢复正常颜色
            _currentShuttlecock.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            _currentShuttlecock.MouseFilter = MouseFilterEnum.Pass;
        }
    }

    private void PlayThrowSound()
    {
        // 这里可以添加投掷音效
        // 暂时留空，可以后续添加
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        // 检查羽毛球是否飞出屏幕，如果是则重置位置
        if (_currentShuttlecock != null && !_isDragging)
        {
            var screen = DisplayServer.ScreenGetSize();
            var pos = _currentShuttlecock.GlobalPosition;
            
            if (pos.X < -100 || pos.X > screen.X + 100 || 
                pos.Y < -100 || pos.Y > screen.Y + 100)
            {
                ResetShuttlecock();
            }
        }
    }

    private void ResetShuttlecock()
    {
        if (_currentShuttlecock != null)
        {
            _currentShuttlecock.GlobalPosition = new Vector2(960, 540);
            _currentShuttlecock.LinearVelocity = Vector2.Zero;
            _currentShuttlecock.AngularVelocity = 0;
            _currentShuttlecock.Rotation = 0;
        }
    }

    // 键盘快捷键
    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        
        if (@event is InputEventKey keyEvent)
        {
            if (keyEvent.Keycode == Key.R && keyEvent.IsPressed())
            {
                ResetShuttlecock();
            }
            else if (keyEvent.Keycode == Key.Escape && keyEvent.IsPressed())
            {
                GetTree().Quit();
            }
        }
    }
}