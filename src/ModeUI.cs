using Godot;
using System;

namespace DesktopBadmintonGame;

public partial class ModeUI : Control
{
    private Label _modeLabel;
    private Button _modeButton;
    private Timer _hideTimer;
    private bool _isVisible = false;
    
    [Export] public float AutoHideTime = 3.0f;
    [Export] public Color EasyModeColor = new Color(0.2f, 0.8f, 0.2f, 0.8f);
    [Export] public Color HardModeColor = new Color(0.8f, 0.2f, 0.2f, 0.8f);

    public override void _Ready()
    {
        base._Ready();
        
        // 创建UI元素
        CreateUI();
        
        // 创建自动隐藏计时器
        _hideTimer = new Timer();
        _hideTimer.WaitTime = AutoHideTime;
        _hideTimer.OneShot = true;
        _hideTimer.Timeout += OnHideTimerTimeout;
        AddChild(_hideTimer);
        
        // 初始隐藏
        Visible = false;
        _isVisible = false;
        
        // 连接模式切换信号
        if (_modeButton != null)
        {
            _modeButton.Pressed += OnModeButtonPressed;
        }
        
        // 初始更新
        UpdateModeDisplay();
    }

    private void CreateUI()
    {
        // 创建背景面板
        var panel = new Panel();
        panel.Size = new Vector2(200, 80);
        panel.Position = new Vector2(10, 10);
        panel.Modulate = new Color(0, 0, 0, 0.7f);
        AddChild(panel);
        
        // 创建模式标签
        _modeLabel = new Label();
        _modeLabel.Text = "模式: 困难";
        _modeLabel.Position = new Vector2(10, 10);
        _modeLabel.Size = new Vector2(180, 30);
        _modeLabel.HorizontalAlignment = HorizontalAlignment.Center;
        _modeLabel.VerticalAlignment = VerticalAlignment.Center;
        _modeLabel.Modulate = Colors.White;
        panel.AddChild(_modeLabel);
        
        // 创建切换按钮
        _modeButton = new Button();
        _modeButton.Text = "切换模式";
        _modeButton.Position = new Vector2(10, 45);
        _modeButton.Size = new Vector2(180, 25);
        panel.AddChild(_modeButton);
        
        // 设置鼠标穿透
        MouseFilter = MouseFilterEnum.Stop;
        panel.MouseFilter = MouseFilterEnum.Stop;
        _modeLabel.MouseFilter = MouseFilterEnum.Pass;
        _modeButton.MouseFilter = MouseFilterEnum.Stop;
    }

    private void OnModeButtonPressed()
    {
        // 切换游戏模式
        GameMode.ToggleDifficulty();
        UpdateModeDisplay();
        
        // 重新开始自动隐藏计时器
        _hideTimer.Start();
        
        // 播放切换音效（如果有）
        PlayModeSwitchSound();
    }

    private void UpdateModeDisplay()
    {
        if (_modeLabel != null)
        {
            var modeText = GameMode.GetDifficultyName();
            _modeLabel.Text = $"模式: {modeText}";
            
            // 根据模式设置颜色
            if (GameMode.IsEasyMode())
            {
                _modeLabel.Modulate = EasyModeColor;
            }
            else
            {
                _modeLabel.Modulate = HardModeColor;
            }
        }
        
        if (_modeButton != null)
        {
            _modeButton.Text = GameMode.IsEasyMode() ? "切换到困难" : "切换到简单";
        }
    }

    public void ShowUI()
    {
        Visible = true;
        _isVisible = true;
        UpdateModeDisplay();
        
        // 开始自动隐藏计时器
        _hideTimer.Start();
    }

    public void HideUI()
    {
        Visible = false;
        _isVisible = false;
        _hideTimer.Stop();
    }

    public void ToggleUI()
    {
        if (_isVisible)
        {
            HideUI();
        }
        else
        {
            ShowUI();
        }
    }

    private void OnHideTimerTimeout()
    {
        HideUI();
    }

    private void PlayModeSwitchSound()
    {
        // 这里可以添加模式切换音效
        // 暂时留空，可以后续添加
        GD.Print("模式切换音效");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        // 检查是否需要更新显示
        if (_isVisible && _modeLabel != null)
        {
            UpdateModeDisplay();
        }
    }

    // 键盘快捷键支持
    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        
        if (@event is InputEventKey keyEvent)
        {
            if (keyEvent.Keycode == Key.M && keyEvent.IsPressed())
            {
                ToggleUI();
            }
            else if (keyEvent.Keycode == Key.Tab && keyEvent.IsPressed())
            {
                GameMode.ToggleDifficulty();
                UpdateModeDisplay();
                if (_isVisible)
                {
                    _hideTimer.Start();
                }
            }
        }
    }
}