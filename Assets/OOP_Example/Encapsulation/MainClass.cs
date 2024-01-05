using UnityEngine;

public class MainClass : MonoBehaviour
{
    public string dragonAbility = "FireAttack";
    protected string dragonName = "Drogo";  
    private string dragonClass = "Fire";

    //GETTER
    public string GetName()
    { 
        return dragonName; 
    }
    public string GetClass()
    {
        return dragonClass;
    }
}
    