   M S B A M L   `   `   `  ����E  AWpfProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:WpfProject
WpfProject  5   	   4    .WpfProject.CalculateChauffeurConsumptionWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:WpfProject     5   	     � Title$  Din förbrukning��5   	   $��515.247��6!   $��651.606��62   .��5	      � .+�5   
   �� $
��Left=�6   .+�5      �� $��Välj år/månad/dag:��6   $
��Left=�61   $��	10,10,0,0q�6L   $	��Top=�6_   (  "System.Windows.Controls.DatePicker  5      -   5          Name$ dtpYearMonthDay��5      $
��Left=�6�   $��10,5,0,0q�6�   $	��Top=�6�   $��22��6�   $	��120��6�      CalendarStyle 5      �� 5       �� 
TargetType&   System.Windows.Controls.Calendar"  6   .�5      ��  �� Property �@ DisplayMode �� Value$ Decade=�68   5      5      �� 5      $��Inloggad chaufför��6   $��17(�60   $
��Left=�6>   $��10,5,0,0q�6Y   $	��Top=�6k   �� 5      $��Senaste körpasset��6   $��15(�60   $
��Left=�6>   $��10,5,0,0q�6Y   $	��Top=�6k   �� 5      $��Medelförbrukningen:��6   $
��Left=�62   $��10,5,0,0q�6M   $	��Top=�6_   �� 5      -   5         ��Name$ tbLatestFuelConsumption��5       �� 
IsReadOnly . $
��Left=�6I   $��23��6d   $��10,5,0,0q�6p    �� TextWrapping$
 Wrap=�6�   $	��Top=�6�   $	��120��6�   �� 5      $��Vald månad��6   $��15(�6)   $
��Left=�67   $��10,5,0,0q�6R   $	��Top=�6d   �� 5      $��Medelförbrukningen:��6   $
��Left=�62   $��10,5,0,0q�6M   $	��Top=�6_   �� 5      -   5        $ tbMonthlyFuelConsumption��5       . $
��Left=�6J   $��23��6e   $��10,5,0,0q�6q   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   �� 5      $��Vald år��6   $��15(�6&   $
��Left=�64   $��10,5,0,0q�6O   $	��Top=�6a   �� 5      $��Medelförbrukningen:��6   $
��Left=�62   $��10,5,0,0q�6M   $	��Top=�6_   �� 5      -   5        $ tbYearhlyFuelConsumption��5       . $
��Left=�6J   $��23��6e   $��10,5,0,0q�6q   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   �� 5      $��Sedan tidens början��6   $��15(�62   $
��Left=�6@   $��10,5,0,0q�6[   $	��Top=�6m   �� 5      $��Medelförbrukningen:��6   $
��Left=�62   $��10,5,0,0q�6M   $	��Top=�6_   �� 5       -   5        $  tbBeginningFuelConsumption��5        . $
��Left=�6L   $��23��6g   $��10,5,0,0q�6s   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   5!      �� 5#   
   $
��Left=�6   $��
0,0,0,-0.4q�60   $	��307��6D   .+�5$      �� $��Medel bränslekostnaden:��6   $
��Left=�66   $��170,140,0,0q�6Q   $	��Top=�6f   �� 5%      -   5        $ tbLatestTotalFuelAmount��5%       . $
��Left=�6I   $��23��6d   $��	170,5,0,0q�6p   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   �� 5&      $��Medel bränslekostnaden:��6   $
��Left=�66   $��
170,40,0,0q�6Q   $	��Top=�6e   �� 5'      -   5        $ tbMonthlyTotalFuelAmount��5'       . $
��Left=�6J   $��23��6e   $��	170,5,0,0q�6q   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   �� 5(      $��Medel bränslekostnaden:��6   $
��Left=�66   $��
170,40,0,0q�6Q   $	��Top=�6e   �� 5)      -   5        $ tbYearlyTotalFuelAmount��5)       . $
��Left=�6I   $��23��6d   $��	170,5,0,0q�6p   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   �� 5*      $��Medel bränslekostnaden:��6   $
��Left=�66   $��
170,40,0,0q�6Q   $	��Top=�6e   �� 5+      -	   5        $  tbBeginningTotalFuelAmount��5+       . $
��Left=�6L   $��23��6g   $��	170,5,0,0q�6s   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   5,      �� 5.   
   	 �� Orientation$	 Vertical=�6   $��Center=�6,   $
��20px��6I   .+�5/      �� $��0,20,0,0q�6   $��Center=�6*   
���   �$	��450��6T   $��2��6a   50      �� 52   
   $
��Left=�6   .+�53      �� $��Välj fordon:��6   $
��Left=�6+   $��
340,10,0,0q�6F   $	��Top=�6Z   �� 54      -
   5        
 ��Name$
 
cmbVehicle��54       �� DisplayMemberPath$ RegNo��6+   $
��Left=�6E   $��	340,5,0,0q�6`   $	��Top=�6s   $	��120��6�   6�   �� 55      $��Valt fordon��6   $��17(�6*   $
��Left=�68   $��	340,5,0,0q�6S   $	��Top=�6f   �� 56      $��Snittförbrukningen:��6   $
��Left=�62   $��
340,40,0,0q�6M   $	��Top=�6a   �� 57      -   5        $$ tbLatestVehicleFuelConsumption��57       . $
��Left=�6P   $��23��6k   $��	340,5,0,0q�6w   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   �� 58      $��Snittförbrukningen:��6   $
��Left=�62   $��
340,40,0,0q�6M   $	��Top=�6a   �� 59      -   5        $% tbMonthlyAverageFuelConsumption��59       . $
��Left=�6Q   $��23��6l   $��	340,5,0,0q�6x   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   �� 5:      $��Snittförbrukningen:��6   $
��Left=�62   $��
340,40,0,0q�6M   $	��Top=�6a   �� 5;      -   5        $$ tbYearlyAverageFuelConsumption��5;       . $
��Left=�6P   $��23��6k   $��	340,5,0,0q�6w   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   �� 5<      $��Snittförbrukningen:��6   $
��Left=�62   $��
340,40,0,0q�6M   $	��Top=�6a   �� 5=      -   5        $' !tbBeginningAverageFuelConsumption��5=       . $
��Left=�6S   $��23��6n   $��	340,5,0,0q�6z   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   5>      �� 5@   
   $
��Left=�6   .+�5A      �� $��Välj år/månad/dag:��6   $
��Left=�61   $��
500,10,0,0q�6L   $	��Top=�6`     5B      -   5        $ dtpVehicleYearMonthDay��5B      $
��Left=�6p   $��	500,5,0,0q�6�   $	��Top=�6�   $��22��6�   $	��120��6�    5C      �� 5D      "  6   .�5E      ��  �@ DisplayMode$ Decade=�68   5F      5H      �� 5I      $��Totala bränslekostnaden:��6   $
��Left=�67   $��
500,77,0,0q�6R   $	��Top=�6f   �� 5J      -   5        $$ tbLatestVehicleTotalFuelAmount��5J       . $
��Left=�6P   $��23��6k   $��	500,5,0,0q�6w   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   �� 5K      $��Totala bränslekostnaden:��6   $
��Left=�67   $��
500,40,0,0q�6R   $	��Top=�6f   �� 5L      -   5        $% tbMonthlyAverageTotalFuelAmount��5L       . $
��Left=�6Q   $��23��6l   $��	500,5,0,0q�6x   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   �� 5M      $��Totala bränslekostnaden:��6   $
��Left=�67   $��
500,40,0,0q�6R   $	��Top=�6f   �� 5N      -   5        $$ tbYearlyAverageTotalFuelAmount��5N       . $
��Left=�6P   $��23��6k   $��	500,5,0,0q�6w   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   �� 5O      $��Totala bränslekostnaden:��6   $
��Left=�67   $��
500,40,0,0q�6R   $	��Top=�6f   �� 5P      -   5        $' !tbBeginningAverageTotalFuelAmount��5P       . $
��Left=�6S   $��23��6n   $��	500,5,0,0q�6z   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   5Q      5S      5T      