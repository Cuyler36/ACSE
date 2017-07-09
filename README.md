# ACSE - Animal Crossing Save Editor

The GameCube only version can be found [here](https://github.com/Cuyler36/ACSE-OLD/releases/tag/0.4.5).
## Outdated/Incorrect information. Update coming soon.

ACSE is a save editor for the Animal Crossing game series.

## Features
* Player Editing
	* Name
	* Bells
	* Debt
	* Pockets
	* House
* Acre Editing
* Town Editing
	* Items
	* Buildings
* Villager Editing
	* Villagers
	* Personalities
	* Catchphrases
	* Nicknames (Not Implemented)

## Extracting Save Files

### Dolphin Emulator
To extract save files from Dolphin, on the main form, click Tools > Memcard Manager (GC).
![Main Form](http://i.imgur.com/wH5OCQO.png)

Then click on Animal Crossing, and click the Export GCI button.
![Memcard Manager](http://i.imgur.com/t2GBonJ.png)

### Modded GameCube/Wii
Refer to the article here: https://wiki.dolphin-emu.org/index.php?title=Ripping_Games#Retrieving_Game_Saves

## Using the Editor

### Opening your Save File
Click on the File button, then click Open and select your save file!

![Open Save](https://puu.sh/toNcf/998c395bf9.png)

### Using the Main Editor Form
This contains the Players, Patterns, Town Name, and buttons to access the other editors! Simply change the values of the text/combo boxes to edit them.

![Main Editor](https://puu.sh/tIJV9/2db8da73fc.png)

### Using the Acre Editor Form
This form allows you to change your acre tiles! There is a 7x10 grid of acre tiles. To select an acre to place, click the "Check Tile" button, then left click on the desired acre. You can also simply click on one of the entries in the acre list to the right of the acre grid. Once selected, a preview image of the selected acre will appear on the bottom right of the screen, along with it's name and AcreID. To place an acre, click on the "Place Tile" button, then just left click over any acre tile on the grid!

![Acre Editor](https://puu.sh/tIJTI/cb72b4831e.png)

#### Acre Editor Precautions
Placing/Removing specific acres can damage your town. Here is a list of possible things that could break your game:
* Changing the top row of acres (Untested)
* Putting a non A-Acre in the A-Acre row (Also untested, but quite likely)
* Removing a Dump Acre, but not removing the dump in the Town Editor (This WILL cause your game to break, but only until you remove the dump or put a Dump Acre back)
* Placing regular land acres in the Ocean/Border acres, and dropping items in those acres. (The game doesn't have save data for those acres, and could possibly crash. You will definitely lose whatever you place in those acres, though)
* Not including at least one of each type of acre: Post Office, Dump (This will break your game), Nook's Shop Acre, Museum, Wishing Well, Police Station, Tailor's Shop, Lake & Beachfront w/ Dock (Also untested)

### Using the Town Editor Form
The Town Editor allows you to customize your town to the fullest extent. You can place/move/remove items, buildings, trees, rocks, decorative items, etc! To select an item to place, either select it in the combo box on the top, or right click on an item on the map. The combo box text will then change and the ItemID & buried checkbox should update to the right of it. To place an item, just left click on the map where you want the item placed. You can change whether or not the item is buried (minus outdoor objects like trees & buildings) by checking/unchecking the buried checkbox. You can also toggle the acre background display with the "Show Background" checkbox!

![Town Editor](http://i.imgur.com/18YlO3K.png)

#### Town Editor Precautions
As the case was with the Acre Editor, there are specific items you can place/remove that can possibly break your game. Here is a list of said items:
* Dumps (This is confirmed. It will break your game if you place it in a non A-Acre & a non Dump Acre. The Town Editor will confirm with you that you wish to place a dump in an incorrect acre, if you do. Otherwise, no warning will be shown!)
* Placing things in the top rows of the A-Acres that have "Occupied/Unavailable" as the item. (Not tested, I'm not sure what will happen if you change them. Feel free to, and let me know what happens!)
* Removing all instances of a building (Neighbor houses, Nook's Shop, Tailor's Shop, etc. could break your game. This is not certain, but it's definitely possible.)
* Placing multiple buildings/decorative items IS safe, as long as it is not a Dump.

### Using the Villager Editor Form
Using the Villager Editor for is fairly safe. There are a few possible game breaking things that will be discussed in the section below. To change a villager, all you have to do is select a different one in the drop down box! It's that easy, really. If you do change a villager, make sure you add the correct house for them in the Town Editor, though! You can also edit their Personality and Catchphrase!

![Villager Editor](http://i.imgur.com/XylZA3v.png)

#### Village Editor Precautions
There are a few cases where editing Villagers can cause strange results or break your game. The list is as follows:
* Adding any of the Villagers below Punchy (They are corrupted, and can cause your game to reset whenever you enter their acre. I added them in case people wanted to play around with them.)
* Not removing the old villagers house or not adding the new villagers house in the Town Editor (It's important that their house is removed/added. Where else would they live???)
* Changing an empty villager slot (No Villager) to another villager (Not 100% sure this can break your game, but it caused some weird AI glitches in mine. If you add their correct house, things might be fine.)

### Using the Shop Editor Form
This form allows you to edit Tom Nook's shop. It automatically detects which iteration of the shop exists in your town. You can change what is being sold, and how many bells you have spent/received from him.

![Shop Editor](https://puu.sh/tIKoj/e3b0269524.png)

### Using the Pocket Editor Form
Editing your pockets is very safe and probably the easiest part of this editor to grasp. You can place ANY item in your pocket. Items, Buildings, Trees, Rocks, etc. Same as before, right click or change the combo box selection to set the selected item. Left click to place in that spot. This form also contains the Dresser editor (See below section for more info about it.)

![Pocket Editor](https://puu.sh/tIJXx/8fbd037246.png)

#### Inventory/Pocket Editor Precautions
There is only one concern when using this editor.
* If you place furniture into the first spot in the dresser, it will appear on top of the dresser. This is due to how the items are stored. The first item is stored on top of the dresser, so it will cause the "Stacked Furniture Glitch" that many speed runners use to duplicate items.

### Using the House Editor Form
The House Editor is pretty simple as well. The floors are labeled for you. The top section is the "ground layer", where all furniture that goes on the floor should be placed. The section below it is the "top layer" where items that go on top of tables should be placed. This editor works exactly like the Town/Pocket Editors.

![House Editor](http://i.imgur.com/upNkKKz.png)

#### House Editor Precautions
There are only a few precautions:
* Placing outdoor objects (trees/buildings) in your house will not make them show up, and could break your game (although they just disappeared when I did it.)
* Placing furniture on the "top layer" with nothing below them will place them on the floor instead (Won't break your game or anything, but the save editor will notify you every time you try to do it.)

### Changing Settings
In the Settings Form, you can customize a few options that effect the save editor's behavior.
![Settings Form](https://puu.sh/tIK8r/3835737ec3.png)

### Saving Your Changes
Until you click File > Save, none of your changes will be commited. You must do this when you are ready to save your game. It will overwrite your old file, so make a backup if you're experimenting!

![Save File](https://puu.sh/toNoF/0a84b27f2c.png)

## Other Questions
Message me on Reddit (Cuyler_36) or on Github. I will do my best to help you with whatever you may need.

## Helping with the Editor
If you would like to contribute, I would appreciate it. Currently, I need someone to make graphics for the island acres, and possibly also for every acre. This would be nice, as it would give the user a feel of what the acre actually looks like.