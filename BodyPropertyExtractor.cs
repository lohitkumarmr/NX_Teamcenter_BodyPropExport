using NXOpen;
using NXOpen.Assemblies;
using System.Collections.Generic;

public class BodyPropertyExtractor
{
    public static List<Body> GetBodies(Component component)
    {
        List<Body> bodies = new();
        Part part = component.Prototype as Part;
        if (part == null && component.Prototype is Component protComp)
            part = protComp.Prototype as Part;

        if (part != null)
        {
            foreach (Body body in part.Bodies)
            {
                if (body != null && body.IsSolidBody)
                    bodies.Add(body);
            }
        }
        return bodies;
    }
}