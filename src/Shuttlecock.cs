using Godot;
using System;

namespace DesktopBadmintonGame;

public partial class Shuttlecock : RigidBody2D
{
    [Signal]
    public delegate void HitGroundEventHandler();
    
    [Signal]
    public delegate void ThrownEventHandler(Vector2 velocity);

    private Line2D _trail;
    private Vector2[] _trailPoints;
    private int _maxTrailPoints = 20;
    private double _lastTrailTime;
    private float _trailInterval = 0.02f; // 20ms间隔
    
    private Vector2 _lastPosition;
    private Vector2 _velocity;
    private float _airResistance = 0.99f;
    private float _bounceDamping = 0.7f;
    private float _rotationSpeed = 0f;
    
    [ExportGroup("Physics Properties")]
    [Export] public float GravityScale { get; set; } = 1.0f;
    [Export] public float AirResistance 
    { 
        get => _airResistance; 
        set => _airResistance = Mathf.Clamp(value, 0.9f, 1.0f); 
    }
    [Export] public float BounceDamping 
    { 
        get => _bounceDamping; 
        set => _bounceDamping = Mathf.Clamp(value, 0.1f, 1.0f); 
    }
    
    [ExportGroup("Visual Effects")]
    [Export] public bool ShowTrail { get; set; } = true;
    [Export] public Color TrailColor { get; set; } = new Color(1, 1, 1, 0.3);
    [Export] public float TrailWidth { get; set; } = 2.0f;

    public override void _Ready()
    {
        base._Ready();
        
        // 初始化轨迹
        _trail = GetNode<Line2D>("Trail");
        _trailPoints = new Vector2[_maxTrailPoints];
        _lastTrailTime = Time.GetTicksMsec() / 1000.0;
        
        // 初始化位置和速度
        _lastPosition = GlobalPosition;
        _velocity = Vector2.Zero;
        
        // 设置物理属性
        Mass = 0.005f; // 羽毛球质量约5克
        GravityScale = GravityScale;
        
        // 连接信号
        BodyEntered += OnBodyEntered;
        BodyShapeEntered += OnBodyShapeEntered;
        
        // 设置初始轨迹样式
        UpdateTrailStyle();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        // 计算速度
        _velocity = (GlobalPosition - _lastPosition) / (float)delta;
        _lastPosition = GlobalPosition;
        
        // 应用空气阻力
        if (!_velocity.IsZeroApprox())
        {
            var dragForce = -_velocity * (1.0f - _airResistance);
            ApplyCentralImpulse(dragForce);
        }
        
        // 更新旋转（基于速度）
        if (!_velocity.IsZeroApprox())
        {
            _rotationSpeed = _velocity.Length() * 0.05f;
            Rotation += _rotationSpeed * (float)delta;
        }
        
        // 更新轨迹
        if (ShowTrail)
        {
            UpdateTrail();
        }
    }

    private void UpdateTrail()
    {
        var currentTime = Time.GetTicksMsec() / 1000.0;
        
        if (currentTime - _lastTrailTime >= _trailInterval)
        {
            // 添加新的轨迹点
            for (int i = _maxTrailPoints - 1; i > 0; i--)
            {
                _trailPoints[i] = _trailPoints[i - 1];
            }
            _trailPoints[0] = GlobalPosition;
            
            // 更新轨迹线
            _trail.ClearPoints();
            for (int i = 0; i < _maxTrailPoints; i++)
            {
                if (!_trailPoints[i].IsZeroApprox())
                {
                    var alpha = 1.0f - (float)i / _maxTrailPoints;
                    _trail.AddPoint(_trailPoints[i]);
                }
            }
            
            _lastTrailTime = currentTime;
        }
    }

    private void UpdateTrailStyle()
    {
        if (_trail != null)
        {
            _trail.DefaultColor = TrailColor;
            _trail.Width = TrailWidth;
            _trail.TextureMode = Line2D.LineTextureMode.Stretch;
        }
    }

    private void OnBodyEntered(Node body)
    {
        // 播放碰撞音效
        PlayHitSound();
        
        // 发出碰撞信号
        EmitSignal(SignalName.HitGround);
        
        // 添加碰撞效果
        CreateHitEffect();
    }

    private void OnBodyShapeEntered(Rid bodyRid, Node body, long bodyShapeIndex, long localShapeIndex)
    {
        // 处理特定形状的碰撞
        if (body is StaticBody2D)
        {
            // 计算反弹
            var normal = GetContactNormal(localShapeIndex);
            var velocity = LinearVelocity;
            var reflected = velocity.Reflect(normal);
            
            // 应用反弹衰减
            LinearVelocity = reflected * _bounceDamping;
            
            // 添加一些随机性使运动更自然
            AngularVelocity = (float)GD.RandRange(-5, 5);
        }
    }

    private Vector2 GetContactNormal(long shapeIndex)
    {
        // 获取碰撞法线
        var contactCount = GetContactCount();
        for (int i = 0; i < contactCount; i++)
        {
            if (GetContactLocalShape(i) == shapeIndex)
            {
                return GetContactNormal(i);
            }
        }
        return Vector2.Up; // 默认向上
    }

    private void PlayHitSound()
    {
        // 这里可以添加碰撞音效
        // 暂时留空，可以后续添加
        GD.Print("Hit sound played");
    }

    private void CreateHitEffect()
    {
        // 创建碰撞视觉效果
        var particles = new CPUParticles2D();
        particles.Emitting = true;
        particles.Amount = 10;
        particles.Lifetime = 0.5f;
        particles.Explosiveness = 0.8f;
        particles.Direction = Vector2.Up;
        particles.Spread = 45.0f;
        particles.InitialVelocityMin = 50.0f;
        particles.InitialVelocityMax = 100.0f;
        particles.Gravity = Vector2.Zero;
        
        // 设置粒子颜色
        particles.Color = new Color(1, 1, 1, 0.8f);
        
        // 添加到场景
        GetParent().AddChild(particles);
        particles.GlobalPosition = GlobalPosition;
        
        // 自动清理
        particles.Connect("finished", new Callable(particles, "queue_free"));
    }

    public void ApplyThrowForce(Vector2 force)
    {
        // 应用投掷力
        ApplyCentralImpulse(force);
        
        // 发出投掷信号
        EmitSignal(SignalName.Thrown, force);
        
        // 清除轨迹
        ClearTrail();
    }

    public void ClearTrail()
    {
        if (_trail != null)
        {
            _trail.ClearPoints();
            for (int i = 0; i < _maxTrailPoints; i++)
            {
                _trailPoints[i] = Vector2.Zero;
            }
        }
    }

    public void ResetToPosition(Vector2 position)
    {
        // 重置羽毛球到指定位置
        GlobalPosition = position;
        LinearVelocity = Vector2.Zero;
        AngularVelocity = 0;
        Rotation = 0;
        Freeze = false;
        
        // 清除轨迹
        ClearTrail();
    }

    public bool IsMoving()
    {
        return !_velocity.IsZeroApprox() && LinearVelocity.Length() > 10.0f;
    }

    public float GetSpeed()
    {
        return LinearVelocity.Length();
    }

    // 设置空气阻力
    public void SetAirResistance(float resistance)
    {
        _airResistance = Mathf.Clamp(resistance, 0.9f, 1.0f);
    }

    // 设置反弹衰减
    public void SetBounceDamping(float damping)
    {
        _bounceDamping = Mathf.Clamp(damping, 0.1f, 1.0f);
    }

    // 切换轨迹显示
    public void ToggleTrail(bool show)
    {
        ShowTrail = show;
        if (_trail != null)
        {
            _trail.Visible = show;
        }
        if (!show)
        {
            ClearTrail();
        }
    }
}