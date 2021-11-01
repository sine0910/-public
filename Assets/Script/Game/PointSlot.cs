using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSlot : MonoBehaviour
{
    new BoxCollider collider;
    Point point;
    SpriteRenderer sprite_renderer;
    TextMesh text;

    void Awake()
    {
        this.sprite_renderer = this.gameObject.transform.Find("point_slot").GetComponent<SpriteRenderer>();
        this.collider = this.gameObject.transform.Find("point_slot").GetComponent<BoxCollider>();
        this.text = this.gameObject.transform.Find("text").GetComponent<TextMesh>();
        this.text.text = "";
        this.text.gameObject.SetActive(false);
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
        switch (state)
        {
            case STATE.BLACK:
                {
                    this.text.color = new Color32(255, 255, 255, 255);
                }
                break;

            case STATE.WHITE:
                {
                    this.text.color = new Color32(0, 0, 0, 255);
                }
                break;
        }
        this.point.set_state(state);
    }

    public void set_index(int index)
    {
        this.text.text = index.ToString();
    }

    public void set_view_text(bool t)
    {
        this.text.gameObject.SetActive(t);
    }

    public void enable_collider(bool flag)
    {
        this.collider.enabled = flag;
    }
}
