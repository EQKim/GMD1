# Weapons

The weapons are sourced as PNG assets from the internet. The system is designed so that players can pick up weapons from platforms and use them during gameplay. Once equipped, the weapons are anchored to the character, supporting both left and right facing directions.
The logic behind this system is handled by three main scripts: **WeaponPickup**, **EquippedWeaponVisual**, and **PlayerWeaponHolder**.

<p align="center">
  <img width="950" alt="Weapon system diagram" src="https://github.com/user-attachments/assets/a2ea7881-45eb-4f16-b1a1-353060996307" />
</p>

<p align="center">
  <em>Diagram showing how weapon pickups, equipped visuals, and player weapon handling communicate with each other.</em>
</p>

The **WeaponPickup** script is responsible for defining the behaviour of each weapon. It assigns values such as damage, fire rate, and knockback. These values depend on settings like **IsRangedWeapon** and **EnableKnockback**, which determine how the weapon behaves.

This script also connects the world pickup to the player’s equipped version. When the player picks up a weapon, the correct visual and scale are applied to the character, ensuring it fits properly when equipped.

The equipped version of the weapon also has a script attached called **EquippedWeaponVisual**, which controls how the weapon is animated during use. This includes how far the weapon swings (**SwingZDelta**), as well as the duration of the swing and its return to the idle position.

These values are then passed to the **PlayerWeaponHolder** script, which **overrides** the character’s default combat values such as damage, fire rate, and knockback. It also ensures that the weapon is correctly anchored to the character and used properly during gameplay.

---

# UI
