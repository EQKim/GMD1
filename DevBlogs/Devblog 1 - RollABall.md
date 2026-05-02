# Devblog 1 - RollABall 

Starting the tutorial section **"Setting up the game"**, it was fairly simple to follow. I had to create the foundation by spawning a 3D object called a plane, which acted as the ground floor. Afterwards, I also created a sphere, which would later in the tutorial become the player-controlled object that we can control with **“W, A, S, D”**. In this section, we also created a materials folder where I colored my sphere to a bright, shiny cyan. 

Moving forward to the next part, **“Moving the Player”**, I learned that I could attach components to my sphere, such as adding a **“Rigidbody”**, which allows it to behave more like a real object, for example by calculating physics. This will be required further ahead in the tutorial. We also added another component called **“Player Input”**, which assists in getting basic controls working.

<p align="center">
  <img width="423" height="92" alt="image5" src="https://github.com/user-attachments/assets/f1a77e39-88dc-4204-acd6-cf0dbe5d89ad" />
</p>

Then I started creating a custom script using C# called **“PlayerController”**.  
Within this script, I learned that the method **“Void Start”** is called once on startup, while the method **“Void Update”** executes every frame, depending on the game’s **“Frames Per Second”**.

<table align="center">
  <tr>
    <td align="center">
      <img width="381" alt="image2" src="https://github.com/user-attachments/assets/24feb31d-dfb9-4616-895b-d0ee721a8341" />
    </td>
    <td align="center">
      <img width="800" alt="image3" src="https://github.com/user-attachments/assets/6a6a4901-d7df-45ae-8f9e-f5388d6b90e1" />
    </td>
  </tr>
</table>

Now tying that to our **“Player Input”** component, we can call **“OnMove”** to get information about our ball placement. Even though the method **“OnMove”** isn’t called anywhere else within the script, it communicates with the **“Player Input”** component and updates our local variables **“movementX, movementY”**. These are then used within the **“FixedUpdate”** method to add force to the ball depending on **“W, A, S, D”**, by utilizing the **“Rigidbody”** component attached to our sphere, defined in the code as **“rb”**.

Moving on to the section **“Moving the Camera”**, this required creating another custom script called **“CameraScript”**, which had logic to follow the ball's movement at an angle. I just had to:

<p align="center">
<img width="599" height="289" alt="image1" src="https://github.com/user-attachments/assets/ed24cd58-44bb-4877-acc7-6af993c541cb" />
</p>

drag the **“Sphere/Player”** GameObject into the script, and it was all hooked up.

In the next section, **“Setting up the Play Area”**, I added walls around my **“Plane”** GameObject so that I couldn’t roll my sphere off the edge into the void. Nothing special in this section.

Next, I’ll talk about the sections **“Creating Collectibles”** and **“Detecting Collisions with Collectibles”** together. Within this section, we had to create mini spheres that the player could collect to win the game. The requirements were that the objects had an animation to rotate and also disappear once they collided with the player/sphere. 

The final bits were about wrapping up the game, having an enemy navigate the **“NavMesh”**, and finally adding a win/lose screen UI.  

<p align="center">
  <img width="1427" height="825" alt="image4" src="https://github.com/user-attachments/assets/aca5d5aa-f637-4de5-93b1-9517cb8f2c48" />
</p>
