@echo �������� ��������
@Set Path=C:\WINDOWS\system32;
net stop AccessTokenWindowService


@echo ����installutil.exeִ��·�� ���Frameworkװ������������ο�����·�������޸� 
@echo ���Ĭ�ϰ�װ��c�����ý�������
@Set Path=C:\Windows\Microsoft.NET\Framework\v4.0.30319;

@echo ת������������,��:�����ļ���ѹ�����ڵĴ���
D:

@echo ת����������Ŀ¼,��:"�������"�ļ��еı��ؾ���·��,�����̷���������ʾ,��Īͬ��һ�����ý�������������Ա�֤UpdateService.exe�ڱ��ص�
@echo ��D:\�����ļ�\��������ļ�����
cd \BasicServices\AccessTokenWindowService

@echo ��װ���� ��������
installutil WT.Services.AccessToken.WinService.exe -u









