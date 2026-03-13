# VR教室数据协议 v1.0

## 协议目的
定义VR教室场景数据的标准JSON格式。

## 数据结构
```json
{
  "metadata": {...},
  "objects": [...]
}

元数据字段
字段	必需	说明	示例
exportTime	是	导出时间	"2026-01-26 23:06:13"
sceneName	是	场景名	"Classroom"
totalObjects	是	物体数	140
toolVersion	是	工具版本	"1.0"

物体数据
每个物体包含：
{
  "name": "物体名称",
  "type": "物体类型",
  "position": {"x":0, "y":0, "z":0},
  "rotation": {"x":0, "y":0, "z":0},
  "scale": {"x":1, "y":1, "z":1}
}

物体类型
类型	识别规则	示例
desk	名称含"desk"或"table"	DeskAndChair
chair	名称含"chair"或"seat"	Chair
wall	名称含"wall"	WallCollider
window	名称含"window"	Window
door	名称含"door"	Door
light	名称含"light"或"lamp"	Light
camera	名称含"camera"	Camera
other	其他	TestCube

格式规范
坐标系：Unity右手系

单位：米

精度：2位小数

时间：YYYY-MM-DD HH:mm:ss

验证要求
元数据字段必须完整

物体必须有name和type

类型必须在列表中