public delegate void OnHealthChanged(IHealth changed, float oldHP, float newHP, float percent);

public interface IHealth 
{
    float maxHP {get; set;}   
    float currentHP {get; set;}
    void AdjustHealth(float percent);
    void TakeDamage(float damage);
    void Heal(float amount);
    void HealToFull();
    void Die();
    event OnHealthChanged onHealthChanged;
}