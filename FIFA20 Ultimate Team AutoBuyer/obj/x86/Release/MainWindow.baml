   M S B A M L   `   `   `  ���� "  FIFA20 Ultimate Team AutoBuyer/    )FIFA20_Ultimate_Team_AutoBuyer.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -     � Title$$  FIFA20 Ultimate Team Autobuyer��$	��610��$	��400�� � 
ResizeMode$ CanMinimize=�.���� $
��Left=�$	��580��$	��Top=�$	��383��.@��� $��Login��.��� $
��Left=�$	��580��$��5,5,-6,0q�$	��Top=�$	��365��.+��� $��Session ID:��$
��Left=�$��0,0,0,0q�$	��Top=�$	��110��$��0q�$��30�����	#FFACACAC�� -    ��Name$ txtSessionID��$��1q�$��Center=�$
��Left=�$��	115,0,0,0q�$	��Top=�$	��250��$��30�����	#FFACACAC{��� *EnableFields+���� *	SessionID+�� -    ��Name$ 	btnStart2��$��	0,495,0,0q�$	��365��$	��top=�$
��Left=�$��38�����	#FFACACAC���� *StartButtonColour+{��� *EnableAddButton+���� *ConnectButton+�� $��Filters��.��� $
��Left=�$	��580��$��5,5,-6,0q�$	��Top=�$	��365��.+��� $��Right=��� -   .�Load Filter �� -   .�Save Filter&   System.Windows.Controls.DataGrid  -     Name$ DataGridPlayers1��$
��Left=�   GridLinesVisibility$
 None=�   AutoGenerateColumns .     RowHeaderWidth$ 0Q�$��1q�   CanUserDeleteRows .  	   CanUserAddRows	 .  
   
IsReadOnly
 . $	��100��$��0,20,0,0q�$	��Top=�$	��365������ *MarketplaceItems+   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$
 Type��   Width5  /System.Windows.Controls.DataGridLengthConverter$	 100 1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *
ItemType+  $
 Name��$	 100  �� *FriendlyName+   ElementStyle �� .���  �� Property � �� �� Value$
 Wrap=�  $ Position��$	 100  �� *
Position+  $ Quality��$	 100  �� *	Quality+  $ Chemistry Style��$	 100  �� *ChemistryStyle+  $ Rating��$ 50  �� *Rating+  $ 	Min Price��$ 50  �� *
MinPrice+  $ 	Max Price��$ 50  �� *
MaxPrice+4  .System.Windows.Controls.DataGridCheckBoxColumn  $
 Sell��$ 30  �� *Sell+�� -    ��Name$ lblType��$��Type:��$��
0, 130,0,0q�$
��Left=�$	��Top=�$��30���� -   $��cboType��$��Center=� �� SelectedIndex$ 0��$
��Left=� �� 
IsEditable .   �� IsTextSearchEnabled .  $��115,130,0,0q�$	��Top=�$	��250��$��30�� �� Text �� *SelectedType+ �� UpdateSourceTrigger$ PropertyChanged=� �� Mode$ TwoWay=����� *Types+�� -	   $ 	lblPlayer��$��Player Name:��$��	0,165,0,0q�$
��Left=�$	��Top=��� -
   $��	cboPlayer��$��Center=�$
��Left=� .  .  $��115,165,0,0q�$	��Top=�$	��250��$��30�� �� *SelectedPlayer+$ PropertyChanged=�$ TwoWay=����� *	Players+{��� *DisableChemistryStyleFields+�� -   $ lblPosition��$��	Position:��$��	0,200,0,0q�$
��Left=�$	��Top=��� -   $��cboPosition��$��Center=�$
��Left=� .   .  $��115,200,0,0q�$	��Top=�$	��250��$��30�� �� *SelectedPosition+$ PropertyChanged=�$ TwoWay=����� *	Positions+{��� *DisableChemistryStyleFields+ �� *SelectedIndexPosition+�� -   $ 
lblQuality��$��Quality:��$��
0, 235,0,0q�$
��Left=�$	��Top=�$��30��$��50���� -   $��
cboQuality��$��Center=�$
��Left=� .   .  $��115,235,0,0q�$	��Top=�$	��250��$��30�� �� *SelectedQuality+$ PropertyChanged=�$ TwoWay=����� *	Qualities+{��� *DisableChemistryStyleFields+ �� *SelectedIndexQuality+�� -   $ lblChemistryStyle��$��Chemistry Style:��$��
0, 270,0,0q�$
��Left=�$	��Top=�$��30���� -   $��cboChemistryStyle��$��Center=�$
��Left=� .   .  $��115,270,0,0q�$	��Top=�$	��250��$��30�� �� *SelectedChemistryStyle+$ PropertyChanged=�$ TwoWay=����� *ChemistryStyles+ �� *SelectedIndexChemistryStyle+�� -   $ 	lblRating��$��Rating:��$��
0, 305,0,0q�$
��Left=�$	��Top=�$��30���� -   $ 	txtRating��$��1q�$��Center=�$
��Left=�$��115,305,0,0q�$	��Top=�$	��250��$��30�����	#FFACACAC���� *PlayerRating+{��� *DisableRating+$ PropertyChanged=��� -   $ lblMinPrice��$��
Min Price:��$��
0, 340,0,0q�$
��Left=�$	��Top=�$��30���� -   $ txtMinPrice��$��1q�$��Center=�$
��Left=�$��115,340,0,0q�$	��Top=�$	��250��$��30�����	#FFACACAC���� *PlayerMinPrice+{��� *DisableMinPrice+$ PropertyChanged=��� -   $ lblMaxPrice��$��
Max Price:��$��
0, 375,0,0q�$
��Left=�$	��Top=�$��30���� -   $ txtMaxPrice��$��1q�$��Center=�$
��Left=�$��115,375,0,0q�$	��Top=�$	��250��$��30�����	#FFACACAC���� *PlayerMaxPrice+{��� *DisableMaxPrice+$ PropertyChanged=��� -   $ lblSellItem��$��
Sell Item:��$��
0, 410,0,0q�$
��Left=�$	��Top=�$��30���� -   $��cboSellItem��$��Center=�$
��Left=�$ 0�� .   .  $��115,410,0,0q�$	��Top=�$	��250��$��30�� �� *SelectedSellItem+$ PropertyChanged=�$ TwoWay=����� *
SellItem+�� -   $ btnAdd�����	#FFACACAC$	��Add��$��	0,460,0,0q�$	��175��$
��Left=�$	��Top=�$��30��{��� *EnableAddButton+$ PropertyChanged=��� -   $ 	btnRemove�����	#FFACACAC$��Remove��$��185,460,0,0q�$	��180��$��30��$
��Left=�$	��Top=�{��� *EnableAddButton+$ PropertyChanged=��� -   $ 	btnStart1��$��	0,495,0,0q�$	��365��$	��top=�$
��Left=�$��38�����	#FFACACAC���� *StartButtonColour+{��� *EnableAddButton+���� *ConnectButton+�� $��Stats��.��� $
��Left=�$	��580��$��5,5,-6,0q�$	��Top=�$	��365��.+��� -   $ lblStartingCreditsTitle��$��Starting Credits:��$��0, 0,0,0q�$
��Left=�$	��Top=�$��30��$	��110���� -   $ lblStartingCredits��$��	115,0,0,0q�$
��Left=�$��30��$	��Top=�$	��250��$��1q����	#FFACACAC���� *StartingCredits+$ TwoWay=� �� NotifyOnTargetUpdated . �� -   $ lblCurrentCreditsTitle��$��Current Credits:��$��	0, 35,0,0q�$
��Left=�$	��Top=�$��30��$	��110���� -   $ lblCurrentCredits��$
��Left=�$��30��$��
115,35,0,0q�$	��Top=�$	��250��$��1q����	#FFACACAC���� *CurrentCredits+�� -    $ lblAssetsTitle��$��Current Assets:��$��	0, 70,0,0q�$
��Left=�$	��Top=�$��30��$	��110���� -!   $ 	lblAssets��$
��Left=�$��30��$��
115,70,0,0q�$	��Top=�$	��250��$��1q����	#FFACACAC���� *Assets+�� -"   $ lblTotalCreditsTitle��$��Total Credits:��$��
0, 105,0,0q�$
��Left=�$	��Top=�$��30��$	��110���� -#   $ lblTotalCredits��$
��Left=�$��30��$��115,105,0,0q�$	��Top=�$	��250��$��1q����	#FFACACAC���� *Total+�� -$   $ lblProfitTitle��$��Total Profit:��$��	0,140,0,0q�$
��Left=�$	��Top=�$��30��$	��110���� -%   $ 	lblProfit��$
��Left=�$��30��$��115,140,0,0q�$	��Top=�$��1q�$	��250�����	#FFACACAC���� *Profit+�� -&   $ btnStart��$��	0,495,0,0q�$	��365��$	��top=�$
��Left=�$��38�����	#FFACACAC���� *StartButtonColour+{��� *EnableAddButton+���� *ConnectButton+�� $	��Log��.��� $
��Left=�$	��540��$��5,5,0,0q�$	��Top=�$	��365��.+�  $
��Left=�$
 None=� .  $ 0Q�$��1q� .  	 .  
 .    ColumnWidth$ * $	��490��$��0,0,0,0q�$	��Top=�$	��365������ *Log+   $
 Time��$ 60  �� *Time+  $ Message�� �� *	Message+ �� .���  � ��$
 Wrap=��� -'   $ 	btnStart3��$��	0,495,0,0q�$	��365��$	��top=�$
��Left=�$��38�����	#FFACACAC���� *StartButtonColour+{��� *EnableAddButton+���� *ConnectButton+�� $��	Tradepile��.��� $
��Left=�$	��540��$��5,5,0,0q�$	��Top=�$	��365��.+�  -(   $ DataGridTradepile��$
��Left=�$
 None=� .  $ 0Q�$��1q� .  	 .  
 . $	��490��$��0,0,0,0q�$	��Top=�$	��365������ *	Tradepile+   $
 Type��$ 70  �� *Type+  $
 Name��$ 90  �� *Name+  $ Rating��$ 50  �� *Rating+  $ Buy Now Price��$ 90  �� *BuyNowPrice+  $ Status��$ 75  �� *	Updates+�� -)   $ 	btnStart4��$��	0,495,0,0q�$	��365��$	��top=�$
��Left=�$��38�����	#FFACACAC���� *StartButtonColour+{��� *EnableAddButton+���� *ConnectButton+�� $��Settings��.��� $
��Left=�$	��400��$��5,5,0,0q�$	��Top=�$	��365��.+��� $��
Sell Price��$��5,5,0,0q�$��30��$��Center=�$	��110��$
��Left=�$	��Top=��� -*   $��cboSellPrice��$ 0��$��Center=�$
��Left=�$��	115,5,0,0q�$	��Top=�$	��250��$��30�� �� *SelectedSellBin+$ PropertyChanged=�$ TwoWay=����� *SellPriceBin+�� $��Listing Duration��$��
5, 40, 0,0q�$��30��$��Center=�$	��110��$
��Left=�$	��Top=��� -+   $��cboDuration��$ 0��$��Center=�$
��Left=� �� SelectedValuePath$
 Name�� �� DisplayMemberPath$
 Name��$��115, 40, 0,0q�$	��Top=�$	��250��$��30�� �� *
Duration+$ PropertyChanged=�$ TwoWay=����� *	Durations+�� $��Min Profit Margin��$��
5, 75, 0,0q�$��30��$��Center=�$	��110��$
��Left=�$	��Top=��� -,   $��cboMinProfitMargin��$��Center=�$
��Left=�$��115, 75, 0,0q�$	��Top=�$	��250��$��30��$ 3�� �� *MinProfitMargin+$ PropertyChanged=�$ TwoWay=����� *MinProfitMarginList+