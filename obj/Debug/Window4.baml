   M S B A M L   `   `   `  ����R  NInternetdownloadmanager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:InternetdownloadmanagerInternetdownloadmanager  5   	   %    Internetdownloadmanager.Window4   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7local%clr-namespace:Internetdownloadmanager     5   	     � Title$  Multiple Downloads��5   	   $	��280��6$   $	��550��61   ���#F0F4F8 � Icon$ /assets/logo.jpg��5	       � WindowStartupLocation$ CenterScreen=�66   .��5
      � $��0,0,0,50q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $��
Enter URL:��6!   $��12(�63   $
��Bold"�6A   $��Center=�6S   $��	10,10,5,0q�6n   �� 5   
   -   5         ��Name$ 
urlTextBox��5      $��0��6&   $��1��63   $��	5,10,10,0q�6C   $	��300��6V   �� 5   
   $��1��6    �� Orientation$ 
Horizontal=�6"   $��Center=�6;   $	��Top=�6X   $
��0,10q�6p   .+�5      �� -   5        $��Download��5      $��5q�6E   �� 5      -   5        $��Cancel��5      $��5q�6A   5      �� 5   
   $��2��6   $��10,5,0,0q�6$   $
��Auto=�66   $
��Auto=�6Y   .��5      �� -   5         ��Name$ downloadsStackPanel��5      $��2��66   $��10,5,0,0q�6C   $ Vertical=�6U   5      5      5      5       