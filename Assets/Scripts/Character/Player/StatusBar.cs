using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    public Slider     m_Slider;
    public Text       m_Text;
    private Attribute m_Attribute;

    public void Initialize(Attribute attribute)
    {
        m_Attribute = attribute;
    }

    private void Update()
    {
        m_Slider.value = m_Attribute.CurrentValue / m_Attribute.MaxValue;
        m_Text.text = m_Attribute.CurrentValue.ToString() + "/" + m_Attribute.MaxValue.ToString();
    }
}
