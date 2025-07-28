using NXOpen.Assemblies;
using System.Collections.Generic;

public class ComponentWalker
{
    public static List<Component> GetAllComponents(Component root)
    {
        List<Component> list = new();
        if (root != null)
            Traverse(root, list);
        return list;
    }

    private static void Traverse(Component comp, List<Component> list)
    {
        list.Add(comp);
        Component[] children = comp.GetChildren();
        foreach (Component child in children)
            Traverse(child, list);
    }
}