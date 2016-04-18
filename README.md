# Glorten's Dungeon

This repository includes the Unity 3D project to build the [Twinsprite Glorten's Dungeon demo APP](http://twinsprite.com/#demo). A dungeon crawler minigame with a final boss RPG battle. It illustrates the interaction of physical objects with mobile games via the Twinsprite platform. Objects are uniquely identified by a code stored in a QR or NFC tag that works as a key to the object data store in the Twinsprite backend.

## See It in Action

### Play Now

1. Download the Glorten's Dungeon App from [Google play](https://play.google.com/store/apps/details?id=com.twinsprite.dungeondemokit) or the [AppStore](https://itunes.apple.com/us/app/glortens-adventure/id953747151).

2. Generate a Glorten's Papercraft on the [Twinsprite page](http://twinsprite.com/#demo)

3. Scan the QR code in the papercraft to play. The character gold, items and experience will be stored in the Twinsprite Platform and linked to unique code in the QR.

To defeat the final boss you will need some help, the [Twinsprite page](http://twinsprite.com/#demo)
 provides codes to unlock a couple of special attacks.

### NFC

If you own a NFC capable Android device, all the codes generated from the [Twinsprite page](http://twinsprite.com/#demo) can be stored in a NFC tag and tested in the Glorten's Dungeon APP. Tags with at least 144 bytes of storage capacity are required, the code have to be stored using [NDEF](http://members.nfc-forum.org/specs/spec_list/#ndefts) format. You can use the [NXP TagWriter App](https://play.google.com/store/apps/details?id=com.nxp.nfc.tagwriter), it provides a function to write NFC tags directly from QR codes.

## Code Overview

### Scenes
The game includes three scenes under the [/Assets/Scenes](https://github.com/twisprite-developers/glortens-dungeon/tree/master/Assets/Scenes) folder.

#### menu
The game main menu screen. These are the main elements in the scene: 

* <b>MainMenu</b> this Game Object contols the SCAN QR, SCAN NFC and PLAY buttons. The main logic is implemented in the [/Assets/Scripts/UI/MainMenu.cs](https://github.com/twisprite-developers/glortens-dungeon/blob/master/Assets/Scripts/UI/MainMenu.cs) script.

* <b>ToyxManager</b> manages all the interactions between the unique identity attached to a physical object and the Twinsprite backend. It also includes an Editor to test the Twinsprite SDK.

#### game 
The dungeon scenario where you have to defeat the skeletons to get gold and find the Glorten's axe. These are the main elements in the scene:

* <b>Dungeon/Characters/Player</b> is the Glorten's Game Object, the character controls are implemented using the Unity free plugin [CN Controls (Jostic + Touchpad components)](https://www.assetstore.unity3d.com/en/#!/content/15233), the main logic is in [/Assets/Scripts/Player/Player.cs](https://github.com/twisprite-developers/glortens-dungeon/blob/master/Assets/Scripts/Player/Player.cs), including the ToyxManager calls to update the character attributes on the Twinsprite backend.

* <b>Dungeon/Characters/EnemySpawner</b> produces the multiple skeletons in the scenario, the enemy spawner logic is implemented in [/Assets/Scripts/Enemy/EnemySpawner.cs](https://github.com/twisprite-developers/glortens-dungeon/blob/master/Assets/Scripts/Enemy/EnemySpawner.cs).

* <b>Dungeon/Pickups/axe_roll</b> Glorten's axe.

#### boss
Final boss battle.

* <b>Dungeon/Characters/Glorten</b> and <b>Dungeon/Characters/Boss</b> are the characters' Game Objects. All the battle logic is included under the [/Assets/Scripts/BossFight](https://github.com/twisprite-developers/glortens-dungeon/tree/master/Assets/Scripts/BossFight) folder.

### Twinsprite Toyx Manager

The [/Assets/Scripts/TwinspriteSDK](https://github.com/twisprite-developers/glortens-dungeon/tree/master/Assets/Scripts/TwinspriteSDK) folder contains the   [ToyxManager.cs](https://github.com/twisprite-developers/glortens-dungeon/tree/master/Assets/Scripts/TwinspriteSDK) utility script that wraps all the calls to the Twinsprite backed. There is also an Editor that simplifies your Toyx testing tasks.

In the Glorten's Dungeon project this script is attached to the ToyxManager Game Object included in the menu scene.

![image](http://developers.twinsprite.com/images/ToyxManager-editor.png)

By default the ToyxManager has the API_KEY and SECRET_KEY to play with the Toyx codes generated from the [Twinsprite page](http://twinsprite.com/#demo) so you can fill the Toyx Id field with the code of a Glorten's papercraft QR removing the starting url string https://s2d.io/.

### Creating a Customized version of Glorten's Dungeon

This section explains how to make the game point to your own Twinsprite account.

1. If you don't have a [Twinsprite Development Portal](http://devportal.twinsprite.com/) account, this is the [register form](http://devportal.twinsprite.com/register).

2. In the Glortn's Dungeon Unity 3D project, open the /Assets/Scenes/menu scene and select the toyxManager game object, on the Inspector area you will see the Twinsprite ToyxManager Editor with the default game key-pair.

3. Go to the [Twinsprite Development Portal](http://devportal.twinsprite.com/), under the Developer Perspective, select  "GAME > My Games", you will find the Glorten's Dungeon game. It contains the API_KEY and SECRET_KEY values you have to include in the ToyxManager Editor.

4. On the [Twinsprite Development Portal](http://devportal.twinsprite.com/), under the Developer Perspective, select "DEVELOPMENT TOYX > Create new Toyx", provide a Batch Name and create a set of Development Toyx. Now copy a Toyx ID code and use it on the ToyxManager Editor.

5. On Unity 3D run the menu scene and start making requests from the ToyxManager Editor.

You can find more info about creating Games and Toyx in the [Twinsprite Developers Quick Start](http://developers.twinsprite.com/v2/docs/pages/quickstart.html) page.

## Resources

[Twinspite Page](http://twinsprite.com)

[Twinsprite Developer Site](http://developers.twinsprite.com/)

[Twinsprite Developer Documentation Site](http://developers.twinsprite.com/v2/docs/pages/overview.html)

## License

Glorten's Dungeon source code has been developed by [Carbonbyte Studios](http://www.carbonbyte.com/) for Twinsprite and is available under the [MIT](https://opensource.org/licenses/MIT) license, excluding some third party plugins. The project art has been authored by [Juanma Zarza](https://twitter.com/mrkoala) for Twinsprite and is available under the [Creative Commons Attribution (CC BY 4.0)](https://creativecommons.org/licenses/by/4.0/) license.

The license details are included in the [LICENSE](https://github.com/twisprite-developers/glortens-dungeon/blob/master/LICENSE) file.

### Third Party Resources

[CN Controls](https://www.assetstore.unity3d.com/en/#!/content/15233) (by Cyrill Nadezhdin): Available under the [MIT](https://opensource.org/licenses/MIT) license.

[GoKit](https://github.com/prime31/GoKit) (by prime[31]): custon license, included bellow.

For any developers just wanting to use GoKit in their games go right ahead. 
You can use GoKit in any and all games either modified or unmodified. In order 
to keep the spirit of this open source project it is expressly forbid to sell 
or commercially distribute GoKit outside of your games. You can freely use it 
in as many games as you would like but you cannot commercially distribute the 
source code either directly or compiled into a library outside of your game.

[Almendra Font](https://www.google.com/fonts/specimen/Almendra) (by Ana Sanfelippo): available under [SIL Open Font License 1.1](http://scripts.sil.org/OFL)

Boss fight ambient music has been composed by [Albert "hellmer" Gallego](https://soundcloud.com/hell_mer) and is available under  [Creative Commons 
Attribution-NoDerivatives (CC BY-ND 4.0)](http://creativecommons.org/licenses/by-nd/4.0/)


![image](http://twinsprite.com/images/arena.png)

