# lostsmile unity 中文本地化项目
## 本项目仅供学习交流使用，严禁一切商业或特殊用途！！！
![Image text](https://raw.githubusercontent.com/cokkeijigen/lostsmile_cn/master/Pictures/lostsmile_00.png)<br>
## `LOSTSMILE_CN`
**程序`hook`相关，实现了资源重定向，可以与原版资源共存**
## `Assembly-CSharp` 
**原版`Assembly-CSharp.dll`反编译源码，修复存档路径问题，以及添加自定义AssetBundle加载和资源替换 <br>
注：`release`分支为未修改过的源码**

## 笔记：如何打包使用Unity打包Assetbundle
首先要知道游戏的unity版本，这个可以通过在游戏`主程序.exe`的属性或者`UnityPlayer.dll`属性中查看，再或者用`ida`打开`UnityPlayer.dll`查找字符串`version`或者`UnityPlayer`等字眼找到。
![Image text](https://raw.githubusercontent.com/cokkeijigen/lostsmile_cn/master/Pictures/lostsmile_01.png)<br>
从[UnityArchive](https://unity.com/cn/releases/editor/archive)中下载对应版本的Unity，这个游戏的版本为`2018.4.15f1`，相信聪明的你，一定不用多说也知道怎么下载安装了吧。
![Image text](https://raw.githubusercontent.com/cokkeijigen/lostsmile_cn/master/Pictures/lostsmile_02.png)<br>
接着随便新建一个项目，`Template`随意，用默认的就行了
![Image text](https://raw.githubusercontent.com/cokkeijigen/lostsmile_cn/master/Pictures/lostsmile_03.png)<br>
来到项目`Assets`目录下新建一个`Editor`的目录
![Image text](https://raw.githubusercontent.com/cokkeijigen/lostsmile_cn/master/Pictures/lostsmile_04.png)<br>
