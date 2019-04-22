# ExcelParser

ExcelParser是为游戏开发设计的配置解析程序。主要功能是代码生成，读表并序列化。

## 如何使用

## 类型解析

### 类型

* 简单类型： int, float, double, bool, string
* 列表
* 自定义类型

### 产生式

field -> objField:id | listField:id | simpleField:id  
objField -> obj{field objFieldRemain}  
objFieldRemain -> ;field objFieldRemain | ε  
listField -> list{field}
simpleField -> int | float | double | bool | string  