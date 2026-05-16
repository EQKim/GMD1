# Devblog 4

## Assets
The assets used throughout my game, including characters, enemies, and weapons, were sourced online. Some were easy to use, while others needed cleanup in GIMP.

GIMP was mainly used to remove backgrounds, create transparency, and refine AI-prompted UI assets, which I will cover more in the next devblog.

<div style="display: flex; gap: 10px;">
  <img width="48%" src="https://github.com/user-attachments/assets/6b14e593-04ad-4572-bc00-0729db7bccd0" />
  <img width="48%" src="https://github.com/user-attachments/assets/bab5b3e9-e76d-4d62-99c3-ba14e673a621" />
</div>

---

## Characters and Animation
The characters were sourced from craftpix.net, which offers free 2D character bundles. I wanted characters that resembled the style of Icy Tower, which led me to choose the Homeless Man and Graffiti Artist bundles.

The animation setup was fairly simple. I created idle, walking, and jumping animations by dragging the multi-sprite sheets into Unity and turning them into Animation Clips.

I gave extra attention to the Homeless Man by adding a timed idle variation, where he transitions into a drinking animation after standing still. I also wanted a special jump animation for the Graffiti character, but scrapped it due to time constraints.

Afterwards, I used an Animator Controller to manage transitions between clips. I did not use Blend Trees, since the transitions were simple. The diagram below shows the playable character structure in Unity. The player object contains the Rigidbody2D, body collider, controller, health, and weapon scripts, while child objects handle visuals, ground detection, audio, weapon anchors, and the AttackPoint hitbox.

<p align="center">
<img width="1142" height="441" alt="image2" src="https://github.com/user-attachments/assets/4a5ae827-18e9-4389-b759-9547fa9e0c27" />
</p>

<p align="center">
<img width="2692" height="1130" alt="PlayerCharator drawio" src="https://github.com/user-attachments/assets/fff25bae-3a46-47db-ae4f-03ffd13fb583" />
</p>  

---

## Flying Demon (Enemy AI)
For the enemies, this idea came later when I was brainstorming ways to spice up the combat and shift the player’s focus. This resulted in me implementing a flying enemy AI that attacks the players. The asset for this enemy was sourced for free from itch.io.

<p align="center">
<img width="474" height="384" alt="image1" src="https://github.com/user-attachments/assets/8c781086-c383-48a9-8ed2-4c7f60fe5c42" />
</p>

The current implementation is that two demons spawn after a set amount of time. Each player has an assigned demon, and only that demon can damage its assigned character.

The movement is handled through custom logic rather than using a NavMesh. The demon checks its distance from the assigned player and either moves closer, backs away, or hovers within a preferred range.

If the player attempts to ride the demon, it automatically lowers itself slowly, preventing it from being abused to move upwards and bypass the intended platforming.

Additionally, I imported a custom fireball asset, refined it within GIMP, and hooked it up to the demon’s animation rig so the demon could shoot at the player.

```csharp
public void Initialize(Transform assignedTarget, FlyingDemonSpawner spawner)
{
    target = assignedTarget;
    ownerSpawner = spawner;

    if (target != null)
        targetHealth = target.GetComponent<PlayerHealth>();

    if (targetHealth == null && target != null)
        targetHealth = target.GetComponentInParent<PlayerHealth>();
}
```

The demon movement is handled through custom distance-based logic. If the demon is too far away, it moves closer. If it is too close, it backs away. Otherwise, it tries to hover around its preferred range.

```csharp
private void HandleMovement()
{
    if (isAttacking)
        return;

    Vector3 targetPoint = GetTargetPoint();
    Vector3 toTarget = targetPoint - transform.position;
    float distance = toTarget.magnitude;

    Vector3 movement = Vector3.zero;

    if (distance > maxRange)
    {
        movement = toTarget.normalized * moveSpeed * Time.deltaTime;
    }
    else if (distance < minRange)
    {
        movement = -toTarget.normalized * moveSpeed * Time.deltaTime;
    }
    else
    {
        Vector3 desiredPosition = targetPoint - toTarget.normalized * preferredRange;
        Vector3 moveDir = desiredPosition - transform.position;

        if (moveDir.magnitude > 0.15f)
            movement = moveDir.normalized * moveSpeed * Time.deltaTime;
    }

    transform.position += movement;
}
```

To prevent the demon from being used as a platform, I added a rider check. If a player is detected above the demon, it blocks upward movement and slowly pushes the demon downward.

```csharp
if (HasPlayerRider())
{
    if (blockUpwardMovementWhenRidden && movement.y > 0f)
        movement.y = 0f;

    movement.y -= riderPushDownSpeed * Time.deltaTime;
}
```

The demon attack uses a cooldown, windup, and recovery system. This helped make the attack feel more intentional instead of spawning the fireball instantly.

```csharp
private IEnumerator AttackRoutine()
{
    isAttacking = true;
    attackCooldownTimer = attackCooldown;

    animator.SetTrigger(AttackHash);

    yield return new WaitForSeconds(attackWindup);

    ShootFireball();

    yield return new WaitForSeconds(attackRecovery);

    isAttacking = false;
}
```

When the demon dies, it notifies the spawner before being destroyed. This allows the spawning system to know when that demon has been defeated.

```csharp
private void Die()
{
    if (isDead)
        return;

    isDead = true;
    animator.SetBool(DeadHash, true);

    if (ownerSpawner != null)
        ownerSpawner.NotifyDemonDied(this);

    Destroy(gameObject, destroyDelayAfterDeath);
}
```

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
