   M S B A M L   `   `   `  ����E  AWpfProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:WpfProject
WpfProject  5   	   !    WpfProject.AddDriverJournal   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:WpfProject     5   	     � Title$  Lägg till körjournal��5   	   $��363.285��6&   $��591.089��67   .��5	      � $��0,0,3.6,0.4q�6   .+�5   
   �� .+�5      �� $��Datum: *��6   $
��Left=�6'   $��	10,10,0,0q�6B   $	��Top=�6U    J� RenderTransformOrigin$ 0.469,0.469&�6m   (  "System.Windows.Controls.DatePicker  5      -   5          Name$ dtpDate��5      $
��Left=�6*   $��10,5,0,0q�6E   $	��Top=�6W   �� 5      $��	Fordon: *��6   $
��Left=�6(   $��10,5,0,0q�6C   $	��Top=�6U   �� 5      -   5         ��Name$ 
cmbVehicle��5       �� DisplayMemberPath$ RegNo��6+   $
��Left=�6E   $��10,5,0,0q�6`   $	��Top=�6r   $	��120��6�   6�   �� 5      $��Chafför: *��6   $
��Left=�6)   $��10,5,0,0q�6D   $	��Top=�6V   �� 5      -   5        $ cmbUser��5      $ Username��6(   $
��Left=�6E   $��10,5,0,0q�6`   $	��Top=�6r   $	��120��6�   �� 5      $��Bränsletyp: *��6   $
��Left=�6,   $��10,5,0,0q�6G   $	��Top=�6Y   �� 5      -   5        $ cmbFuelType��5      $
 Type��6,   $
��Left=�6E   $��10,5,0,0q�6`   $	��Top=�6r   $	��120��6�   6�   �� 5      $��Mätarställning: *��6   $
��Left=�60   $��10,5,0,0q�6K   $	��Top=�6]   �� 5      -   5         ��Name$ tbMilage��5      $
��Left=�6(   $��23��6C   $��10,5,0,0q�6O    �� TextWrapping$
 Wrap=�6a   $	��Top=�6v   $	��120��6�   5      �� 5   
    �� Orientation$ 
Horizontal=�6   .+�5      �� $��Enhetspris: *��6   $
��Left=�6,   $��170,245,0,0q�6G   $	��Top=�6\   �� 5      -   5        $ tbUnitPrice��5      $
��Left=�6+   $��23��6F   $��-70,272,0,0q�6R   $
 Wrap=�6g   $	��Top=�6{   $	��120��6�   6�   �� 5      $��	Mängd: *��6   $
��Left=�6'   $��
15,245,0,0q�6B   $	��Top=�6V   �� 5      -   5        $ tbFuelAmount��5      $
��Left=�6,   $��23��6G   $��-55,272,0,0q�6S   $
 Wrap=�6h   $	��Top=�6}   $	��120��6�   6�   �� 5      $��Totalpris: *��6   $
��Left=�6+   $��
15,245,0,0q�6F   $	��Top=�6Z   �� 5      -   5        $ tbTotalPrice��5       �� 
IsReadOnly . $
��Left=�6>   $��23��6Y   $��-60,270,0,0q�6e   $
 Wrap=�6z   $	��Top=�6�   $	��120��6�   �� 5      -	   5        	 ��Name$	 btnAddDriverJournal��5      $��
Lägg till��62   $
��Left=�6F   $��-120,305,0,0q�6a   $	��Top=�6w   $	��120��6�   6�   �� 5       -
   5        
 ��Name$
 lbCurrentMilage��5       $
��Left=�6-   $��	8,298,0,0q�6H   $	��Top=�6[   5!      5#      5$      