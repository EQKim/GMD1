# Devblog 3 - EndlessManagers
# Managers

Talking about the most important components, I have three empty GameObjects in the root of my Unity scene that define the core functionality. I classify them as **"Endless" managers** because their class names all start with "Endless":

- "EndlessPlatformManager"
- "EndlessObjectFallingManager"
- "EndlessBackgroundManager"

---

## Core Manager

The core manager is **"EndlessPlatformManager"**.  
This class is responsible for:

- Spawning and recycling platform prefabs so that players always have objects to stand on  
- Spawning weapons that players can use against each other  
- Increasing the platform speed every (x) amount of seconds  

This speed increase is then communicated to both the **"EndlessObjectFallingManager"** and the **"EndlessBackgroundManager"**, causing the background to scroll faster and increasing the fall speed of hazard objects, such as the boulder.

<img width="1751" height="1040" alt="download" src="https://github.com/user-attachments/assets/b1d16f55-8208-4218-98ff-8b355cd318b5" />


### Platform and Weapon Spawning

The **"EndlessPlatformManager"** builds a pool of platforms and places them in random lanes. When each platform is created, the script also attempts to spawn a weapon or item on top of it.

```csharp
private void BuildPlatformPool()
{
    ClearAllPlatforms();

    float y = GetSpawnY();

    for (int i = 0; i < poolSize; i++)
    {
        GameObject platformObject = Instantiate(platformPrefab, transform);
        Rigidbody2D rb = platformObject.GetComponent<Rigidbody2D>();

        float x = GetNextLaneX();
        rb.position = new Vector2(x, y);

        platforms.Add(rb);
        platformItems[rb] = null;

        TrySpawnItemOnPlatform(rb);

        y += Random.Range(minGapY, maxGapY);
    }
}

```

### Speed Communication

The **"EndlessPlatformManager"** also controls the shared speed progression. When the platform speed increases, it updates the linked managers.

```csharp
private void UpdateLinkedManagerSpeeds()
{
    if (backgroundManager != null)
        backgroundManager.SetScrollSpeed(currentFallSpeed * backgroundSpeedMultiplier);

    if (fallingObjectManager != null)
    {
        float fallingSpeed = currentFallSpeed * fallingObjectSpeedMultiplier;
        fallingObjectManager.SetFallSpeed(fallingSpeed);
    }
}
```

This shows how **"EndlessPlatformManager"** acts as the central manager by controlling platform spawning, item spawning, and speed progression.

---

## Supporting Managers

The other managers, **"EndlessObjectFallingManager"** and **"EndlessBackgroundManager"**, are responsible for managing their default starting speeds and handling everything related to their specific roles.

---

### EndlessObjectFallingManager

This manager is responsible for:

- Warning players when a boulder is about to spawn at a specific location  
- Defining where boulders can spawn within the camera view  
- Allowing new hazard objects to be added easily through a list  

<img width="2602" height="662" alt="download (1)" src="https://github.com/user-attachments/assets/1f13721c-ece4-4c81-98d6-838caa432934" />


---

### EndlessBackgroundManager

This manager is responsible for maintaining and cycling through a list of background images. Additional backgrounds can easily be added if needed.

This specific manager required some initial setup. To get the system working, I first needed to drag the background images into the scene and scale them to match the camera view.

The hierarchy is structured as follows:  
"EndlessBackgroundManager" (Empty) → "EndlessBackground" (Empty) → BG_A (Sprite) and BG_B (Sprite).

Once this setup is in place, any additional background images added to the "EndlessBackgroundManager" must have the same resolution. The script then automatically scales them to ensure they align seamlessly during scrolling.
<p align="center">
<img width="768" height="220" alt="image" src="https://github.com/user-attachments/assets/27a37c53-16a5-4233-8cad-fbd6b0d74217" />
</p>

---

## Challenges

One challenge I faced was keeping the speed progression consistent between the platforms, background, and falling hazard objects. I solved this by making "EndlessPlatformManager" control the shared speed progression and then communicate the updated speed to the other managers.

Another challenge was making the background loop smoothly, because the images needed to match in resolution and align correctly to avoid visible gaps while scrolling.
