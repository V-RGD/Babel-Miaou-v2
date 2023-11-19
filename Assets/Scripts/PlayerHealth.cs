public class PlayerHealth : HealthSystem 
{
    public static PlayerHealth Instance;
    
    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    public override void LooseHealth(int amount)
    {
        base.LooseHealth(amount);
        //HeadUpDisplay.Instance?.UpdateLife(false);
    }

    public override void RestoreHealth(int amount)
    {
        base.RestoreHealth(amount);
        //HeadUpDisplay.Instance?.UpdateLife(true);
    }
}
