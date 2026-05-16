# Personal Reflection

I have learned many concepts related to 2D game development, and the work shown throughout my devblogs reflects my full contribution and overall learning experience. For me, there was not much friction during development, as there is a lot of accessible learning material available online. Because of this, I was able to research, test, and implement the different systems needed for my game.

## Implemented Features

Throughout the project, I successfully implemented:

- 2D character movement
- Flying Demon AI
- Rigidbody2D physics
- Jumping and platform collision
- Animator controllers and animation transitions
- Attack hitboxes
- Health and lives system
- Weapon pickups
- Melee and ranged weapons, including bullets
- Main menu UI
- Character portrait UI
- Health, lives, and winner text UI
- Sound effects and music
- GameManager and EndlessManagers
- WebGL built around 1920x1080 resolution

I have also created draw.io diagrams to depict the more complicated parts of my scripts and how they communicate with each other. Since the devblogs are limited to only 3000 characters each, it was difficult to go into depth about certain scripts. Therefore, the diagrams helped provide a better understanding of how specific scripts are connected and how the different systems work together.

<table>
  <tr>
    <td align="center">
      <img width="450" height="380" alt="download" src="https://github.com/user-attachments/assets/b1d16f55-8208-4218-98ff-8b355cd318b5" />
    </td>
    <td align="center">
      <img width="450" alt="draw.io diagram 2" src="https://github.com/user-attachments/assets/f0e20b97-acfe-42ef-8158-565d74b55988" />
    </td>
  </tr>
</table>

<table>
  <tr>
    <td align="center">
      <img width="930" alt="PlayerCharacter drawio" src="https://github.com/user-attachments/assets/fff25bae-3a46-47db-ae4f-03ffd13fb583" />
    </td>
  </tr>
</table>


## Technical Challenge: Knockback

If I were to pick something that was difficult or time-consuming to implement, it would be getting the knockback effect to work cleanly as intended. In earlier iterations, the knockback was mostly applied upwards instead of being based on the direction the character was hit from. This made the combat feel less responsive, because the player was not pushed away naturally from the attack.

To solve this, I calculated the knockback direction based on the attacker’s facing direction. In my game, the character is flipped by changing the X scale of the visual object, so I used `ownerVisual.localScale.x` to determine whether the attacker was facing left or right.

```csharp
float direction = 1f;

if (ownerVisual != null)
    direction = ownerVisual.localScale.x >= 0f ? 1f : -1f;
```

The `direction` variable is first set to `1f` as a default value, meaning right. The code then checks whether `ownerVisual` exists to avoid errors. If the character’s X scale is positive, the direction stays as `1f`, meaning the knockback is applied to the right. If the X scale is negative, the direction becomes `-1f`, meaning the knockback is applied to the left.

This direction value is then used when creating the actual knockback impulse:

```csharp
Vector2 impulse = new Vector2(
    direction * knockbackForce,
    knockbackForce * Mathf.Max(0f, knockbackUpwardMultiplier)
);

targetRb.AddForce(impulse, ForceMode2D.Impulse);
```

The X value of the `Vector2` controls the horizontal knockback by multiplying the direction with `knockbackForce`. The Y value adds a smaller upward force using `knockbackUpwardMultiplier`, which helps make the hit feel more exaggerated. Finally, `AddForce` applies the impulse to the target’s `Rigidbody2D`. I used `ForceMode2D.Impulse` because the knockback should happen instantly when the attack connects, rather than gradually pushing the player over time.

This made the knockback feel more consistent and readable during combat, because the player was pushed in the correct direction while still receiving a small upward force.

## Game Feel and Iteration

Another important learning experience was understanding how much small adjustments affect game feel. Even when a feature worked technically, it still needed testing and refinement before it felt good. This was especially clear with jump height, weapon rotation, knockback, platform speed, and damage values.

Since the game is meant to feel competitive, responsiveness was important. Players should quickly understand what is happening, whether they are attacking, taking damage, picking up weapons, losing lives, or being pushed away by knockback. UI, blood effects, and sound effects helped communicate this clearly, making the features more satisfying and readable.

## Scope and Prioritization

I also knew from the start to keep the scope simple, then refine and enhance the experience afterwards. From previous semesters, I learned that people often overestimate scope, which can lead to a half-baked product. Since I worked alone, it was easier to estimate what needed to be done first and what could be added later.

For example, after the core gameplay was in place, I added details like smoke and muzzle flash for the AK-47, as well as the idle drinking animation for my Homeless Man character. These were not essential to the game, but helped improve the experience and give it more personality.

## Conclusion

Overall, I am satisfied with the outcome of the project. I take pride in what I accomplished, especially while balancing other courses, and I feel the game has developed its own identity beyond its original inspiration, Icy Tower.

If I had more time, I could have added more enemy types, double jump or parkour elements, environmental events, or other weapons such as an RPG. However, I feel I have learned the core components of Unity, including movement, combat, weapons, enemies, UI, sound, C# scripting, animation, physics, and gameplay systems.

Overall, I am proud of what I produced.
