   M S B A M L   `   `   `  ����A  =Tools3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:Tools3Tools3  5          Tools3.ucConexao   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      &localclr-namespace:Tools3     5      $	��400��5      $	��120��5	        � 	Resources  5       �� 
TargetType �� Property �� Value��%�   '��       'G� H     �� 5      " ��6
   .�5      ��  � ��$ 23��6   5      �� 5      " G�6
   .�5      ��  � ��$ 23��6   5      5      .��5      �  � RowDefinitions 5      �� 5      $��* �6   �� 5      $��* �6   .+�5      ��  �� Orientation$ 
Horizontal=�6   $��0��6(   .+�5      �� $ Vertical=�6   $	��100��6'   .+�5      �� $��Lista��6   
�������6   �� 5       �� DisplayMemberPath$
 Nome��5      
���   ���5      �� *
Conexoes+ �� SelectedItem 5      �� *	Conexao+{�5      �� *DesConectado+5       �� 5!      $ Vertical=�6   $	��100��6'   .+�5"      �� $��Servidor��6   
�������6    �� 5#      ��6   �� *Conexao.Server+{�6.   �� *DesConectado+5$      �� 5%      $ Vertical=�6   $	��100��6'   .+�5&      �� $��Database��6   
�������6    �� 5'      ��6   �� *Conexao.Database+{�60   �� *DesConectado+5(      �� 5)      $ Vertical=�6   .+�5*      �� $��Trust��6   
�������6   �� 5+       d� 	IsChecked 6   �� *	IsTrust+	 �� Mode$	 TwoWay=�5,      5.      �� 50      $ 
Horizontal=�6   $��1��6(   .+�51      �� $ Vertical=�6   $	��100��6'   .+�52      �� $��Usuário��6   
�������6   �� 53      {�6   �� *Conexao.NTrust+��63   �� *Conexao.User+54      �� 55      $ Vertical=�6   $	��100��6'   .+�56      �� $��Senha��6   
�������6   G� 57      -   5        {�57      �� *Conexao.NTrust+67   58      59      �� 5:      $ 
Horizontal=�6   $��1��6(   $��Right=�65   .+�5;      �� -   5        $��Conecta��5;      $��30��6   $��80��6M   $��Center=�6[   y�6*   �� *ShowConecta+6v   �� 5<      -   5        $��
Desconecta��5<      $��30��6!   
���  ��$��80��6d   $��Center=�6r   y�6>   �� *ShowDesConecta+6�   5>      5?      5@      