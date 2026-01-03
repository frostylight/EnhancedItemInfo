# EnhancedItemInfo

## 功能
* 根据物品的稀有度/价值显示不同颜色
* 显示物品信息
  * 已有（背包、宠物、仓库）（不包括配件槽和身上装备）
  * 需求（任务、强化、建筑）（不包括可无限建造的建筑）
  * 重量
  * 价格
  * 分解
* 为已录入钥匙蓝图增加标识（包括黑市）

### TODO
* 优化信息显示
* 做了很多提前优化和没用的工具类，代码清理？

## 参考
|Mod|Workshop|Github|License||
|-|-|-|-|-|
|显示物品价值|[link](https://steamcommunity.com/sharedfiles/filedetails/?id=3532400883)|[link](https://github.com/xvrsl/duckov_modding)|官方教程|-|
|物品价值稀有度与搜索音效{#level}| [link](https://steamcommunity.com/sharedfiles/filedetails/?id=3588386576)| [link](https://github.com/dzj0821/ItemLevelAndSearchSoundMod)| MIT License|物品稀有度部分|
|Better Key&Blueprint Indicator - 更好的钥匙&蓝图标识|[link](https://steamcommunity.com/sharedfiles/filedetails/?id=3590154134)| [link](https://github.com/Tonwed/KeycardRecordedIndicator)|MIT License|创建标识部分|
|JMC Mod模板|-|[link](https://github.com/JMC2002/ModTemplate)|MIT License|项目结构和.csproj|

## 工具
向ChatGPT询问了部分Unity和C#相关知识

使用[ILSpy](https://github.com/icsharpcode/ILSpy)查看游戏代码

使用[Harmony](https://github.com/pardeike/Harmony)进行游戏代码修改，参考了其[文档](https://harmony.pardeike.net)