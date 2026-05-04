# Devblog 4

## Assets
The asset I’ve used throughout my games, such as the characters, enemies, and weapons, was all sourced from the internet. Some were easy to incorporate, while others I had to refine within a program called GIMP.  

GIMP was useful in removing the background and making it transparent. It was also useful to refine other AI-prompted assets for the UI, which I’ll go into depth in the next Devblog.

<div style="display: flex; gap: 10px;">
  <img width="48%" src="https://github.com/user-attachments/assets/6b14e593-04ad-4572-bc00-0729db7bccd0" />
  <img width="48%" src="https://github.com/user-attachments/assets/bab5b3e9-e76d-4d62-99c3-ba14e673a621" />
</div>

---

## Characters and Animation
The characters were sourced from a website called craftpix.net. They offer a lot of free 2D character bundles ready for use. I was specifically looking for characters that resembled something from the game Icy Tower, which led me to choose the Homeless Man bundle and the Graffiti Artist bundle.

Rigging these characters was fairly simple. I began by creating the animations for idle, walking, and jumping. I achieved this by dragging the corresponding multi-sprite sheets into the scene and naming them according to their animation type.

I gave more attention to the Homeless Man character by adding a timed idle variation. After a certain amount of time (x seconds), the character transitions into a drinking animation, where he drinks his beer. This required creating multiple Animation Clips to handle both the standard idle state and the timed behaviour.

I also wanted to give a special jump animation to the Graffiti character, but it took a lot of time to figure out, so I ended up scrapping the idea due to time constraints.

Afterwards, I created an **Animator Controller** to control and transition between these animation clips for my characters. I did not implement **Blend Trees**, as I had already set up the transitions for both characters before being introduced to this feature, and I chose to stick with that approach.  

Perhaps if I had used Blend Trees, I could have achieved smoother transitions between certain animations, such as movement states, but for this project the existing setup was sufficient.
<p align="center">
<img width="1142" height="441" alt="image2" src="https://github.com/user-attachments/assets/4a5ae827-18e9-4389-b759-9547fa9e0c27" />
</p>

---

## Flying Demon (Enemy AI)
For the enemies, this idea came later when I was brainstorming ways to spice up the combat and shift the player’s focus. This resulted in me implementing a flying enemy AI that interacts with and attacks the players.  

The asset for this enemy was sourced for free from itch.io.

<p align="center">
<img width="474" height="384" alt="image1" src="https://github.com/user-attachments/assets/8c781086-c383-48a9-8ed2-4c7f60fe5c42" />
</p>

The current implementation is that two demons spawn every x amount of seconds, and the timer resets whenever that specific demon is killed. Additionally, the logic is set so that each player has an assigned demon, and only the assigned demon can damage its assigned character.

The movement is handled through custom logic rather than using a NavMesh. The demon constantly checks its distance from the assigned player and adjusts its position accordingly, either moving closer, backing away, or hovering within a preferred range.  

Additionally, if the player attempts to ride the demon, it will automatically lower itself slowly, preventing it from being abused to move upwards and bypass the platforming as intended.

Naturally, I also imported a custom fireball asset that the demon can shoot and hooked it up to the demon’s animation rig.

<p align="center">
<img width="1014" height="242" alt="image6" src="https://github.com/user-attachments/assets/d0f94c0f-7926-41f2-b179-a73def73b2f3" />
</p>

<table align="center">
  <tr>
    <td align="center">
      <img height="384" alt="image5" src="https://github.com/user-attachments/assets/d2ccb756-b6bc-4fd5-9346-fad62d919b74" />
    </td>
    <td align="center">
      <img height="384" alt="image4" src="https://github.com/user-attachments/assets/e46ca834-5069-403b-9547-20792e355097" />
    </td>
  </tr>
</table>


