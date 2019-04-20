# ExcelParser

ExcelParser是为游戏开发设计的配置解析程序。主要功能是代码生成，读表并序列化。

## 如何使用

## 类型解析

### 类型

* 简单类型： int, float, double, bool, string
* 列表
* 自定义类型

### 产生式

type -> objType:id | listType:id | simpleType:id  
objType -> obj{type objTypeRemain}  
objTypeRemain -> ;type | ε  
listType -> list{objType} | list{listType} | list{simpleType}  
simpleType -> int | float | double | bool | string  
id -> letter_(letter_ | digit)*  
leter_ -> [a-zA-Z_]  
digit -> [0-9]  