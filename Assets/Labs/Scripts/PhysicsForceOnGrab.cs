using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class PhysicsForceOnGrab : MonoBehaviour
{
    public float massOnGrab = 5f;
    public float pushForce = 3f;
    public ForceMode pushForceMode = ForceMode.Impulse;
    public Vector3 pushDirection = Vector3.up;
    public bool restoreMass = true;
    public float massRestoreSpeed = 4f;

    XRGrabInteractable m_Grab;
    Rigidbody m_Body;
    float m_OriginalMass;
    bool m_Grabbed;
    float m_RestoreTimer;

    void Awake()
    {
        m_Grab = GetComponent<XRGrabInteractable>();
        m_Body = GetComponent<Rigidbody>();
        if (m_Body == null)
            m_Body = gameObject.AddComponent<Rigidbody>();
        m_OriginalMass = m_Body.mass;

        m_Grab.selectEntered.AddListener(OnGrabbed);
        m_Grab.selectExited.AddListener(OnReleased);
    }

    void OnDestroy()
    {
        if (m_Grab == null)
            return;
        m_Grab.selectEntered.RemoveListener(OnGrabbed);
        m_Grab.selectExited.RemoveListener(OnReleased);
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        m_Grabbed = true;
        m_RestoreTimer = 0f;
        m_Body.mass = massOnGrab;
        m_Body.AddForce(pushDirection.normalized * pushForce, pushForceMode);
    }

    void OnReleased(SelectExitEventArgs args)
    {
        m_Grabbed = false;
        if (!restoreMass)
            m_Body.mass = m_OriginalMass;
    }

    void Update()
    {
        if (!m_Grabbed && restoreMass && !Mathf.Approximately(m_Body.mass, m_OriginalMass))
        {
            m_RestoreTimer += Time.deltaTime * massRestoreSpeed;
            m_Body.mass = Mathf.Lerp(massOnGrab, m_OriginalMass, Mathf.Clamp01(m_RestoreTimer));
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
        Vector3 dir = pushDirection.normalized * pushForce;
        Gizmos.DrawLine(transform.position, transform.position + dir);
    }
}
