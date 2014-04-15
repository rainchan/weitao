@echo 启动服务 不用设置
@Set Path=C:\WINDOWS\system32;
net stop AccessTokenWindowService


@echo 设置installutil.exe执行路径 如果Framework装到其他磁盘请参考如下路径进行修改 
@echo 如果默认安装在c盘则不用进行设置
@Set Path=C:\Windows\Microsoft.NET\Framework\v4.0.30319;

@echo 转到服务所在盘,即:部署文件解压后所在的磁盘
D:

@echo 转到服务所在目录,即:"服务程序"文件夹的本地绝对路径,不带盘符，如下所示,那莫同上一个设置结合起来看，可以保证UpdateService.exe在本地的
@echo “D:\部署文件\服务程序”文件夹中
cd \BasicServices\AccessTokenWindowService

@echo 安装服务 不用设置
installutil WT.Services.AccessToken.WinService.exe -u









