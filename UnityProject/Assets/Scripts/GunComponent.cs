using UnityEngine;
using System;
using System.Collections.Generic;
using ExtentionUtil;

public class IsNonNull : Attribute {}

public class AspectGroupAttribute : Attribute {
    public string group;
    public AspectGroupAttribute(string group) {
        this.group = group;
    }
}

public class HasTransformPathAttribute : Attribute {
    public string[] paths;
    public HasTransformPathAttribute(params string[] paths) {
        if(paths.Length <= 0)
            this.paths = new string[0];
        else
            this.paths = paths;
    }
}

public class GunInspectorAspectAttribute : Attribute {
    public GunAspect aspect;
    public GunInspectorAspectAttribute(GunAspect gun_aspect) {
        aspect = gun_aspect;
    }
}

public class GunDataAttribute : Attribute {
    public GunAspect gun_aspect;
    public GunDataAttribute(params ushort[] gun_aspect) {
        this.gun_aspect = gun_aspect;
    }
}

public class PriorityAttribute : Attribute {
    public const int VERY_EARLY = -100;
    public const int EARLY = -50;
    public const int NORMAL = 0;
    public const int LATE = 50;
    public const int VERY_LATE = 100;

    public int priority = NORMAL;

    public PriorityAttribute(int priority) {
        this.priority = priority;
    }
}

public class InclusiveAspectsAttribute : Attribute {
    public GunAspect inclusive_aspects;
    public InclusiveAspectsAttribute(params ushort[] inclusive_aspects) {
        this.inclusive_aspects = inclusive_aspects;
    }

    public bool IsConditionMet(GunAspect aspects) {
        return (inclusive_aspects & aspects) == inclusive_aspects;
    }
}

public class ExclusiveAspectsAttribute : Attribute {
    public GunAspect exclusive_aspects;
    public ExclusiveAspectsAttribute(params ushort[] exclusive_aspects) {
        this.exclusive_aspects = exclusive_aspects;
    }

    public bool IsConditionMet(GunAspect aspects) {
        return (exclusive_aspects & aspects).IsEmpty();
    }
}

public class Predicates : List<Func<bool>> {
    public bool AllTrue() {
        foreach(Func<bool> func in this)
            if(!func.Invoke())
                return false;
        return true;
    }

    public bool AllFalse() {
        foreach(Func<bool> func in this)
            if(func.Invoke())
                return false;
        return true;
    }
}

[System.Serializable]
public abstract class GunComponent : MonoBehaviour {
    public virtual void ResolveTransformReferences(Transform root) {}
    protected void ResolveReference(out Transform transform, Transform root, string name) {
        transform = root.Find(name, true);
    }
}