using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class GunAspectHelper {
    public static readonly Dictionary<ushort, GUIContent> GUI_CONTENT = new Dictionary<ushort, GUIContent> {
        {GunAspect.REVOLVER_CYLINDER, new GUIContent("Revolver Cylinder")},
        {GunAspect.MAGAZINE, new GUIContent("Magazine Base")},
        {GunAspect.MANUAL_LOADING, new GUIContent("Manual Loading", "Aspect that allows inserting individual rounds into chamber, mags or cylinders.")},
        {GunAspect.CHAMBER, new GUIContent("Chamber")},
        {GunAspect.SLIDE, new GUIContent("Slide", "Regular slide component used in modern firearms")},
        {GunAspect.HAMMER, new GUIContent("Hammer", "Aspect to ignite a round, \"primers\" like in the glock fall ushorto this")},
        {GunAspect.FIRE_MODE, new GUIContent("Fire Mode")},
        {GunAspect.TRIGGER, new GUIContent("Trigger")},
        {GunAspect.RECOIL, new GUIContent("Recoil")},
        {GunAspect.EXTRACTOR_ROD, new GUIContent("Extractor Rod")},
        {GunAspect.THUMB_COCKING, new GUIContent("Thumb Cocking", "Allow the hammer to be cocked manually")},
        {GunAspect.TRIGGER_COCKING, new GUIContent("Trigger Cocking", "Cock the hammer if the trigger is pulled (Double Action Trigger)")},
        {GunAspect.HAMMER_VISUAL, new GUIContent("Hammer Visual")},
        {GunAspect.LOCKABLE_BOLT, new GUIContent("Lockable Bolt", "Turns the Slide aspect into a rotateable bolt")},
        {GunAspect.FIRING, new GUIContent("Firing Base", "Aspect to create the actual bullet")},
        {GunAspect.SLIDE_COCKING, new GUIContent("Slide Cocking", "Cock the hammer if the slide is back")},
        {GunAspect.THUMB_SAFETY, new GUIContent("Thumb Safety", "Safety switch that can be toggled via a switch")},
        {GunAspect.SLIDE_LOCK, new GUIContent("Slide Lock", "The Slide lock catches the slide in a pulled position. Releasable with the slide lock switch")},
        {GunAspect.SLIDE_LOCK_VISUAL, new GUIContent("Slide Lock Visual")},
        {GunAspect.SLIDE_SPRING, new GUIContent("Slide Spring", "Spring that pushes the slide forward if it isn't held by anything")},
        {GunAspect.SLIDE_PUSHING, new GUIContent("Slide Pushing")},
        {GunAspect.GRIP_SAFETY, new GUIContent("Grip Safety", "Safety machanism that engages if the gun is NOT aimed")},
        {GunAspect.SLIDE_VISUAL, new GUIContent("Slide Visual")},
        {GunAspect.EXTRACTOR_ROD_VISUAL, new GUIContent("Extractor Rod Visual")},
        {GunAspect.CYLINDER_VISUAL, new GUIContent("Cylinder Visual")},
        {GunAspect.AIM_SWAY, new GUIContent("Aim Sway")},
        {GunAspect.INTERNAL_MAGAZINE, new GUIContent("Internal Magazine")},
        {GunAspect.EXTERNAL_MAGAZINE, new GUIContent("External Magazine", "Regular spring loaded magazine that can be ejected")},
        {GunAspect.TRIGGER_VISUAL, new GUIContent("Trigger Visual")},
        {GunAspect.ALTERNATIVE_STANCE, new GUIContent("Alternative Stance Mode", "Adds logic for an alternative stance, this can be used to simulate multiple ways of holding a weapon.")},
        {GunAspect.YOKE, new GUIContent("Yoke", "Make your cylinder openable with a Yoke aspect")},
        {GunAspect.YOKE_VISUAL, new GUIContent("Yoke Visual")},
        {GunAspect.OPEN_BOLT_FIRING, new GUIContent("Open Bolt Firing")},
        {GunAspect.SLIDE_RELEASE_BUTTON, new GUIContent("Slide Release Button", "Aspect to allow for manual slide lock interactions like releasing via a button press.")},
        {GunAspect.FIRE_MODE_VISUAL, new GUIContent("Fire Mode Visual")},
        {GunAspect.THUMB_SAFETY_VISUAL, new GUIContent("Thumb Safety Visual")},
        {GunAspect.GRIP_SAFETY_VISUAL, new GUIContent("Grip Safety Visual")},
        {GunAspect.SLIDE_SPRING_VISUAL, new GUIContent("Slide Spring Visual")},
        {GunAspect.YOKE_AUTO_EJECTOR, new GUIContent("Yoke Auto Ejector", "When opening the yoke, eject every round in the cylinder.")},
    };

    public static readonly Dictionary<ushort, string> GUI_GROUP = new Dictionary<ushort, string> {
        {GunAspect.FIRING, "Firing"},
        {GunAspect.OPEN_BOLT_FIRING, "Firing"},
        {GunAspect.TRIGGER, "Firing"},
        {GunAspect.TRIGGER_VISUAL, "Firing"},
        {GunAspect.RECOIL, "Firing"},
        {GunAspect.FIRE_MODE, "Firing"},
        {GunAspect.FIRE_MODE_VISUAL, "Firing"},
        
        {GunAspect.HAMMER, "Hammer"},
        {GunAspect.THUMB_COCKING, "Hammer"},
        {GunAspect.TRIGGER_COCKING, "Hammer"},
        {GunAspect.HAMMER_VISUAL, "Hammer"},
        {GunAspect.SLIDE_COCKING, "Hammer"},

        {GunAspect.THUMB_SAFETY, "Safety"},
        {GunAspect.THUMB_SAFETY_VISUAL, "Safety"},
        {GunAspect.GRIP_SAFETY, "Safety"},
        {GunAspect.GRIP_SAFETY_VISUAL, "Safety"},

        {GunAspect.SLIDE, "Slide"},
        {GunAspect.SLIDE_LOCK, "Slide"},
        {GunAspect.SLIDE_RELEASE_BUTTON, "Slide"},
        {GunAspect.SLIDE_LOCK_VISUAL, "Slide"},
        {GunAspect.SLIDE_SPRING, "Slide"},
        {GunAspect.SLIDE_SPRING_VISUAL, "Slide"},
        {GunAspect.SLIDE_PUSHING, "Slide"},
        {GunAspect.SLIDE_VISUAL, "Slide"},
        {GunAspect.LOCKABLE_BOLT, "Slide"},

        {GunAspect.MAGAZINE, "Magazine"},
        {GunAspect.INTERNAL_MAGAZINE, "Magazine"},
        {GunAspect.EXTERNAL_MAGAZINE, "Magazine"},

        {GunAspect.EXTRACTOR_ROD, "Revolver"},
        {GunAspect.EXTRACTOR_ROD_VISUAL, "Revolver"},
        {GunAspect.REVOLVER_CYLINDER, "Revolver"},
        {GunAspect.CYLINDER_VISUAL, "Revolver"},
        {GunAspect.YOKE, "Revolver"},
        {GunAspect.YOKE_VISUAL, "Revolver"},
        {GunAspect.YOKE_AUTO_EJECTOR, "Revolver"},
    };

    public static string GetGUIGroup(this GunAspect aspect) {
        if(aspect.value.Length != 1) 
            return "None";
        
        if(GUI_GROUP.ContainsKey(aspect.value[0]))
            return GUI_GROUP[aspect.value[0]];
        return "None";
    }

    public static GUIContent GetGUIContent(this GunAspect aspect) {
        if(aspect.value.Length > 1) 
            return new GUIContent(aspect.ToString());
        
        if(aspect.value.Length <= 0)
            return new GUIContent("GunAspect.NONE");

        if(GUI_CONTENT.ContainsKey(aspect.value[0]))
            return GUI_CONTENT[aspect.value[0]];
        
        Debug.LogWarning($"GUI_CONTENT missing for GunAspect {aspect.value[0]}!");
        return new GUIContent("null");
    }
}

[System.Serializable]
public class GunAspect {
    public const ushort REVOLVER_CYLINDER = 0;
    public const ushort MAGAZINE = 1;
    public const ushort MANUAL_LOADING = 2;
    public const ushort CHAMBER = 3;
    public const ushort SLIDE = 4;
    public const ushort HAMMER = 5;
    public const ushort FIRE_MODE = 6;
    public const ushort TRIGGER = 7;
    public const ushort RECOIL = 8;
    public const ushort EXTRACTOR_ROD = 9;
    public const ushort THUMB_COCKING = 10;
    public const ushort TRIGGER_COCKING = 11;
    public const ushort HAMMER_VISUAL = 12;
    public const ushort LOCKABLE_BOLT = 13;
    public const ushort FIRING = 14;
    public const ushort SLIDE_COCKING = 15;
    public const ushort THUMB_SAFETY = 16;
    public const ushort SLIDE_LOCK = 17;
    public const ushort SLIDE_LOCK_VISUAL = 18;
    public const ushort SLIDE_SPRING = 19;
    public const ushort SLIDE_PUSHING = 20;
    public const ushort GRIP_SAFETY = 21;
    public const ushort SLIDE_VISUAL = 22;
    public const ushort EXTRACTOR_ROD_VISUAL = 23;
    public const ushort CYLINDER_VISUAL = 24;
    public const ushort AIM_SWAY = 25;
    public const ushort INTERNAL_MAGAZINE = 26;
    public const ushort EXTERNAL_MAGAZINE = 27;
    public const ushort TRIGGER_VISUAL = 28;
    public const ushort ALTERNATIVE_STANCE = 29;
    public const ushort YOKE = 30;
    public const ushort YOKE_VISUAL = 31;
    public const ushort OPEN_BOLT_FIRING = 32;
    public const ushort SLIDE_SPRING_VISUAL = 33;
    public const ushort SLIDE_RELEASE_BUTTON = 34;
    public const ushort FIRE_MODE_VISUAL = 35;
    public const ushort THUMB_SAFETY_VISUAL = 36;
    public const ushort GRIP_SAFETY_VISUAL = 37;
    public const ushort YOKE_AUTO_EJECTOR = 38;

    private const ushort MAX_VALUE = 38; // Used to determine an all bits enum
    public static readonly ushort[] ALL = System.Array.ConvertAll<int, ushort>(Enumerable.Range(0, MAX_VALUE + 1).ToArray(), item => (ushort)item);

    public ushort[] value = new ushort[0];

    public GunAspect(params ushort[] value) {
        this.value = value;
    }

    public bool HasFlag(GunAspect aspect) {
        if(aspect.IsEmpty() || IsEmpty())
            return false;

        foreach(ushort bit in aspect.value) {
            if(!value.Contains(bit)) {
                return false;
            }
        }
        return true;
    }

    public bool HasAnyFlag(GunAspect aspect) {
        if(aspect.IsEmpty() || IsEmpty())
            return false;

        foreach(ushort bit in aspect.value)
            if(value.Contains(bit))
                return true;
        return false;
    }

    public bool IsEmpty() {
        return value.Length <= 0;
    }

    public override string ToString() {
        return "GunAspect(" + string.Join(" | ", value) + ")";
    }

    // Operators
    public static GunAspect operator |(GunAspect a, GunAspect b) {
        return a.value.Union(b.value).ToArray();
    }

    public static GunAspect operator &(GunAspect a, GunAspect b) {
        return a.value.Intersect(b.value).ToArray();
    }

    public static GunAspect operator ^(GunAspect a, GunAspect b) { // Might be quite slow
        List<ushort> bits = new List<ushort>();
        foreach(ushort bit in GunAspect.ALL) {
            if(a.HasFlag(bit) != b.HasFlag(bit)) {
                bits.Add(bit);
            }
        }
        return bits.ToArray();
    }

    public static GunAspect operator ~(GunAspect a) {
        return a ^ GunAspect.ALL;
    }

    // Casting
    public static explicit operator ushort(GunAspect x) {
        if(x.value.Length != 1)
            throw new System.Exception("Tried to cast multiple GunAspects to one single GunAspect!");
        return x.value[0];
    }
    public static implicit operator ushort[](GunAspect x) => x.value;
    public static implicit operator GunAspect(ushort[] x) => new GunAspect(x);
    public static implicit operator GunAspect(ushort x) => new GunAspect(x);
}
