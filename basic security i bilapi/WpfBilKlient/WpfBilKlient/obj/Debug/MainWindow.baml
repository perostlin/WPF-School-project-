   M S B A M L   `   `   `  ����G  CWpfBilKlient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:WpfBilKlientWpfBilKlient  5   	       WpfBilKlient.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:WpfBilKlient     5   	     � Title$  
MainWindow��5   	   $	��350��6   $��682.532��6)   .��5	      � $��0,0,0,-6q�6   .+�5
   
   &   System.Windows.Controls.DataGrid  -   5          Name$ dataGridBilar��5
      $
��Left=�6*   $��	19,14,0,0q�6E   $	��Top=�6X   $	��199��6p   $	��579��6}    J� RenderTransformOrigin$ 0.5,0.5&�6�   $y�	Collapsed=�6�   z�5      V� 5      .~�5      �� �� 5      �� 5       �� Angle$
 0.02Q�6&   T� 5      5      5      �� 5   
   -   5         ��Name$ buttonGetAllCars��5      $��Hämta alla bilar��6+   $
��Left=�6F   $��
52,243,0,0q�6a   $	��Top=�6u   $	��105��6�   6�   �� 5   
   -   5         ��Name$ textBoxSokRegnr��5      $
��Left=�6+   $��23��6F   $��198,243,0,0q�6R    �� TextWrapping$
 Wrap=�6g   $	��Top=�6{   $	��120��6�   $��SKL886��6�   �� 5   
   -   5        $ buttonHamtaBil��5      $��Hämta specifik bil��6)   $
��Left=�6F   $��329,243,0,0q�6a   $	��Top=�6v   $	��109��6�   6�   �� 5   
   -   5        $ buttonClear��5      $��Rensa��6&   $
��Left=�66   $��476,243,0,0q�6Q   $	��Top=�6f   $��75��6~   6�   �� 5   
   
���   �$��1q�6%   $
��Left=�69   $��49��6T   $��179,229,0,0q�6`   $	��Top=�6u   $	��273��6�   �� 5   
   
���   �$��1q�6%   $
��Left=�69   $��49��6T   $��
32,229,0,0q�6`   $	��Top=�6t   $	��142��6�   �� 5   
   
���   �$��1q�6%   $
��Left=�69   $��49��6T   $��457,229,0,0q�6`   $	��Top=�6u   $	��125��6�   �� 5   
   -   5         ��Name$ NewCarPanel��5      $
��Left=�6*   $	��181��6E   $��
312,21,0,0q�6R   $	��Top=�6f   $	��253��6~   .+�5      ��  �� Orientation$ 
Horizontal=�6   .+�5      �� -   5        	 ��Name$	 
labelRegnr��5      $��Reg nr��6,   �� 5      -   5        $ textBoxRegnr��5      $��23��60   $
 Wrap=�6<   $�� ��6P   $	��100��6X   5       �� 5!      $ 
Horizontal=�6   .+�5"      �� -	   5        $	 labelArsmodell��5"      $��
Årsmodell��60   �� 5#      -
   5        $ textBoxArsmodell��5#      $��23��64   $
 Wrap=�6@   $
��2016��6T   $	��100��6`   5$      �� 5%      $ 
Horizontal=�6   .+�5&      �� -   5        $	 labelModell��5&      $��Modellbeteckning��6-   �� 5'      -   5        $ textBoxModell��5'      $��23��61   $
 Wrap=�6=   $�� ��6Q   $	��100��6Y   5(      �� 5)      $ 
Horizontal=�6   .+�5*      �� -   5        $	 
labelMarke��5*      $��Märke��6,   �� 5+      -   5        $ textBoxMarke��5+      $��23��60   $
 Wrap=�6<   $
��Audi��6P   $	��100��6\   5,      �� 5-      $ 
Horizontal=�6   .+�5.      �� -   5        $	 	labelFarg��5.      $��Färg��6+   �� 5/      -   5        $ textBoxFarg��5/      $��23��6/   $
 Wrap=�6;   $��Ragusa��6O   $	��100��6]   50      �� 51      $ 
Horizontal=�6   .+�52      �� -   5        $	 labelMetallic��52      $��Metalliclack��6/   �� 53      -   5        
 ��Name$
 checkBoxMetallic��53      �� 54      -   5        $ buttonNewCar��54      $��Skapa ny bil��6/   $��Right=�6F   6b   55      56      �� 57   
   -   5         ��Name$ newCarBorder��57      
���   �$��1q�6<   $
��Left=�6P   $	��181��6k   $��
312,10,0,0q�6x   $	��Top=�6�   $	��239��6�   59      �� 5:   
   -   5        $ textBoxAddress��5:      $
��Left=�6*   $��23��6E   $��
32,292,0,0q�6Q   $
 Wrap=�6e   $P��Jhttp://microsoft-apiappdba56dcc1d6e4d2e9c0a2c61707dbd1e.azurewebsites.net/��6y   $	��Top=�6�   $	��443��6�   �� 5;   
   -   5        $ buttonLocal��5;      $��Local��6&   $
��Left=�66   $��500,292,0,0q�6Q   $	��Top=�6f   $��75��6~   6�   �� 5<   
   -   5        $ buttonAzure��5<      $��Azure��6&   $
��Left=�66   $��580,291,0,0q�6Q   $	��Top=�6f   $��75��6~   6�   5>      5?      