public class Attack
{
    private readonly int _damage;
    private readonly bool _critical;

    public Attack (int damage, bool critical)
    {
        _damage = damage;
        _critical = critical;
    }

    public int Damage => _damage;

    public bool IsCritical => _critical;
}
