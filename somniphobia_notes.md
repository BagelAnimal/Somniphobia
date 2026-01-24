Here, notes about the project and its goals will be captured.

Launch MCP local session using window & `http://localhost:8080`.
Need uv installed (can be installed via Powershell) to run MCP local session.

Characters are controlled by souls. Souls are controlled by players or CPUs.
Players or CPUs can have any number of souls. Souls can control any number
of characters. The goal is to enable emergent behaviors where high level
CPUs can delegate actions to multiple souls simultaneously, and those
souls can dynamically possess and unpossess characters in the world. While
this layered relationship might result in repetitive code, the goal is to
enable interesting interactions and emergent game behavior.

Coding Style Guidelines...
-
    RULE 1
    Public accessor properties for private fields will be directly below their corresponding field.
    The goal is to make moving field and property pairs easy.
    ```
    private GameObject _playerPrefab;
    public GameObject PlayerPrefab => _playerPrefab;
    ```
-
    RULE 2
    Each attribute should be on its own line above the associated field, method, or class.
    ```
    [Header("Example")]
    [SerializeField]
    private string _name = String.Empty;
    ```
-
    RULE 3
    All lines should remain below 100 characters.
-
    RULE 4
    All classes and structs in the Somniphobia namespace should have an xml-style
    header explaining the goals of the object.
-

Authoring Guidelines...
- 1u = 1m
- Player Dimensions: Capsule, 1.75u tall, 0.4375u radius.
- Prefab Prefix: Entity = E_

Movement Sandbox Layout

Goal: a compact traversal loop that hits most edge cases in 3-5 minutes.

[x] Start (flat platform to be surrounded with test case options)
[x] Step Tests (small platforms of varying heights)
[x] Jump Height Tests (blocks of varying heights)
[x] Physics Object Tests (stacks of crates, crate surface)
[x] Gaps Test (blocks of varying distances)
[x] Ramp Test (many ramps of many angles with labels)
[] Wedge Test (triangular wedges with a seam at the top)
[] Pit Test (concave holes with varying depths ending in a line or a point to test unstuck logic)
[] Alley (various squeezes between walls)
[] Crouch Tunnel Tests (test crouch clearance)
[] Narrow Ledge Tests (thin bridges to test edge snapping and falling)
[] Teleporter Gates (on touch, teleport to another location)
[] Fall Tests (drop a long w/ a reset trigger)
[] Surfing Tests (wedges to slide along, source-engine style)
[] Terrain w/ Gradual Slopes (teleport to and from this area)
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
[] Curved Tests (walls, slopes)

Notes:
- Keep each node within line of sight of the next to speed iteration.
- Provide a "reset" button/trigger to return to Start quickly.
- Include a debug sign at each node describing the targeted behaviors.

Sandbox Spec Entities:
- Sign: worldspace UI with a short textmeshpro label to label tests.
- Teleport Trigger: on collision enter, return target to a position serialized in the component.

Movement Sandbox Blockout Spec
- Each test gets a Sign and a nearby reset/teleport target.
