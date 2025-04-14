using UnityEngine;
using UnityEditor;

public class AlignAndDistribute : MonoBehaviour
{
    public float customSpacing = 0f;  // Khoảng cách tùy chỉnh giữa các đối tượng

    [MenuItem("Tools/Align & Distribute/Distribute Evenly X")]
    static void DistributeEvenlyX()
    {
        // Tạo đối tượng mới để truy cập customSpacing
        AlignAndDistribute alignAndDistribute = new AlignAndDistribute();
        alignAndDistribute.DistributeObjects(Vector3.right);
    }

    [MenuItem("Tools/Align & Distribute/Distribute Evenly Y")]
    static void DistributeEvenlyY()
    {
        AlignAndDistribute alignAndDistribute = new AlignAndDistribute();
        alignAndDistribute.DistributeObjects(Vector3.up);
    }

    [MenuItem("Tools/Align & Distribute/Distribute Evenly Z")]
    static void DistributeEvenlyZ()
    {
        AlignAndDistribute alignAndDistribute = new AlignAndDistribute();
        alignAndDistribute.DistributeObjects(Vector3.forward);
    }

    void DistributeObjects(Vector3 axis)
    {
        var selected = Selection.transforms;
        if (selected.Length < 2) return;

        // Sắp xếp các đối tượng theo vị trí trên trục cần căn
        System.Array.Sort(selected, (a, b) => a.position[GetAxisIndex(axis)].CompareTo(b.position[GetAxisIndex(axis)]));

        // Tính tổng chiều dài của tất cả các object trên trục đó
        float totalLength = 0;
        for (int i = 0; i < selected.Length; i++)
        {
            Renderer renderer = selected[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                totalLength += renderer.bounds.size[GetAxisIndex(axis)];
            }
        }

        // Khoảng cách do người dùng tùy chỉnh
        float spacing = customSpacing;

        // Tính toán vị trí đầu tiên để căn chỉnh
        float currentPos = selected[0].position[GetAxisIndex(axis)];

        for (int i = 0; i < selected.Length; i++)
        {
            Renderer renderer = selected[i].GetComponent<Renderer>();
            float size = 0;
            if (renderer != null)
            {
                size = renderer.bounds.size[GetAxisIndex(axis)];
            }

            // Điều chỉnh vị trí để các đối tượng căn giữa
            Vector3 pos = selected[i].position;
            pos[GetAxisIndex(axis)] = currentPos + size / 2;
            selected[i].position = pos;

            // Cập nhật vị trí cho đối tượng tiếp theo
            currentPos += size + spacing;
        }
    }

    static int GetAxisIndex(Vector3 axis)
    {
        if (axis == Vector3.right) return 0; // Trục X
        if (axis == Vector3.up) return 1;    // Trục Y
        return 2;                            // Trục Z
    }
}
