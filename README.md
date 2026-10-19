Unity作業四

尚烜妃-F15261011

專案截圖：

<img width="865" height="496" alt="image" src="https://github.com/user-attachments/assets/47ab0d56-6122-4fa9-bed6-d951af735596" />

 
Github鏈接：

https://github.com/oozeww/1151VR-HW4-F15261011-SXF

Youtube鏈接：

https://youtu.be/P1qIo1lqpV4

製作流程：

1.	我于網絡下載了建築物作爲背景、障礙物以及飛行器模型，設置了基礎的碰撞和wasd jk控制

2.	添加了canvas作爲UI

3.	實現音效：添加了幾個圓環指引通過，在script中判斷，如果撞到東西即發出碰撞音效；如果穿過圓環播放成功音效；常態情況持續播放轟鳴的飛行音效；

4.	發現bug，飛機落在地面也會發出碰撞音效，詢問ai尋找解決方法：給障礙方塊創建專用的layer

5.	完善：當碰撞時播放碰撞的粒子效果

6.	爲了在手機上使用，在脚本裏添加了對無綫手柄的支持
