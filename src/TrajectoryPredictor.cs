using Godot;
using System;
using System.Collections.Generic;

namespace DesktopBadmintonGame;

public partial class TrajectoryPredictor : Node2D
{
    private PackedScene _trajectoryPointScene;
    private List<Node2D> _trajectoryPoints = new List<Node2D>();
    private bool _showTrajectory = false;
    private Vector2 _startPosition;
    private Vector2 _initialVelocity;
    private float _gravity = 980.0f;
    private float _airResistance = 0.99f;
    
    [ExportGroup("Trajectory Settings")]
    [Export] public int MaxPoints = 30;
    [Export] public float PointSpacing = 15.0f;
    [Export] public float PredictionTime = 2.0f;
    [Export] public Color TrajectoryColor = new Color(1, 1, 0, 0.6f); // 黄色轨迹
    [Export] public float PointSize = 3.0f;
    
    [ExportGroup("Physics Settings")]
    [Export] public float Gravity 
    { 
        get => _gravity; 
        set => _gravity = value; 
    }
    [Export] public float AirResistance 
    { 
        get => _airResistance; 
        set => _airResistance = Mathf.Clamp(value, 0.9f, 1.0f); 
    }

    public override void _Ready()
    {
        base._Ready();
        
        // 加载轨迹点场景
        _trajectoryPointScene = ResourceLoader.Load<PackedScene>("res://objects/TrajectoryPoint.tscn");
        
        // 初始化时隐藏
        Visible = false;
    }

    public void ShowTrajectory(Vector2 startPosition, Vector2 initialVelocity)
    {
        if (!GameMode.IsEasyMode())
        {
            HideTrajectory();
            return;
        }
        
        _startPosition = startPosition;
        _initialVelocity = initialVelocity;
        _showTrajectory = true;
        Visible = true;
        
        UpdateTrajectoryPoints();
    }

    public void HideTrajectory()
    {
        _showTrajectory = false;
        Visible = false;
        ClearTrajectoryPoints();
    }

    public void UpdateTrajectory(Vector2 startPosition, Vector2 initialVelocity)
    {
        if (!_showTrajectory || !GameMode.IsEasyMode())
        {
            return;
        }
        
        _startPosition = startPosition;
        _initialVelocity = initialVelocity;
        UpdateTrajectoryPoints();
    }

    private void UpdateTrajectoryPoints()
    {
        ClearTrajectoryPoints();
        
        if (!_showTrajectory || !GameMode.IsEasyMode())
        {
            return;
        }
        
        // 计算轨迹点
        var points = CalculateTrajectoryPoints();
        
        // 创建轨迹点可视化
        foreach (var point in points)
        {
            CreateTrajectoryPoint(point);
        }
    }

    private List<Vector2> CalculateTrajectoryPoints()
    {
        var points = new List<Vector2>();
        var position = _startPosition;
        var velocity = _initialVelocity;
        var dt = 0.016f; // 60 FPS
        var time = 0.0f;
        
        for (int i = 0; i < MaxPoints && time < PredictionTime; i++)
        {
            points.Add(position);
            
            // 物理计算：考虑重力和空气阻力
            var gravityForce = new Vector2(0, _gravity);
            var dragForce = -velocity * (1.0f - _airResistance);
            var acceleration = gravityForce + dragForce;
            
            // 更新速度和位置
            velocity += acceleration * dt;
            position += velocity * dt;
            
            time += dt;
            
            // 检查是否碰到地面（简化处理）
            if (position.Y > 1080) // 假设屏幕高度为1080
            {
                break;
            }
        }
        
        return points;
    }

    private void CreateTrajectoryPoint(Vector2 position)
    {
        if (_trajectoryPointScene != null)
        {
            var point = _trajectoryPointScene.Instantiate<Node2D>();
            point.GlobalPosition = position;
            
            // 设置点的属性
            if (point is Sprite2D sprite)
            {
                sprite.Modulate = TrajectoryColor;
                sprite.Scale = new Vector2(PointSize / 10.0f, PointSize / 10.0f);
            }
            
            AddChild(point);
            _trajectoryPoints.Add(point);
        }
        else
        {
            // 如果没有预设场景，创建简单的点
            var sprite = new Sprite2D();
            sprite.Texture = CreatePointTexture();
            sprite.Modulate = TrajectoryColor;
            sprite.Scale = new Vector2(PointSize / 10.0f, PointSize / 10.0f);
            sprite.GlobalPosition = position;
            
            AddChild(sprite);
            _trajectoryPoints.Add(sprite);
        }
    }

    private Texture2D CreatePointTexture()
    {
        // 创建一个简单的圆形纹理
        var image = new Image();
        image.Create(8, 8, false, Image.Format.Rgba8);
        
        // 填充圆形
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                var centerX = 3.5f;
                var centerY = 3.5f;
                var distance = Mathf.Sqrt(Mathf.Pow(x - centerX, 2) + Mathf.Pow(y - centerY, 2));
                
                if (distance <= 3.5f)
                {
                    image.SetPixel(x, y, new Color(1, 1, 1, 1));
                }
                else
                {
                    image.SetPixel(x, y, new Color(1, 1, 1, 0));
                }
            }
        }
        
        return ImageTexture.CreateFromImage(image);
    }

    private void ClearTrajectoryPoints()
    {
        foreach (var point in _trajectoryPoints)
        {
            if (IsInstanceValid(point))
            {
                point.QueueFree();
            }
        }
        _trajectoryPoints.Clear();
    }

    public void SetGravity(float gravity)
    {
        _gravity = gravity;
        if (_showTrajectory)
        {
            UpdateTrajectoryPoints();
        }
    }

    public void SetAirResistance(float airResistance)
    {
        _airResistance = Mathf.Clamp(airResistance, 0.9f, 1.0f);
        if (_showTrajectory)
        {
            UpdateTrajectoryPoints();
        }
    }

    public void SetTrajectoryColor(Color color)
    {
        TrajectoryColor = color;
        UpdateExistingPointsColor();
    }

    private void UpdateExistingPointsColor()
    {
        foreach (var point in _trajectoryPoints)
        {
            if (IsInstanceValid(point) && point is Sprite2D sprite)
            {
                sprite.Modulate = TrajectoryColor;
            }
        }
    }

    public bool IsShowingTrajectory()
    {
        return _showTrajectory && Visible;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        ClearTrajectoryPoints();
    }
}