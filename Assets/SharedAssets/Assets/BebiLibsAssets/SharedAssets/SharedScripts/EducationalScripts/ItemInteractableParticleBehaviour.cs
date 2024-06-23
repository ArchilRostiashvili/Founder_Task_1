using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteractableParticleBehaviour : MonoBehaviour
{
    [SerializeField] ItemInteractable SC_ItemInteractable;
    [SerializeField] GameObject GO_Particle;
    [SerializeField] bool startAsDisabled = true;

    private void Awake()
    {
        if (this.startAsDisabled) this.GO_Particle.SetActive(false);
    }

    private void OnEnable()
    {
        this.SC_ItemInteractable.onDragStart.AddListener(this.OnDragStart);
        this.SC_ItemInteractable.onDragEnd.AddListener(this.OnDragEnd);
    }

    private void OnDisable()
    {
        this.SC_ItemInteractable.onDragStart.RemoveListener(this.OnDragStart);
        this.SC_ItemInteractable.onDragEnd.RemoveListener(this.OnDragEnd);
    }

    private void OnDragStart(ItemInteractable interactable) => this.GO_Particle.SetActive(true);

    private void OnDragEnd(ItemInteractable interactable) => this.GO_Particle.SetActive(false);
}
