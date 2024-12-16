public interface IEnemy : IDamageable
{
    bool CanBeSteppedOn();
    void GiveDamage();
}
