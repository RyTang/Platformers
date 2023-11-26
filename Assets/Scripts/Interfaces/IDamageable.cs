
/// <summary>
/// Interface any object that can be affected by damaged should be interfacing this
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Damage to be taken and what will happen to the person/object
    /// </summary>
    /// <param name="damage">Damage Amount</param>
    void TakeDamage(int damage);

    /// <summary>
    /// What happens when this objedt gets destroyed
    /// </summary>
    void Destroyed();
}