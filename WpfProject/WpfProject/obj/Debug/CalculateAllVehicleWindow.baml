   M S B A M L   `   `   `  ����E  AWpfProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:WpfProject
WpfProject  5   	   *    $WpfProject.CalculateAllVehicleWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:WpfProject     5   	     � Title$  Räkna ut på alla fordon��5   	   $��393.486��6)   $��496.479��6:   .��5	      � .+�5   
   �� .+�5      �� $��Välj år/månad/dag:��6   $
��Left=�61   $��	10,10,0,0q�6L   $	��Top=�6_   (  "System.Windows.Controls.DatePicker  5      -   5          Name$ dtpYearMonthDay��5      $
��Left=�6�   $��10,5,0,0q�6�   $	��Top=�6�   $��22��6�   $	��120��6�      CalendarStyle 5      �� 5       �� 
TargetType&   System.Windows.Controls.Calendar"  6   .�5      ��  �� Property �@ DisplayMode �� Value$ Decade=�68   5      5      �� 5      $��Vald månad��6   $��15(�6)   $
��Left=�67   $��	10,10,0,0q�6R   $	��Top=�6e   �� 5      $��Snittförbrukningen:��6   $
��Left=�62   $��10,5,0,0q�6M   $	��Top=�6_   �� 5      -   5         ��Name$ tbMonthlyFuelConsumption��5       �� 
IsReadOnly . $
��Left=�6J   $��23��6e   $��10,5,0,0q�6q    �� TextWrapping$
 Wrap=�6�   $	��Top=�6�   $	��120��6�   �� 5      $��Valt år��6   $��15(�6&   $
��Left=�64   $��10,5,0,0q�6O   $	��Top=�6a   �� 5      $��Snittförbrukningen:��6   $
��Left=�62   $��10,5,0,0q�6M   $	��Top=�6_   �� 5      -   5        $ tbYearhlyFuelConsumption��5       . $
��Left=�6J   $��23��6e   $��10,5,0,0q�6q   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   �� 5      $��Sedan tidens början��6   $��15(�62   $
��Left=�6@   $��10,5,0,0q�6[   $	��Top=�6m   �� 5      $��Snittförbrukningen:��6   $
��Left=�62   $��10,5,0,0q�6M   $	��Top=�6_   �� 5      -   5        $  tbBeginningFuelConsumption��5       . $
��Left=�6L   $��23��6g   $��10,5,0,0q�6s   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   5      �� 5   
   .+�5       �� $��Totala bränslekostnaden:��6   $
��Left=�67   $��170,107,0,0q�6R   $	��Top=�6g   �� 5!      -   5        $ tbMonthlyTotalFuelAmount��5!       . $
��Left=�6J   $��23��6e   $��	170,5,0,0q�6q   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   �� 5"      $��Totala bränslekostnaden:��6   $
��Left=�67   $��
170,40,0,0q�6R   $	��Top=�6f   �� 5#      -   5        $ tbYearlyTotalFuelAmount��5#       . $
��Left=�6I   $��23��6d   $��	170,5,0,0q�6p   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   �� 5$      $��Totala bränslekostnaden:��6   $
��Left=�67   $��
170,40,0,0q�6R   $	��Top=�6f   �� 5%      -   5        $  tbBeginningTotalFuelAmount��5%       . $
��Left=�6L   $��23��6g   $��	170,5,0,0q�6s   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   5&      �� 5(   
   .+�5)      �� $��Totala kostnaden:��6   $
��Left=�60   $��350,107,0,0q�6K   $	��Top=�6`   �� 5*      -   5        $ tbMonthlyTotalVehicleCost��5*       . $
��Left=�6K   $��23��6f   $��	350,5,0,0q�6r   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   �� 5+      $��Totala kostnaden:��6   $
��Left=�60   $��
350,40,0,0q�6K   $	��Top=�6_   �� 5,      -	   5        $ tbYearlyTotalVehicleCost��5,       . $
��Left=�6J   $��23��6e   $��	350,5,0,0q�6q   $
 Wrap=�6�   $	��Top=�6�   $	��120��6�   5-      5/      50      