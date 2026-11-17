# Snow Village Defense

A 3D tower defense game made in Unity 6.6 for Assignment 2. Defend a decorated winter village from three waves of zombies.

## How to play

Open `Assets/Scenes/SnowDefense.unity` and press Play. Click a glowing build spot to choose a tower, then press Start when ready. Defeating enemies earns coins to buy more towers. Click a placed tower to open its sell menu and receive a full refund if you sell it.

Defeat all three waves to win. If any enemy reaches the end of the path alive, the game ends in a loss.

## Assignment requirements

- **Tower placement and limits:** Players choose where to build using fixed slots. Available coins and slots limit how many towers can be placed.
- **Automatic firing and aiming:** Towers track enemies within range and fire automatically at a set rate. Projectiles use 3D physics and collisions to deal damage.
- **Different tower types:** Basic, Rapid, and Heavy towers have different firing rates, projectile speeds, and damage, with distinct weapon models and tower heights.
- **Placement instructions:** On-screen text explains how to build and how to open the sell menu.
- **Enemy paths:** Enemies follow a predetermined winding waypoint path with multiple turns.
- **End condition:** An enemy surviving to the final waypoint triggers Game Over.
- **Different enemy types:** Smaller zombies are faster with less health; larger zombies are slower with more health.
- **Non-basic assets:** Imported character, tower, and environment models create the snowy village setting.
- **Predictive aiming:** Towers estimate where to aim using the enemy's position and velocity, accounting for projectile travel time.

## Additional features

- Coin economy with tower prices, enemy rewards, and full-refund selling.
- Three waves with increasing enemy counts and a victory screen.
- Red flashes when enemies take damage, plus walking animations.
- Flickering cyan halos that highlight available build spots.
- Winter-themed UI with a snowflake coin icon and the Lilita One font.
- Decorative cabins, trees, gifts, rocks, and other holiday scenery.
- A Play Again option after winning or losing.

## Asset credits

Kenney Tower Defense Kit, Blocky Characters, and Holiday Kit (CC0). Font: Lilita One.
