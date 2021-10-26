using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Gradient")]
public class GradientEffect : BaseMeshEffect
{
    [SerializeField]
    private Color32 top_color = Color.white;

    [SerializeField]
    private Color32 bottom_color = Color.black;

    public Gradient gradient;

    public override void ModifyMesh(VertexHelper vertex_helper)
    {
        if (!IsActive()) return;

        GradientColorKey[] color_key = new GradientColorKey[2];
        GradientAlphaKey[] alpha_key = new GradientAlphaKey[2];

        color_key[0].color = top_color;
        color_key[0].time = 1.0f;
        color_key[1].color = bottom_color;
        color_key[1].time = -1.0f;

        alpha_key[0].alpha = 0;
        alpha_key[0].time = 1.0f;
        alpha_key[1].alpha = 0;
        alpha_key[1].time = -1.0f;

        gradient.SetKeys(color_key, alpha_key);

        List<UIVertex> ui_vertex_list = new List<UIVertex>();

        vertex_helper.GetUIVertexStream(ui_vertex_list);

        float min = ui_vertex_list.Min(t => t.position.y);
        float max = ui_vertex_list.Max(t => t.position.y);

        float ui_element_height = max - min;

        for (int i = 0; i < ui_vertex_list.Count; i++)
        {
            UIVertex ui_vertex = ui_vertex_list[i];

            float current_y_normalized = Mathf.InverseLerp(min, max, ui_vertex.position.y);
            Color color = gradient.Evaluate(current_y_normalized);

            ui_vertex.color = new Color(color.r, color.g, color.b, 1);
            ui_vertex_list[i] = ui_vertex;
        }

        vertex_helper.Clear();
        vertex_helper.AddUIVertexTriangleStream(ui_vertex_list);
    }
}
