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

Coding Style Guidlines...
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
    header explaining the intent and/or goals of the object.
-