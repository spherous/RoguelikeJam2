public interface IEnemy : IHealth
{
    float speed {get; set;}
    void AdjustSpeed(float amount);
}