   M S B A M L   `   `   `  ����R  NInternetdownloadmanager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:InternetdownloadmanagerInternetdownloadmanager  5      %    Internetdownloadmanager.Window1   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      7local%clr-namespace:Internetdownloadmanager     5        � 
ResizeMode$  NoResize=�5       � Title$# Enter the address to download��5      $	��280��6+   $	��550��68    � Icon$ /assets/logo.jpg��6D   .��5	      � $��0,0,0,50q�6    � RowDefinitions 5
      �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5      �� $��0��6   $��
Enter URL:��6   $��12(�6-   $
��Bold"�6;   $��Center=�5      $��	10,10,5,0q�6(   �� 5   
   -   5         ��Name$ 
urlTextBox��5      $��0��6&   $��1��63   $��	5,10,10,0q�6C   $	��300��5      �� 5   
   $��2��6    �� Orientation$ 
Horizontal=�6"   $��Center=�6;   $	��Top=�5      $
��0,10q�6%   .+�5      �� -   5        $��Download��5      $��5q�6E   �� 5      -   5        $��Cancel��5      $��5q�6'   63   � 5      -   5         �Name$ progressBar��5      $	��200��6/   $��20��6;   $��Center=�5      $��5q�6,   5      �� 5      $��2��6   $ Vertical=�6   $
��Left=�63   $��Bottom=�5      $��
0,40,0,-48q�6(   $	��154��6<   .+�5      �� -   5         ��Name$ urlTextBlock��5      $��URL: ��6%   $
��Bold"�62   �� 5       -   5        $ statusTextBlock��5       $��Status: ��6(   $
��Bold"�68   �� 5!      -   5        $ fileSizeTextBlock��5!      $��File Size: ��6*   $
��Bold"�6=   �� 5"      -   5        $ percentageTextBlock��5"      $��Percentage: ��6,   $
��Bold"�6@   �� 5#      -	   5        $ transferRateTextBlock��5#      $��Transfer Rate: ��6.   $
��Bold"�6E   �� 5$      -
   5        $ timeLeftTextBlock��5$      $��Time Left: ��6*   $
��Bold"�6=   5%      �� 5&      $��2��6   $ 
Horizontal=�6   $
��Left=�65   $��Bottom=�5'      $��215,0,0,-20q�6(   .+�5(      �� -   5        $��pause��5(      $��5q�66   �� 5)      -   5        $��Resume��5)      $��5q�68   5+      58      59      