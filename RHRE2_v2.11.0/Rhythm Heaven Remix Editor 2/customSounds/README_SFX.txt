This folder is where you can add custom sounds without having to go through the hassle of the json setup.
However, you will be a lot more limited.


The sounds must be placed in a folder inside this directory.
Each folder is a \"game\", and you can have multiple folders.


Each sound will be limited to these factors:
  * 0.5 beats long by default
  * Loaded sound data must be under a megabyte. The editor WILL CRASH if it's too big!
  * You will be able to stretch the cues.
  * You will be able to change the duration of the cues.
  * The cues will not loop. If you want them to loop, you will have to manually add a game using the JSON files.

You can have at most 60 custom games.
If you have too many they WILL NOT show up!

It is advised that you generate JSON versions of the custom games for more flexibility.

You can use data.json files if you need to quickly test databasing.

Supported audio formats are ogg, wav, and mp3.

Optionally, if you put an icon.png in the folder it will use it.