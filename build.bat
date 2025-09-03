@echo off
echo ========================================
echo    桌面羽毛球游戏构建脚本
echo ========================================
echo.

REM 检查Godot是否在PATH中
where godot >nul 2>&1
if %errorlevel% neq 0 (
    echo 错误: 未找到Godot，请确保Godot在PATH中
    echo 或者直接运行: godot --export "Windows Desktop" desktop_badminton_game.exe
    exit /b 1
)

echo 正在构建桌面羽毛球游戏...
echo.

REM 构建Windows版本
godot --export "Windows Desktop" desktop_badminton_game.exe

if %errorlevel% neq 0 (
    echo 构建失败！
    exit /b 1
)

echo.
echo ✓ 构建成功！
echo 输出文件: desktop_badminton_game.exe
echo.
echo 运行说明:
echo 1. 双击 desktop_badminton_game.exe 启动游戏
echo 2. 游戏将以透明窗口模式运行
echo 3. 使用鼠标拖拽羽毛球进行投掷
echo 4. 按R键重置，ESC键退出
echo.
echo 注意: 请确保已准备好所需的图片资源文件