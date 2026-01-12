# EnhancedItemInfo

## 功能
* 根据物品的稀有度/价值显示不同颜色
* 显示物品信息
  * 已有（背包、宠物、仓库）（不包括配件槽和身上装备）
  * 耐久、（预估）可用次数（食物的耐久消耗会正常显示）
  * 需求（任务、强化、建筑）（不包括可无限建造的建筑）
  * 更高精度的重量（会额外显示剔除配件后的自重）
  * 价格
  * 分解、分解来源
  * 在黑市显示武器、装备、配件等物品的属性
* 为已录入钥匙蓝图增加标识（包括黑市）
* 可通过 ModSetting 在游戏中配置
* 在余额旁显示现有现金数量（在基地外不包含仓库数量）

### TODO
* 优化信息显示
* 做了很多提前优化和没用的工具类，代码清理？
* 可配置化（优化、接入 ModConfig）
* 武器属性对比

## 参考
| Mod                                                  | Workshop                                                                  | Github                                                        | License           |                   |
| ---------------------------------------------------- | ------------------------------------------------------------------------- | ------------------------------------------------------------- | ----------------- | ----------------- |
| 显示物品价值                                         | [link](https://steamcommunity.com/sharedfiles/filedetails/?id=3532400883) | [link](https://github.com/xvrsl/duckov_modding)               | 官方教程          | -                 |
| 物品价值稀有度与搜索音效{#level}                     | [link](https://steamcommunity.com/sharedfiles/filedetails/?id=3588386576) | [link](https://github.com/dzj0821/ItemLevelAndSearchSoundMod) | MIT license       | 物品稀有度部分    |
| Better Key&Blueprint Indicator - 更好的钥匙&蓝图标识 | [link](https://steamcommunity.com/sharedfiles/filedetails/?id=3590154134) | [link](https://github.com/Tonwed/KeycardRecordedIndicator)    | MIT license       | 创建标识部分      |
| JMC Mod模板                                          | -                                                                         | [link](https://github.com/JMC2002/ModTemplate)                | MIT license       | 项目结构和.csproj |
| ModConfig                                            | [link](https://steamcommunity.com/sharedfiles/filedetails/?id=3590674339) | [link](https://github.com/FrozenFish259/duckov_mod_config)    | Unlicense license | 参考、接入配置    |
| ModSetting                                           | [link](https://steamcommunity.com/sharedfiles/filedetails/?id=3595729494) | [link](https://github.com/xisTC/ModSetting)                   | MIT license       | 参考、接入配置    |
| 显示现金                                             | [link](https://steamcommunity.com/sharedfiles/filedetails/?id=3588488152) | [link](https://github.com/dzj0821/DisplayCashWithMoney)       | MIT lincense      | -                 |

## 工具
向ChatGPT询问了部分Unity和C#相关知识

使用[ILSpy](https://github.com/icsharpcode/ILSpy)查看游戏代码

使用[Harmony](https://github.com/pardeike/Harmony)进行游戏代码修改，参考了其[文档](https://harmony.pardeike.net)