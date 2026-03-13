# 开发环境配置指南

## 环境要求
- Unity 2022.3.62f2c1
- Visual Studio 2022 或 VS Code
- Git

## 项目获取
```bash
git clone https://github.com/xuzhiheng215/UnityClassroom.git
cd UnityClassroom

项目结构
Assets/Editor/Tools/
├── SceneAnalyzer/       # UI界面(B同学)
│   ├── ProtocolAnalyzerWindow.cs
│   ├── ObjectListPanel.cs
│   └── ExportControlPanel.cs
├── DataExtractor/       # 数据提取(A同学)
│   └── SceneScanner.cs
├── Performance/         # 性能监控(D同学)
│   └── AnalysisProfiler.cs
└── Integration/         # 集成测试(C同学)
    └── IntegrationTester.cs

关键文件
ProtocolAnalyzerWindow.cs - 主窗口

SceneScanner.cs - 数据提取

ExportControlPanel.cs - JSON导出

Git协作流程
main (稳定版)
├── feature/data-extractor    # A同学
├── feature/editor-ui         # B同学
├── feature/performance       # D同学
└── integration               # C同学

测试流程
打开SimpleTest.unity

运行Window → VR Protocol Analyzer

测试扫描功能

测试导出功能

验证JSON文件

常见问题
1. 编译错误
检查类名重复

检查命名空间

2. JSON导出失败
检查GeneratedOutput目录权限

查看控制台错误

3. UI不显示
检查窗口高度

使用ScrollView

联系方式
问题：查看控制台错误

协作：联系团队负责人

紧急：群组@所有成员