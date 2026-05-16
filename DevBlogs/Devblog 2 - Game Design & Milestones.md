# Devblog 2 - Game Design & Milestones

## Introduction

The game I planned to make was inspired by **Icy Tower**, an old platform game I used to play as a kid. In Icy Tower, the objective is to climb upwards while the game gradually increases in speed, making it harder to stay on the platforms.

I chose this game as inspiration because it fits well with an arcade setup. It uses minimal inputs, such as **W, A, S, D** for movement, **SPACE** for jumping, and an additional input for attacking.

<table align="center">
  <tr>
    <td align="center">
      <img width="500" alt="icy tower 1" src="https://github.com/user-attachments/assets/fe7d8fd3-61b4-4155-a561-d3161978a855" />
    </td>
    <td align="center">
      <img width="500" alt="icy tower 2" src="https://github.com/user-attachments/assets/2a34d05f-2c72-41f7-a053-f0504d20146e" />
    </td>
  </tr>
</table>

---

## Game Design

I’m trying to mimic **“Icy-Tower”**, but with a twist. I will still implement the endless upward tower mechanic, but instead of it being a solo experience where you fall to your death, it will be a **two-player game**.

The objective is to manage your own character while also trying to outlive your opponent, whether that is by actively attacking your opponent or avoiding them to outlive them.

---

## Milestones and Goals (1/2)

Within this section, I’ll outline the requirements I would like to implement into my game:

- **2D Game**: The game will be designed and developed as a 2D experience  
- **Endless Background**: The game will loop images seamlessly to make the tower appear endless  
- **Endless Platforms**: Players will need platforms to navigate upwards to avoid death  
- **Characters**: Player 1 and Player 2 must have controllable characters with 2D animations for running, jumping, and attacking  
- **Lava Pool**: Placed at the bottom of the screen, including an idle animation and a lava splash effect when players collide with it  

---

## Milestones and Goals (2/2)

- **Weapons**: Weapons will spawn on platforms and can be picked up by players  
  - Knife: Shank players  
  - Bat: Knockback effect  
  - AK-47: Shoot projectiles  

- **Character Hurt Animations**: When a player gets hurt, blood particles should spawn  

- **Sound Effects**:  
  - Player damage sounds  
  - Lava splash on collision  
  - Weapon activation/hit sounds  
  - Main menu music and in-game music  

- **Random Events**:  
  This could include events like boulders falling down and dealing massive damage, or other random entities/events appearing  

- **Platform Speed Increase**:  
  Implement a system where platforms speed up over time or in bursts to increase difficulty and stress for the players  

---

## Aesthetics

When designing the game, I focused mainly on **Challenge**, **Sensation**, and **Fellowship**.

The game is built around **Challenge**, as players must continuously platform upwards while the speed and pressure increase. This requires quick reactions, careful movement, and awareness of both environmental dangers and the opposing player.

**Sensation** is created through the fast-paced arcade gameplay, visual effects, and sound design. Lava animations, hit effects, blood particles, speed increases, and music help make the game feel intense and chaotic.

I also used **Fellowship**, since the game is multiplayer and encourages competitive interactions. Both players try to survive longer than the other while interfering through combat and movement.
