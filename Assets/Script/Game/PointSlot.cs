using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSlot : MonoBehaviour
{
    new BoxCollider collider;
    Point point;
    SpriteRenderer sprite_renderer;

    void Awake()
    {
        this.collider = this.gameObject.GetComponent<BoxCollider>();
        this.sprite_renderer = this.gameObject.transform.Find("point_slot").GetComponent<SpriteRenderer>();
    }

    public void set_point(Point point)
    {
        this.point = point;
    }

    public Point get_point()
    {
        return this.point;
    }

    public bool is_same_point(byte x, byte y)
    {
        return this.point.is_same_point(x, y);
    }

    public void get_sprite_image(Sprite sprite)
    {
        this.sprite_renderer.sprite = sprite;
    }

    public void set_state(STATE state)
    {
        this.point.set_state(state);
    }

    public void enable_collider(bool flag)
    {
        this.collider.enabled = flag;
    }
}
