https://github.com/chrislo27/RhythmHeavenRemixEditor2

(If you'd like to view this online, go to https://github.com/chrislo27/RhythmHeavenRemixEditor2/wiki/The-README)

Welcome to Rhythm Heaven Remix Editor 2!
This is a remake of the original RHRE (referred to as RHRE0).

STARTING THE PROGRAM:
You NEED Java 1.8 (referred to as Java 8).

If you're on Windows, simply double click the run_windows.bat file.
Otherwise, run the jar file (double-click or right click > Run).
The second file (doNotLazyLoad) forces the game to load all the sounds at the start, rather than when you need it. It will have a MUCH LONGER loading time, but it may stop stutters when you first drag sounds in.

On macOS or Linux, use the two shell files provided.

Otherwise, use Terminal and type:
java -jar RHRE.jar

REPORTING AN ISSUE:
When reporting an issue, make sure to include a log file (find it in the logs folder in the directory). If you do not fill out the issue form correctly, I will most likely reject it for being invalid. Just saying "it crashes" is no help at all, and I generally will be less than amused.
https://github.com/chrislo27/RhythmHeavenRemixEditor2/issues

USE IN YOUTUBE VIDEOS:
Yes, you can use this in your YouTube videos, monetized or not.
YOU SHOULD AT LEAST link the GitHub page at the top of this file in the description, though.

SAVING AND LOADING:
You should generally save as a .brhre2 file (bundled file). This means that the music is included in the file so you don't have to reload it each time. If you only want the data file, save it as a .rhre2 file.

CUSTOM SFX:
Custom SFX can be used easily. Boot the editor up to the editor screen at least once, and a folder named customSounds will be made. Follow the README_SFX.txt inside.

RECORDING:
To record your remixes, you should use a program like Audacity and record the Stereo Mix (on Windows) output, or use recording software like OBS.

LICENSE:
This software is licensed under the GNU GPL-3.0 license. See the LICENSE.txt included for more detailed, or go to https://choosealicense.com/licenses/gpl-3.0/.

TIPS AND TRICKS:
There's a silence cue under Extra > Count-Ins. You can use it to lengthen your remix (the remix plays up to the last cue) to fit the music in.
Inspections help you prevent glaring errors, like offbeat Lockstep cues when it's supposed to be onbeat.
You can toggle it on and off with the Inspections button.

SCRIPTING:
As of v2.9.0, experimental scripting support has been added. View this page for more details.
https://github.com/chrislo27/RhythmHeavenRemixEditor2/wiki/Scripting

CONTROLS:
To drag cues, select the series you want (GBA, DS, FEVER, MEGAMIX, etc.),
click on the game you want on,
then scroll on the pattern list. Click and drag out of the pattern list and it will
spawn in the pattern you've chosen.

To delete, drag to below the track area, or press DELETE or BACKSPACE.

If you can't scroll, W/S/UP/DOWN imitates scrolling.

Right click - change the start playback tracker
Middle click OR Ctrl+Right click - change the music's start time tracker
A/D or LEFT/RIGHT - move camera
	Hold CONTROL/SHIFT - faster
Click and drag from pattern area - place cues
Click and drag in track - Move cues
	Hold ALT - copy
SPACEBAR - Play/stop remix
SHIFT+SPACEBAR - Play/pause remix
HOME - Jump to beginning
END - Jump to end
DEL/BACKSPACE - Delete selection
1, 2, 3, ... - Tool select
Scroll while selecting cues - change pitch
F8 - Debug mode
R while in debug mode and selecting cues - Export pattern to console
CONTROL+Z - Undo
CONTROL+Y - Redo

BPM tool:
Left click - add BPM marker
Scroll - change BPM
	Hold SHIFT - +/- 5
	HOLD CTRL  - +/- 0.1

Pattern split tool:
Left click pattern - split into separate cues

