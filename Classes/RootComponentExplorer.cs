using NXOpen;
using NXOpen.Assemblies;
using System.Collections.Generic;
using System.Linq;

namespace NX_Teamcenter_Export
{
    public static class RootComponentExplorer
    {
        public static void LoadAllComponents(Part rootPart, ref List<Component> allComponents)
        {
            Component rootComponent = rootPart.ComponentAssembly?.RootComponent;
            if (rootComponent == null)
                return;

            Queue<Component> queue = new Queue<Component>();
            queue.Enqueue(rootComponent);

            while (queue.Count > 0)
            {
                Component current = queue.Dequeue();
                allComponents.Add(current);

                foreach (Component child in current.GetChildren())
                    queue.Enqueue(child);
            }
        }
    }
}