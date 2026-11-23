using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TrainerSequence : MonoBehaviour
{
    public XRSimpleInteractable[] buttonsInOrder;
    public IndicatorLight[] stepIndicators;
    public IndicatorLight completionIndicator;
    public GameObject completionMessage;

    int m_Step;

    void Awake()
    {
        for (var i = 0; i < buttonsInOrder.Length; i++)
        {
            var index = i;
            buttonsInOrder[i].selectEntered.AddListener(_ => OnButtonPressed(index));
        }

        if (completionMessage != null)
            completionMessage.SetActive(false);
        RefreshIndicators();
    }

    void OnButtonPressed(int index)
    {
        if (index == m_Step)
        {
            m_Step++;
            RefreshIndicators();
            if (m_Step >= buttonsInOrder.Length)
                OnCompleted();
        }
        else if (index != m_Step - 1)
        {
            m_Step = 0;
            RefreshIndicators();
        }
    }

    void RefreshIndicators()
    {
        if (stepIndicators != null)
        {
            for (var i = 0; i < stepIndicators.Length; i++)
            {
                if (stepIndicators[i] != null)
                    stepIndicators[i].SetOn(i < m_Step);
            }
        }

        if (completionIndicator != null)
            completionIndicator.SetOn(m_Step >= buttonsInOrder.Length);
    }

    void OnCompleted()
    {
        if (completionMessage != null)
            completionMessage.SetActive(true);
    }
}
