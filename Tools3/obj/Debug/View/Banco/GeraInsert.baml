   M S B A M L   `   `   `  ����A  =Tools3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:Tools3.View.BancoTools3.View.Banco  5      clr-namespace:Tools3Tools3  5      "    Tools3.View.Banco.GeraInsert   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      1localclr-namespace:Tools3.View.Banco     5      'Tools3clr-namespace:Tools3     5        N� Title$  
GeraInsert��5
       � 	Resources 5       �� 
TargetType �� Property �� Value��%�  '��       '�� w     '	� �     �� 5      " ��6
   .�5      ��  � ��
 �����6"   �� 5       � ��$ Courier New)�6"   5      �� 5      " ��6
   .�5      ��  � ��
 �   �6"   �� 5       � ��$ Courier New)�6"   5      �� 5      " 	�6
   .�5      ��  � ��
 �����6"   �� 5       � ��$ Courier New)�6"   5      5      .��5      �  � RowDefinitions 5      �� 5      $��30 �6   �� 5      $	��150 �6   �� 5      $��* �6    � ColumnDefinitions 5!      �� 5"      $��* �6   �� 5#      $��* �6   .+�5%      ��  �� Orientation$ 
Horizontal=�6   $��0��6(   $��1��65   $
��Left=�6E   .+�5&      �� �� 6   $�Tabela para insert:��6   62   �� 5'      $	��150��6   ��6   �� *TabelaSaida+5)      �� 5*      $ 
Horizontal=�6   $��0��6(   $��1��65   $��Right=�6E   .+�5+      �� $��Center=�6   $��Executa��6'   ��69   �� *ExecutaClick+5,      �� 5-      $��1��6   $��1��6   $��0,0,0,0q�6.   $��Visible=�6@   $��Visible=�6h   .��5.      �� ._�5/      �� 
���   � �� AcceptsReturn . ��66   �� *Sql+	 �� Mode$	 TwoWay=�50      51         Tools3.ucConexao  52      -   5        
  Name$
 Conexao��52      $��1��6'   $	��Top=�65   &   System.Windows.Controls.DataGrid  53      $��2��6   $��2��6   ��6.   �� *	Resultado+54      55      