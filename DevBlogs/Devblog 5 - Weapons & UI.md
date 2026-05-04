# Weapons

The weapons are sourced as PNG assets from the internet. The system is designed so that players can pick up weapons from platforms and use them during gameplay. Once equipped, the weapons are anchored to the character, supporting both left and right facing directions.
The logic behind this system is handled by three main scripts: **WeaponPickup**, **EquippedWeaponVisual**, and **PlayerWeaponHolder**.

<img width="1280" height="1064" alt="download-ezgif com-video-to-gif-converter" src="https://github.com/user-attachments/assets/98199375-1cd5-4b2a-87db-0d7155fae0a0" />


<p align="center">
  <em>Diagram showing how weapon pickups, equipped visuals, and player weapon handling communicate with each other.</em>
</p>

The **WeaponPickup** script is responsible for defining the behaviour of each weapon. It assigns values such as damage, fire rate, and knockback. These values depend on settings like **IsRangedWeapon** and **EnableKnockback**, which determine how the weapon behaves.

This script also connects the world pickup to the player’s equipped version. When the player picks up a weapon, the correct visual and scale are applied to the character, ensuring it fits properly when equipped.

The equipped version of the weapon also has a script attached called **EquippedWeaponVisual**, which controls how the weapon is animated during use. This includes how far the weapon swings (**SwingZDelta**), as well as the duration of the swing and its return to the idle position.

These values are then passed to the **PlayerWeaponHolder** script, which **overrides** the character’s default combat values such as damage, fire rate, and knockback. It also ensures that the weapon is correctly anchored to the character and used properly during gameplay.

---

## UI

For the UI of the game, I designed it to match the overall style and atmosphere. The start screen uses a transparent yellow tint, allowing the player to still see the characters and background elements behind it.  

In addition, I implemented a fight HUD for each character, displaying their portrait, health bar, and remaining lives.  

The game includes four main menu categories: **Start**, **Settings**, **Controls**, and **Quit**. The **Start** option begins the game and enables player control. **Settings** allows the player to adjust the volume, while **Controls** displays the hardcoded keybindings for each character. The **Quit** option exits the game.  

To structure the layout, I used Unity components such as **Vertical Layout Group** and **Horizontal Layout Group**, which helped maintain consistent spacing between UI elements, such as the start screen buttons and the lives displayed in the fight HUD.  

<img width="1465" height="669" alt="UIShowCase" src="https://github.com/user-attachments/assets/862560c9-18a0-44c1-9c3f-0d651216b282" />



## UI Skinning

After implementing the UI functionality, I began styling each UI element to better match the game’s visual theme.  

For simpler elements such as buttons, this involved navigating to the **Image** component and replacing the source image with a custom sprite.  

Sliders required a slightly more detailed setup. By expanding the slider hierarchy, I accessed elements such as **Background**, **Fill Area**, and **Handle Slide Area**. The **Fill Area** and **Handle Slide Area** contain child objects with their own **Image** components, which needed to be updated with new source images and scaled appropriately to fit the design.  

This approach allowed me to fully customize the appearance of the UI while keeping the underlying functionality intact. The same concept was also applied to the fight HUD to maintain visual consistency across all UI elements.

<img width="1215" height="450" alt="UIShowcase2" src="https://github.com/user-attachments/assets/534431e9-d4a3-47f8-b4c4-d30539df27cc" />
