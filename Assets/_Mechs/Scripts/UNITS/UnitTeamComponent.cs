// Assets/_Mechs/Scripts/ENEMIES_V2/UnitTeam.cs
using UnityEngine;

public enum UnitTeam { Player, Ally, Enemy }

[DisallowMultipleComponent]
public class UnitTeamComponent : MonoBehaviour

{
    [SerializeField] private UnitTeam team = UnitTeam.Enemy;
    public UnitTeam Team => team;
}
