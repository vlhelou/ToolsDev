   M S B A M L   `   `   `  ����A  =Tools3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:Tools3.View.BancoTools3.View.Banco  5      clr-namespace:Tools3Tools3  5      "    Tools3.View.Banco.Dicionario   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      1localclr-namespace:Tools3.View.Banco     5      'Tools3clr-namespace:Tools3     5      $	��400��5
        N� Title$  
Dicionario��5       � 	Resources 5       �� 
TargetType �� Property �� Value��%�  '��       '�� w     '	� �     �� 5      " ��6
   .�5      ��  � ��
 �����6"   �� 5       � ��$ Courier New)�6"   5      �� 5      " ��6
   .�5      ��  � ��
 �   �6"   �� 5       � ��$ Courier New)�6"   5      �� 5      " 	�6
   .�5      ��  � ��
 �����6"   �� 5       � ��$ Courier New)�6"   5      5      .��5      �  � RowDefinitions 5      �� 5      $	��120 �6   �� 5      $��* �6    � ColumnDefinitions 5!      �� 5"      $	��200 �6   �� 5#      $��* �6   .+�5%         Tools3.ucConexao  -   5          Name$ Conexao��5%      $��0��6&   $��0��66   $��2��6C   $
��Left=�6W   R� 5&      -   5         R�Name$ treeView��5&      $��0��5'      $��1��6   $��Stretch=�5(      $��Stretch=�6#   ��5)      �� *Base.Filhos+��5+      �� 5,      	 �� ItemsSource	 6   �� *Filhos+.R�5-      �� ��6   �� *Nome+6)   5.      50      � 51      $��1��6	   $��1��6    52      �� 53      $��20 �6   �� 54      $��70 �6   �� 55      $��* �6   .+�57      �� 
 �� Orientation$
 
Horizontal=�6   $��0��6)   .+�58      �� $��Tipo=��6   �� 59      ��6   �� *Selecionado.Tipo+�� 5:      $�� Nome=��6   �� 5;      ��6   �� *Selecionado.NomeSql+5<      �� 5=      $
 
Horizontal=�6   $��1��6)   .+�5>      �� $
 Vertical=�6   $��Center=�6(   $��60��6C   .+�5?      �� Campo6   �� 5@      ��6   �� *Campo+ �� Mode$ TwoWay=�5A      �� 5B      $
 Vertical=�6   $��Center=�6(   $��60��6C   .+�5C      �� Valor6   �� 5D      ��6   �� *Valor+$ TwoWay=�5E      �� 5F      $��Center=�6   $��30��6(   $��Right=�64   $��Grava��6y   ��6P   �� *GravaDicionarioClick+6y   5G      &   System.Windows.Controls.DataGrid  5H      $��Stretch=�5I      $��Stretch=�6#   $��2��6?      CanUserAddRows .  ��5J      �� *
Dicionario+ �� SelectedItem 5K      �� *DicionarioSelecionado+$ TwoWay=� J� InputBindings 5M      �� 5N      
 �� Key$ Delete��6    �� Command 6   �� *ApagaDicionarioClick+5P      5Q      5T      5U      