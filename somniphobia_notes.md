Here, notes about the project and its goals will be captured.

f5 can be used to attach VS Code to the editor instance.
Launch MCP local session using window & `http://localhost:8080`.
Need uv installed (can be installed via Powershell) to run MCP local session.

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