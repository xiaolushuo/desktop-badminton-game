#!/usr/bin/env python3
"""
桌面羽毛球游戏项目验证脚本
检查项目文件结构和配置是否正确
"""

import os
import json
import sys
from pathlib import Path

def check_file_exists(filepath, description):
    """检查文件是否存在"""
    if os.path.exists(filepath):
        print(f"✓ {description}: {filepath}")
        return True
    else:
        print(f"✗ {description}: {filepath} (缺失)")
        return False

def check_directory_exists(dirpath, description):
    """检查目录是否存在"""
    if os.path.exists(dirpath) and os.path.isdir(dirpath):
        print(f"✓ {description}: {dirpath}")
        return True
    else:
        print(f"✗ {description}: {dirpath} (缺失)")
        return False

def check_godot_project():
    """检查Godot项目配置"""
    print("\n=== Godot项目配置检查 ===")
    
    project_file = "project.godot"
    if not check_file_exists(project_file, "项目配置文件"):
        return False
    
    # 读取并检查项目配置
    try:
        with open(project_file, 'r', encoding='utf-8') as f:
            content = f.read()
            
        required_configs = [
            'config/name="Desktop Badminton Game"',
            'run/main_scene="res://scenes/Main.tscn"',
            'window/size/borderless=true',
            'window/size/always_on_top=true',
            'window/size/transparent=true',
            'window/per_pixel_transparency/allowed=true'
        ]
        
        for config in required_configs:
            if config in content:
                print(f"✓ 配置项: {config}")
            else:
                print(f"✗ 配置项: {config} (缺失)")
                
    except Exception as e:
        print(f"✗ 读取项目配置文件失败: {e}")
        return False
    
    return True

def check_source_files():
    """检查源代码文件"""
    print("\n=== 源代码文件检查 ===")
    
    source_files = [
        ("src/Main.cs", "主场景脚本"),
        ("src/Shuttlecock.cs", "羽毛球脚本"),
        ("scenes/Main.tscn", "主场景文件"),
        ("objects/Shuttlecock.tscn", "羽毛球对象文件")
    ]
    
    all_exist = True
    for filepath, description in source_files:
        if not check_file_exists(filepath, description):
            all_exist = False
    
    return all_exist

def check_asset_files():
    """检查资源文件"""
    print("\n=== 资源文件检查 ===")
    
    asset_files = [
        ("assets/icon.png", "应用图标"),
        ("assets/shuttlecock_body.png", "羽毛球主体纹理"),
        ("assets/shuttlecock_feathers.png", "羽毛部分纹理"),
        ("assets/trail.png", "轨迹纹理")
    ]
    
    all_exist = True
    for filepath, description in asset_files:
        if os.path.exists(filepath):
            size = os.path.getsize(filepath)
            if size > 100:  # 假设有效文件大小大于100字节
                print(f"✓ {description}: {filepath} ({size} bytes)")
            else:
                print(f"⚠ {description}: {filepath} (可能是占位符文件)")
        else:
            print(f"✗ {description}: {filepath} (缺失)")
            all_exist = False
    
    return all_exist

def check_build_files():
    """检查构建相关文件"""
    print("\n=== 构建文件检查 ===")
    
    build_files = [
        ("export_presets.cfg", "导出配置"),
        ("build.bat", "Windows构建脚本"),
        ("README.md", "项目说明文档"),
        ("test_config.gd", "测试配置")
    ]
    
    all_exist = True
    for filepath, description in build_files:
        if not check_file_exists(filepath, description):
            all_exist = False
    
    return all_exist

def check_directory_structure():
    """检查目录结构"""
    print("\n=== 目录结构检查 ===")
    
    directories = [
        ("src", "源代码目录"),
        ("scenes", "场景文件目录"),
        ("objects", "游戏对象目录"),
        ("assets", "资源文件目录")
    ]
    
    all_exist = True
    for dirpath, description in directories:
        if not check_directory_exists(dirpath, description):
            all_exist = False
    
    return all_exist

def check_code_syntax():
    """检查C#代码语法（基础检查）"""
    print("\n=== 代码语法检查 ===")
    
    cs_files = ["src/Main.cs", "src/Shuttlecock.cs"]
    all_good = True
    
    for cs_file in cs_files:
        if os.path.exists(cs_file):
            try:
                with open(cs_file, 'r', encoding='utf-8') as f:
                    content = f.read()
                
                # 基础语法检查
                if 'using Godot;' in content:
                    print(f"✓ {cs_file}: Godot引用正确")
                else:
                    print(f"✗ {cs_file}: 缺少Godot引用")
                    all_good = False
                
                if 'public partial class' in content:
                    print(f"✓ {cs_file}: 类定义正确")
                else:
                    print(f"✗ {cs_file}: 类定义可能有问题")
                    all_good = False
                
                if content.count('{') == content.count('}'):
                    print(f"✓ {cs_file}: 括号匹配")
                else:
                    print(f"✗ {cs_file}: 括号不匹配")
                    all_good = False
                    
            except Exception as e:
                print(f"✗ {cs_file}: 读取失败 - {e}")
                all_good = False
        else:
            print(f"✗ {cs_file}: 文件不存在")
            all_good = False
    
    return all_good

def main():
    """主函数"""
    print("桌面羽毛球游戏项目验证")
    print("=" * 50)
    
    # 检查当前目录是否正确
    if not os.path.exists("project.godot"):
        print("错误: 请在项目根目录运行此脚本")
        sys.exit(1)
    
    # 执行各项检查
    checks = [
        check_directory_structure,
        check_godot_project,
        check_source_files,
        check_asset_files,
        check_build_files,
        check_code_syntax
    ]
    
    results = []
    for check in checks:
        try:
            result = check()
            results.append(result)
        except Exception as e:
            print(f"检查过程中出现错误: {e}")
            results.append(False)
    
    # 总结
    print("\n" + "=" * 50)
    print("验证总结:")
    
    passed = sum(results)
    total = len(results)
    
    print(f"通过检查: {passed}/{total}")
    
    if passed == total:
        print("✓ 项目配置完整，可以开始开发！")
    else:
        print("✗ 项目配置不完整，请检查上述问题")
        
        print("\n建议:")
        print("1. 确保所有必需文件都已创建")
        print("2. 准备好图片资源文件")
        print("3. 检查Godot项目配置")
        print("4. 安装Godot 4.3+版本")
    
    return passed == total

if __name__ == "__main__":
    success = main()
    sys.exit(0 if success else 1)