internal class EnergyEffect : StatusEffect
{
    public int m_ModifierValue = 1;

    public EnergyEffect()
    {
        m_Name = "Energy Effect";
    }

    public void Initialize(string name, float time, int value)
    {
        m_Name = name;
        m_Timer = time;
        m_ModifierValue = value;
    }

    public override void ApplyEffect(Unit owner)
    {
        owner.m_Energy.CurrentValue += m_ModifierValue;
    }
}