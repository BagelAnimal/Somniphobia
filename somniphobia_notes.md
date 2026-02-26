---

SOMNIPHOBIA

Here, notes about the project and its goals will be captured.

Somniphobia is a first-person roguelike dungeon crawler that makes use of several high-impact
sandbox mechanics with the intent of creating emergent situations. Mechanics include
gravity manipulation, dynamic possession systems, and spellcasting.

The goal is to make the player feel the mystery, exploration, and experimentation of Noita,
the action of ULTRAKILL, and the skill expression of a source-engine game like Half-Life 2.

---

MECHANICS OVERVIEW

A high-level goal of the game is to enable skill expression in a variety of ways.

Characters are controlled by souls. Souls are controlled by players or CPUs.
Players or CPUs can have any number of souls. Souls can control any number
of characters. The goal is to enable emergent behaviors where high level
CPUs can delegate actions to multiple souls simultaneously, and those
souls can dynamically possess and unpossess characters in the world. While
this layered relationship might result in repetitive code, the goal is to
enable interesting interactions and emergent game behavior.

Character movement is inspired by the source-engine. The controller will be
written to enable air-strafing and surfing while rewarding frame-perfect
jump inputs.

---

CODING STYLE GUIDELINES
-
    1
    Public accessor properties for private fields will be directly below their corresponding field.
    The goal is to make moving field and property pairs easy.
    ```
    private GameObject _playerPrefab;
    public GameObject PlayerPrefab => _playerPrefab;
    ```
-
    2
    Each attribute should be on its own line above the associated field, method, or class.
    ```
    [Header("Example")]
    [SerializeField]
    private string _name = String.Empty;
    ```
-
    3
    All lines should remain below 100 characters.
-
    4
    All classes and structs in the Somniphobia namespace should have an xml-style
    header explaining the goals of the object.
-
    5
    Fail as loudly as possible when encountering null references without throwing
    exceptions. Avoid these cases from causing side effects, i.e., grabbing the
    wrong component as a fallback and failing silently.
-
    6
    Avoid using <see> and <seealso> in xml headers as they create additional references
    when debugging to sift through.

---

AUTHORING GUIDELINES

- 1u = 1m
- Player Dimensions: Capsule, 1.75u tall, 0.4375u radius.
- Prefab Prefix: Entity = E_

---

SANDBOX

Goal: create a wide variety of test cases for character controller
and physics-related interactions

[x] Start (flat platform to be surrounded with test case options)
[x] Step Tests (small platforms of varying heights)
[x] Jump Height Tests (blocks of varying heights)
[x] Physics Object Tests (stacks of crates, crate surface)
[x] Gaps Test (blocks of varying distances)
[x] Ramp Test (many ramps of many angles with labels)
[x] Wedge Test (triangular wedges with a seam at the top)
[x] Seam Test (walk between tiled planes to ensure parallel seams don't cause collsion)
[x] Pit Test (concave holes with varying depths ending in a line or a point to test unstuck logic)
[x] Alley Test (various squeezes between walls)
[x] Crouch Tunnel Tests (test crouch clearance)
[] Narrow Ledge Tests (thin bridges to test edge snapping and falling)
[] Curved Tests (walls, slopes)
[x] Teleporter Gates (on touch, teleport to another location)
[] Fall Tests (drop a long w/ a reset trigger)
[x] Terrain w/ Gradual Slopes (teleport to and from this area)
[] Surfing Tests (wedges to slide along, source-engine style)
[] Stairs & Stair Mesh
[] Moving Platform (linear mover over a reset trigger)
[] Rotating Platform (of varying speeds)
[] Elevator Shaft (vertical movement)
[] Pusher Tests (horizontally sliding walls)
[] Squeeze Test (sliding wall that closes on the player, giving them nowhere to go)
[] Conveyor Tests (belts that further test external forces)
[] Bouncer Tests (spheres that apply force outward on touch)
[] Physics Material Tests (step, jump, stair, and slope tests w/ ice & rubber)
[] Water Volume Test (pool to drop down into water, sphere of water)
[] Ladder Test (sheer wall with ladder entity)
[] Slider Test (wedges with script that disallows becoming grounded, i.e., overwatch roofing)

Sandbox Spec Entities:
[x] Sign: worldspace UI with a short textmeshpro label to name tests.
[x] Teleporter: on collision enter, return target to a position serialized in the component.
[] Mover: object that follows a series of waypoints in sequence
[] Rotator: object that constantly rotates at some rate
[] Conveyor: object that moves colliding objects?
[] Water: trigger that represents water, transitions objects to in-water state
[] Ladder: some vertical climbable object
[] Roof: wedge that disallows becoming grounded

---