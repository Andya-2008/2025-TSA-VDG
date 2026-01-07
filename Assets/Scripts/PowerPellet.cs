using UnityEngine;

public class PowerPellet : Pellet
{
    public float duration = 8f;
    public PelletSpawner myParent;

    protected override void Eat()
    {
        myParent.GrabbedPellet();
        GameManager.Instance.PowerPelletEaten(this);
    }

}
