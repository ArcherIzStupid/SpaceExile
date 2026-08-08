using System.Collections.Generic;
using UnityEngine;

public enum TriggerTargetType
{
    Object,
    Group
}

public class EditorObjectRegistry : MonoBehaviour
{
    public static EditorObjectRegistry instance;

    private readonly Dictionary<int, EditableObject>
        objectsByID =
        new Dictionary<int, EditableObject>();

    private readonly Dictionary<int, List<EditableObject>>
        objectsByGroup =
        new Dictionary<int, List<EditableObject>>();

    private int nextObjectID = 1;

    private void Awake()
    {
        if (instance != null &&
            instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    //==================================================
    // REGISTRATION
    //==================================================

    public void Register(EditableObject obj)
    {
        if (obj == null)
            return;

        if (obj.objectID < 0)
        {
            obj.objectID =
                GenerateObjectID();
        }

        // Prevent duplicate IDs.
        if (objectsByID.TryGetValue(
                obj.objectID,
                out EditableObject existing))
        {
            if (existing != obj)
            {
                Debug.LogWarning(
                    $"Duplicate Object ID {obj.objectID} " +
                    $"found on {obj.name}. " +
                    $"Generating a new ID.");

                obj.objectID =
                    GenerateObjectID();
            }
        }

        objectsByID[obj.objectID] = obj;

        RegisterGroup(obj);
    }

    public void Unregister(EditableObject obj)
    {
        if (obj == null)
            return;

        if (objectsByID.TryGetValue(
                obj.objectID,
                out EditableObject existing) &&
            existing == obj)
        {
            objectsByID.Remove(obj.objectID);
        }

        RemoveFromGroup(obj);
    }

    //==================================================
    // LOOKUP
    //==================================================

    public EditableObject GetObject(int objectID)
    {
        objectsByID.TryGetValue(
            objectID,
            out EditableObject obj);

        return obj;
    }

    public IReadOnlyList<EditableObject>
        GetGroup(int groupID)
    {
        if (objectsByGroup.TryGetValue(
                groupID,
                out List<EditableObject> group))
        {
            return group;
        }

        return System.Array.Empty<EditableObject>();
    }

    //==================================================
    // GROUPS
    //==================================================

    public void RefreshGroup(
        EditableObject obj,
        int oldGroupID)
    {
        RemoveFromSpecificGroup(
            obj,
            oldGroupID);

        RegisterGroup(obj);
    }

    private void RegisterGroup(
        EditableObject obj)
    {
        if (obj.groupID < 0)
            return;

        if (!objectsByGroup.TryGetValue(
                obj.groupID,
                out List<EditableObject> group))
        {
            group =
                new List<EditableObject>();

            objectsByGroup.Add(
                obj.groupID,
                group);
        }

        if (!group.Contains(obj))
        {
            group.Add(obj);
        }
    }

    private void RemoveFromGroup(
        EditableObject obj)
    {
        if (obj.groupID < 0)
            return;

        RemoveFromSpecificGroup(
            obj,
            obj.groupID);
    }

    private void RemoveFromSpecificGroup(
        EditableObject obj,
        int groupID)
    {
        if (groupID < 0)
            return;

        if (!objectsByGroup.TryGetValue(
                groupID,
                out List<EditableObject> group))
        {
            return;
        }

        group.Remove(obj);

        if (group.Count == 0)
        {
            objectsByGroup.Remove(groupID);
        }
    }

    //==================================================
    // IDS
    //==================================================

    private int GenerateObjectID()
    {
        while (
            objectsByID.ContainsKey(nextObjectID))
        {
            nextObjectID++;
        }

        return nextObjectID++;
    }
}