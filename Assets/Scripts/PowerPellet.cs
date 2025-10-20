using UnityEngine;

public class PowerPellet : Pellet
{
    public float duration = 8f;

    protected override void Eat()
    {
        GameObject.Find("PelletSpawner").GetComponent<PelletSpawner>().GrabbedPellet();
        GameManager.Instance.PowerPelletEaten(this);
    }

}
