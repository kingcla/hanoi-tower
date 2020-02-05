# Hanoi Tower Game

<!--This game was commissioned by [**TocaBoca**](https://tocaboca.com/) games srl for an interview.  -->

Some of the assets (disks, base and poles) are covered by copyright, therefore, I cannot upload them in the asset folder. You must provide your own images for disks and relative animation parts, for the base and poles png as well.

The rest was downloaded from open source assets websites (like [opengameart.org](https://opengameart.org)). Animations and Logic were developed by me using Unity3D and C# language.

The project had two main target platforms: Windows and Android.

Game code was written in Visual Studio Community edition. For debugging purposes, don’t forget to check the Visual Studio tools for Unity [here](http://unityvs.com/).

Unity version _2017.3.1f1_ was used.

![demo1](https://github.com/kingcla/hanoi-tower/blob/master/demo/demo1.png)
![demo2](https://github.com/kingcla/hanoi-tower/blob/master/demo/demo2.png)

## Game Development Journal

### Day 1

Learning the task and write down some initial game design. Decided to proceed with a simple approach using stick and disk object and a stack as the list of disk on each stick. Stack data structure is perfect for this task because LIFO structure will automatically take the last disk on each stick.

Downloading all the necessary to start with Unity and Visual Studio.

Create the first project and start adding sprites and play around with Unity interface.

Follow different entry level videos for a general introduction.

### Day 2

Complete the creation and import of all sprites.

Creating the animation for each disk with Paint.NET. Animations will give a nice living touch to the disks make the more fun than a mere static sprite.

Learning about animation in Unity.

Test animations.

### Day 3

First impact with scripting: control the drag&amp;drop. Not an easy task, I needed some time to figure it out how to move a sprite and I had some initial difficulties to understand the world and local positions.

Decided to use physics2D to animate the falling of a disk on the stick. It will take off a lot of logic and I will save some time.

Create controllers script for disks and sticks.

### Day 4

Set sticks as triggers.

Very long time spent to make the dragged disk not to collide with the other disks, deactivating the collider was not working because it was not firing the trigger in the stick. Eventually found a solution setting the collider of the disk to trigger while dragging it!!

Make disk go back to the original stick when dropped outside or on a wrong stick.

Stick controller will manage the stack logic of the game with pushing and removing disk GameObject from the game, so that the logic of the game is written in one place.

### Day 5

Polishing the last details for the drag&amp;drop.

Added some effects: shaking ([GitHub source](https://gist.github.com/GuilleUCM/d882e228d93c7f7d0820)) and transparency when hovering the stick.

Also change the animation of the dropping in case the disk was dropped on the same stick.

GameManager script created to manage timer and score. It uses the Singleton pattern.

GameMangaer script also implements some sort of Composition pattern, where the &quot;Game Winning&quot; stick is passed as field and the GameManager uses to check on the winning conditon.

Rudimentary UI showing timer and a game over text.

### Day 6

Full UI day, polished and improved (Using Ui sprites from [opengameart.org](https://opengameart.org)).

Use events fired by the disks to increment the moves score.

Added an overlay panel to make the colors of the disks more soft.

Add game restart.

Add game info.

Beginning including the background and scene details.

Beginning adapting the game for Mobile screens also.

### Day 7

Add sounds and music (Ambient music and game sounds from [opengameart.org](https://opengameart.org)).

Complete background and scrolling elements. Scrolling clouds was not an easy task and strangely enough I didn&#39;t found much info on how to do it and how to actually scale it with bigger screen. Nevertheless, I am convinced it was worth it the effort because it gave a very nice dynamic look to the game.

Use a canvas to adapt background when changing resolution.

Created transparency and fade-out effect when dragging disk on a stick.

Adding glow effect to each stick when hovered to give an extra confirmation to the player where the disk will be dropped. See [SpriteGlow](https://github.com/Elringus/SpriteGlow).

### Day 8

Reorganizing scripts and asset folders.

Adding some extra elements in the background.

Randomize background images.

Compile and run on my Android device to test the game with different resolutions and device. It worked!!

## Possible improvements

- User a countdown before starting the timer
- Add some particles system while dragging
- Animate disk when dragging
- Add UI options (for music and sounds)
- Add initial menu and different difficulty levels
