using UnityEngine;
using System;


public class VRButton : MonoBehaviour
{
    public Color activeColor = Color.green;
    public Color defaultColor = Color.gray;
    private Renderer rend;
    private bool isActive = false;
    private float timer = 0f;
    private Action onPressed;

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.color = defaultColor;
    }

    void Update()
    {
        if (isActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
                Deactivate();
        }
    }

    public void Activate(Action onPressedCallback, float activeDuration = 2f)
    {
        isActive = true;
        rend.material.color = activeColor;
        timer = activeDuration;
        onPressed = onPressedCallback;
    }

    public void Deactivate()
    {
        isActive = false;
        rend.material.color = defaultColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        if (other.CompareTag("PlayerHand"))
        {
            onPressed?.Invoke();
            Deactivate();
        }
    }
}
